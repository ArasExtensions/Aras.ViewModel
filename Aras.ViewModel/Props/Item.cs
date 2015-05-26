/*  
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

namespace Aras.ViewModel.Properties
{
    public class Item : Property
    {
        public Aras.Model.Item Value
        {
            get
            {
                return (Aras.Model.Item)this.PropertiesCache["Value"];
            }
            set
            {
                if (this.PropertiesCache["Value"] == null)
                {
                    if (value != null)
                    {
                        this.PropertiesCache["Value"] = value;
                        this.OnPropertyChanged("Value");
                    }
                }
                else
                {
                    if (!((Aras.Model.Item)this.PropertiesCache["Value"]).Equals(value))
                    {
                        this.PropertiesCache["Value"] = value;
                        this.OnPropertyChanged("Value");
                    }
                }
            }
        }

        public Item(Session Session, Boolean Required, Boolean ReadOnly, Model.Item Default)
            : base(Session, Required, ReadOnly)
        {
            this.PropertiesCache["Value"] = Default;
        }

        public Item(Session Session, Boolean Required, Boolean ReadOnly)
            :this(Session, Required, ReadOnly, null)
        {
        }
    }
}
