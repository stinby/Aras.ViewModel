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
    public class Column : Control
    {
        public Grid Grid { get; private set; }

        public System.String Name 
        { 
            get
            {
                return (System.String)this.PropertiesCache["Name"];
            }
        }

        public System.String Label 
        { 
            get
            {
                return (System.String)this.PropertiesCache["Label"];
            }
        }

        public override string ToString()
        {
            return this.Label;
        }

        internal Column(Session Session, Grid Grid, String Name, String Label)
            :base(Session)
        {
            this.Grid = Grid;
            this.PropertiesCache["Name"] = Name;
            this.PropertiesCache["Label"] = Label;
        }
    }
}
