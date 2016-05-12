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

namespace Aras.ViewModel.Design
{
    public class PartRelationshipTree : RelationshipTree
    {
        protected override Model.Item GetContext(Model.Session Session, string ID)
        {
            // Set Columns to show in Search
            this.Search.AddToPropertyNames("item_number,major_rev,name");

            // Set Part Properties to Select
            Session.ItemType("Part").AddToSelect("item_number,major_rev,name,keyed_name");

            // Ensure RelationshipType Set
            this.RelationshipType = (Session.ItemType("Part").RelationshipType("Part BOM"));

            // Return Part
            return Session.Cache("Part").Get(ID);
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Set Columns to show in Search
            this.Search.AddToPropertyNames("item_number,major_rev,name");
        }

        public PartRelationshipTree()
            : base(new RelationshipFormatters.Part(), new ItemFormatters.Part())
        {

        }
    }
}
