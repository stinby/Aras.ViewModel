﻿/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Design
{
    public class Order : Control
    {
        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("UpdateBOM")]
        public UpdateBOMCommand UpdateBOM { get; private set; }

        private ViewModel.Grid _bOM;
        [ViewModel.Attributes.Property("BOM", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid BOM
        {
            get
            {
                if (this._bOM == null)
                {
                    this._bOM = new Grid();
                    this.OnPropertyChanged("BOM");
                    this._bOM.AllowSelect = false;
                    this._bOM.AddColumn("number", "Number");
                    this._bOM.AddColumn("revision", "Revision");
                    this._bOM.AddColumn("name", "Name");
                    this._bOM.AddColumn("quantity", "Qty");
                }

                return this._bOM;
            }
        }

        private ViewModel.Grid _configuration;
        [ViewModel.Attributes.Property("Configuration", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid Configuration
        {
            get
            {
                if (this._configuration == null)
                {
                    this._configuration = new Grid();
                    this.OnPropertyChanged("Configuration");
                    this._configuration.AllowSelect = false;
                    this._configuration.AddColumn("rule", "Item");
                    this._configuration.AddColumn("value", "Required");
                    this._configuration.AddColumn("quantity", "Qty");
                }

                return this._configuration;
            }
        }

        protected override Model.Item GetContext(Model.Session Sesison, String ID)
        {
            try
            {
                return Sesison.Cache("v_Order").Get(ID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override object Binding
        {
            get
            {
                return base.Binding;
            }
            set
            {
                if (value == null)
                {
                    base.Binding = value;
                }
                else
                {
                    if (value is Model.Design.Order)
                    {
                        base.Binding = value;
                    }
                    else
                    {
                        throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Design.Order");
                    }
                }
            }
        }

        // Configuration Control Caches
        private ControlCache<Model.Design.OrderContext, Properties.String> ConfigQuestionCache;
        private ControlCache<Model.Design.OrderContext, Properties.List> ConfigValueCache;
        private ControlCache<Model.Design.OrderContext, Properties.Float> ConfigQuantityCache;

        // PartBOM Control Caches
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMNumberCache;
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMRevisionCache;
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMNameCache;
        private ControlCache<Model.Design.PartBOM, Properties.Float> PartBOMQuantityCache;

        private Model.Design.Order OrderModel
        {
            get
            {
                return (Model.Design.Order)this.Binding;
            }
        }

        private void UpdateBOMGrid()
        {
            if ((this.OrderModel != null) && (this.OrderModel.ConfiguredPart != null))
            {
                IEnumerable<Model.Design.PartBOM> currentpartboms = this.OrderModel.ConfiguredPart.PartBOMS.CurrentItems();

                // Set No of Rows
                this.BOM.NoRows = currentpartboms.Count();

                // Update BOM Grid
                int cnt = 0;

                foreach (Model.Design.PartBOM partbom in currentpartboms)
                {
                    if (partbom.Action != Model.Item.Actions.Delete)
                    {
                        Row row = this.BOM.Rows[cnt];

                        // Add Part Number
                        Properties.String numbercontrol = this.PartBOMNumberCache.Get(partbom);
                        numbercontrol.Binding = partbom.Related.Property("item_number");
                        row.Cells[0].Value = numbercontrol;
                        numbercontrol.Enabled = false;

                        // Add Part Revision
                        Properties.String revisioncontrol = this.PartBOMRevisionCache.Get(partbom);
                        revisioncontrol.Binding = partbom.Related.Property("major_rev");
                        row.Cells[1].Value = revisioncontrol;
                        revisioncontrol.Enabled = false;

                        // Add Part Name
                        Properties.String namecontrol = this.PartBOMNameCache.Get(partbom);
                        namecontrol.Binding = partbom.Related.Property("cmb_name");
                        row.Cells[2].Value = namecontrol;
                        namecontrol.Enabled = false;

                        // Add Quantity
                        Properties.Float quantitycontrol = this.PartBOMQuantityCache.Get(partbom);
                        quantitycontrol.Binding = partbom.Property("quantity");
                        row.Cells[3].Value = quantitycontrol;
                        quantitycontrol.Enabled = false;

                        cnt++;
                    }
                }
            }
            else
            {
                this.BOM.NoRows = 0;
            }

            this.OnPropertyChanged("BOM");
        }

        private void UpdateConfigurationGrid()
        {
            // Build List of OrderContext to Display
            List<Model.Design.OrderContext> ordercontexts = new List<Model.Design.OrderContext>();

            foreach (Model.Design.OrderContext ordercontext in this.OrderModel.OrderContexts)
            {
                if (!ordercontext.VariantContext.IsMethod)
                {
                    ordercontexts.Add(ordercontext);
                }
            }

            // Order
            ordercontexts.Sort(
                delegate(Model.Design.OrderContext p1, Model.Design.OrderContext p2)
                {
                    return p1.VariantContext.SortOrder.CompareTo(p2.VariantContext.SortOrder);
                }
            );

            // Update number of Rows
            this.Configuration.NoRows = ordercontexts.Count();

            // Update Configuration Grid
            int cnt = 0;

            foreach (Model.Design.OrderContext ordercontext in ordercontexts)
            {
                Row row = this.Configuration.Rows[cnt];

                // Add Question
                Properties.String questioncontrol = this.ConfigQuestionCache.Get(ordercontext);

                if (!String.IsNullOrEmpty((String)ordercontext.VariantContext.Property("question").Value))
                {
                    questioncontrol.Binding = ordercontext.VariantContext.Property("question");
                }
                else
                {
                    questioncontrol.Binding = ordercontext.VariantContext.Property("name");
                }

                row.Cells[0].Value = questioncontrol;
                
                // Add Values
                Properties.List valuecontrol = this.ConfigValueCache.Get(ordercontext);
                valuecontrol.Binding = ordercontext.Property("value_list");
                row.Cells[1].Value = valuecontrol;

                if (this.OrderModel.Locked(false))
                {
                    valuecontrol.Enabled = true;
                }
                else
                {
                    valuecontrol.Enabled = false;
                }

                // Add Quantity
                Properties.Float quantitycontrol = this.ConfigQuantityCache.Get(ordercontext);
                quantitycontrol.Binding = ordercontext.Property("quantity");
                row.Cells[2].Value = quantitycontrol;

                // Add Min Max Quantity
                quantitycontrol.MinValue = (System.Double)ordercontext.VariantContext.MinQuantity;
                quantitycontrol.MaxValue = (System.Double)ordercontext.VariantContext.MaxQuantity;

                if (this.OrderModel.Locked(false))
                {
                    quantitycontrol.Enabled = true;
                }
                else
                {
                    quantitycontrol.Enabled = false;
                }

                cnt++;
            }

            this.OnPropertyChanged("Configuration");
        }

        private Model.Transaction Transaction;

        private Boolean OrderLocked;

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                // Check if Order is already Locked
                this.OrderLocked = this.OrderModel.Locked(true);

                this.Update();

                // Add Event Handlers
                this.OrderModel.PropertyChanged += OrderModel_PropertyChanged;
                this.OrderModel.OrderContexts.StoreChanged += OrderContexts_StoreChanged;
            }
            else
            {
                this.OrderLocked = false;
            }
        }

        void OrderContexts_StoreChanged(object sender, Model.StoreChangedEventArgs e)
        {
            this.UpdateConfigurationGrid();
        }

        void OrderModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ConfiguredPart":
                    this.UpdateBOMGrid();
                    break;
                default:
                    break;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                // Rollbck Transaction

                if (this.Transaction != null)
                {
                    this.Transaction.RollBack();
                    this.Transaction = null;
                }

                // Remove Event Handlers
                this.OrderModel.OrderContexts.StoreChanged -= OrderContexts_StoreChanged;
                this.OrderModel.PropertyChanged -= OrderModel_PropertyChanged;

                // Clear Grids
                this.Configuration.Rows.Clear();
                this.BOM.Rows.Clear();
            }
        }

        private void Update()
        {
            if (this.OrderModel != null)
            {
                this.ResetError();

                try
                {
                    this.OrderModel.Refresh();

                    // Create Transaction if Order Locked

                    if (this.OrderModel.Locked(true))
                    {
                        if (this.OrderModel.Transaction == null)
                        {
                            this.Transaction = this.OrderModel.Session.BeginTransaction();
                            this.OrderModel.Update(this.Transaction, true);
                        }
                        else
                        {
                            this.Transaction = this.OrderModel.Transaction;
                        }

                        // Update BOM
                        this.OrderModel.UpdateBOM();

                        if (this.OrderModel.Part != null)
                        {
                            this.Save.UpdateCanExecute(true);
                            this.UpdateBOM.UpdateCanExecute(true);
                        }
                        else
                        {
                            this.Save.UpdateCanExecute(false);
                            this.UpdateBOM.UpdateCanExecute(false);
                        }
                    }
                    else
                    {
                        this.Save.UpdateCanExecute(false);
                        this.UpdateBOM.UpdateCanExecute(false);
                    }

                    // Update Grids
                    this.UpdateConfigurationGrid();
                    this.UpdateBOMGrid();
                }
                catch (Model.Exceptions.UnLockException e)
                {
                    // Failed to unlock Item
                    this.BOM.NoRows = 0;
                    this.Configuration.NoRows = 0;

                    this.OnError("Order Locked By: " + e.Item.LockedBy.KeyedName);
                }
            }
        }

        protected override void CloseControl()
        {
            base.CloseControl();

            // Rollback any changes
            if (this.Transaction != null)
            {
                this.Transaction.RollBack();
            }

            // If Order was originally locked then Lock
            if (this.OrderLocked)
            {
                if (this.OrderModel != null)
                {
                    Model.Transaction transaction = this.OrderModel.Session.BeginTransaction();
                    this.OrderModel.Update(transaction);
                }
            }
        }

        public Order()
            :base()
        {
            this.ConfigQuestionCache = new ControlCache<Model.Design.OrderContext, Properties.String>();
            this.ConfigValueCache = new ControlCache<Model.Design.OrderContext, Properties.List>();
            this.ConfigQuantityCache = new ControlCache<Model.Design.OrderContext, Properties.Float>();

            this.PartBOMNumberCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMRevisionCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMNameCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMQuantityCache = new ControlCache<Model.Design.PartBOM, Properties.Float>();

            this.Save = new SaveCommand(this);
            this.UpdateBOM = new UpdateBOMCommand(this);
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            // Update
            this.Update();
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            public Order Order { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.Order.Transaction != null)
                {
                    if (this.Order.OrderModel.Part != null)
                    {
                        // Process BOM
                        this.Order.OrderModel.UpdateBOM();

                        // Commit current transaction
                        this.Order.Transaction.Commit();

                        // Create new Transaction
                        this.Order.Transaction = this.Order.OrderModel.Session.BeginTransaction();
                        this.Order.OrderModel.Update(this.Order.Transaction);

                        // Update Grids
                        this.Order.UpdateConfigurationGrid();
                        this.Order.UpdateBOMGrid();

                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }

                return true;
            }

            internal SaveCommand(Order Order)
            {
                this.Order = Order;
                this.CanExecute = false;
            }
        }

        public class UpdateBOMCommand : Aras.ViewModel.Command
        {
            public Order Order { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.Order.Transaction != null)
                {
                    // Process BOM
                    this.Order.OrderModel.UpdateBOM();

                    // Update Grids
                    this.Order.UpdateConfigurationGrid();
                    this.Order.UpdateBOMGrid();

                    this.CanExecute = true;
                }

                return true;
            }

            internal UpdateBOMCommand(Order Order)
            {
                this.Order = Order;
                this.CanExecute = false;
            }
        }
    }
}
