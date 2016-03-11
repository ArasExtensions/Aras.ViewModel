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
    public class PartEditor : Control
    {
        [ViewModel.Attributes.Property("Parts", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Searches.Item Parts { get; private set; }

        [ViewModel.Attributes.Property("Relationships", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.RelationshipTree Relationships { get; private set; }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();
       
            if (this.Binding != null && this.Binding is Model.Session)
            {
                // Get Part Store
                Model.Stores.Item partstore = ((Model.Session)this.Binding).Store("Part");

                // Set Part Properties to Select
                partstore.ItemType.AddToSelect("item_number,major_rev,name,keyed_name");
                
                // Set Properties to display in Parts Search
                this.Parts.SetPropertyNames("item_number,major_rev,name");

                // Set Binding for Parts
                this.Parts.Binding = partstore;

                // Watch for changes in Parts selection
                this.Parts.Selected.ListChanged += Selected_ListChanged;

                // Add Search Columns to RelationshipTree
                this.Relationships.Search.AddToPropertyNames("item_number,major_rev,name");

                // Add RelationshipType to RelationshipTree
                this.Relationships.AddRelationshipType(((Model.Session)this.Binding).ItemType("Part").RelationshipType("Part BOM"));
            }
            else
            {
                this.Parts.Binding = null;
            }
        }

        private void Selected_ListChanged(object sender, EventArgs e)
        {
            if (this.Parts.Selected.Count() == 0)
            {
                this.Relationships.Binding = null;
            }
            else
            {
                this.Relationships.Binding = this.Parts.Selected.First();
            }
        }

        public PartEditor()
            :base()
        {
            this.Parts = new Searches.Item();
            this.Relationships = new RelationshipTree(new ItemFormatters.Part());
        }
    }
}
