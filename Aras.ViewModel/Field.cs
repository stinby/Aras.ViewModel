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
    public abstract class Field : Control
    {
        [Attributes.Property("Required", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Required { get; set; }

        [Attributes.Property("ReadOnly", Attributes.PropertyTypes.Boolean, true)]
        public Boolean ReadOnly { get; set; }

        private Model.Property _binding;
        public Model.Property Binding
        {
            get
            {
                return this._binding;
            }
            set
            {
                if (value == null)
                {
                    if (this._binding != null)
                    {
                        this._binding = null;
                        this.OnPropertyChanged("Binding");
                    }
                }
                else
                {
                    if (this._binding == null || !this._binding.Equals(value))
                    {
                        this._binding = value;
                        this.OnPropertyChanged("Binding");
                    }
                }
            }
        }

        protected virtual void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Binding")
            {
                if (this.Binding != null)
                {
                    Model.PropertyType proptype = this.Binding.PropertyType;
                    this.ReadOnly = proptype.ReadOnly;
                }
            }
        }

        public Field(Session Session, Boolean Required, Boolean ReadOnly)
           :base(Session)
        {
            this.Required = Required;
            this.ReadOnly = ReadOnly;
            this.PropertyChanged += Property_PropertyChanged;
        }
    }
}