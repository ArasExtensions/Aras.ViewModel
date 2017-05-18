/*  
  Copyright 2017 Processwall Limited

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Web:     http://www.processwall.com
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
        public ViewModel.Grids.Search Parts { get; private set; }

        [ViewModel.Attributes.Property("Relationships", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.RelationshipTree Relationships { get; private set; }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();
       
            if (this.Binding != null && this.Binding is Model.Session)
            {
                // Get Part Store
                //Model.Stores.Item<Model.Item> partstore = new Model.Stores.Item<Model.Item>((Model.Session)this.Binding, "Part");

                // Set Part Properties to Select
                //partstore.ItemType.AddToSelect("item_number,major_rev,name,keyed_name");
                
                // Set Properties to display in Parts Search
                //this.Parts.SetPropertyNames("item_number,major_rev,name");

                // Set Binding for Parts
                //this.Parts.Binding = partstore;

                // Watch for changes in Parts selection
                this.Parts.Selected.ListChanged += Selected_ListChanged;

                // Add Search Columns to RelationshipTree
                //this.Relationships.Search.AddToPropertyNames("item_number,major_rev,name");

                // Add RelationshipType to RelationshipTree
                this.Relationships.RelationshipType = (((Model.Session)this.Binding).ItemType("Part").RelationshipType("Part BOM"));
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

        public PartEditor(ViewModel.Manager.Session Session)
            :base(Session)
        {
            this.Parts = new Grids.Search (this.Session);
            this.Relationships = new RelationshipTree(this.Session, new RelationshipFormatters.Part(), new ItemFormatters.Part());
        }
    }
}
