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

namespace Aras.ViewModel.Debug
{
    public class Test : ViewModel.Application
    {
        public Properties.Control Search { get; private set; }

        public Test(Model.Session Session)
            :base(Session)
        {
            Model.ItemType itemtype = this.Session.ItemType("Part");
            List<Model.PropertyType> propertytypes = new List<Model.PropertyType>();
            propertytypes.Add(itemtype.PropertyType("item_number"));
            propertytypes.Add(itemtype.PropertyType("major_rev"));
            propertytypes.Add(itemtype.PropertyType("name"));

            this.Search = new Properties.Control(this, "Search", true, true, new ViewModel.Search(this.Session, itemtype, propertytypes));
            this.RegisterProperty(this.Search);
        }
    }
}
