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

namespace Aras.ViewModel
{
    public abstract class Property : Control
    {
        private Boolean _required;
        [Attributes.Property("Required", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Required
        {
            get
            {
                return this._required;
            }
            set
            {
                if (this._required != value)
                {
                    this._required = value;
                    this.OnPropertyChanged("Required");
                }
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
                    if (value is Model.Property)
                    {
                        base.Binding = value;
                    }
                    else
                    {
                        throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Property");
                    }
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Property)this.Binding).PropertyChanged += Property_PropertyChanged;
                this.ReadOnly = ((Model.Property)this.Binding).ReadOnly;
                
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Property)this.Binding).PropertyChanged -= Property_PropertyChanged;
            }
        }

        void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadOnly":
                    this.ReadOnly = ((Model.Property)sender).ReadOnly;
                    break;
                default:
                    break;
            }
        }

        public Property()
            :base()
        {
            this.Required = false;
        }
    }
}