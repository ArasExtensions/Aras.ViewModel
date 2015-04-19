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
        internal override void SetObject(object value)
        {
            if (value == null || value is Aras.Model.Cache.Item)
            {
                base.SetObject(value);
            }
            else
            {
                throw new Exceptions.ValueTypeException("Aras.Model.Cache.Item");
            }
        }

        public Aras.Model.Cache.Item Value
        {
            get
            {
                return (Aras.Model.Cache.Item)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        public Item(ViewModel.Control Control, System.String Name, Boolean Required, Boolean ReadOnly, Model.Cache.Item Default)
            : base(Control, Name, Required, ReadOnly)
        {
            this.SetObject(Default);
        }
    }
}
