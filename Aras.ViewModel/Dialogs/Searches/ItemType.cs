﻿/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2017 Processwall Limited.

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

namespace Aras.ViewModel.Dialogs.Searches
{
    public class ItemType : Dialog
    {
        public Containers.Toolbar Toolbar { get; private set; }

        public Grids.Searches.ItemType Search { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Stores.Item))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Stores.Item");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Set Search Binding
            this.Search.Binding = this.Binding;

            // Set Title
            if (this.Binding != null)
            {
                this.Title = "Select " + ((Model.Stores.Item)this.Binding).ItemType.SingularLabel;
            }
            else
            {
                this.Title = null;
            }
        }

        public ItemType(Manager.Session Session)
            :base(Session)
        {
            // Set Default Width
            this.Width = 600;

            // Set Default Height
            this.Height = 800;

            // Create Border Container
            Containers.BorderContainer bordercontainer = new Containers.BorderContainer(this.Session);
            this.Content = bordercontainer;

            // Create Search
            this.Search = new Grids.Searches.ItemType(this.Session);
            this.Search.Region = Regions.Center;
            bordercontainer.Children.Add(this.Search);

            // Set Toolbar
            this.Toolbar = ((IToolbarProvider)this.Search).Toolbar;
            this.Toolbar.Region = Regions.Top;
            bordercontainer.Children.Add(this.Toolbar);
        }
    }
}