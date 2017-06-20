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

namespace Aras.ViewModel.Design.Applications
{
    [Aras.ViewModel.Attributes.Application("Parts", "PartFamily", "Design", false)]
    public class Parts : Aras.ViewModel.Containers.Application
    {
        public Model.Design.Queries.Searches.Part SearchQuery { get; private set; }

        public Aras.ViewModel.Grids.Search Search { get; private set; }

        public Model.Design.Queries.Forms.Part FormQuery { get; private set; }

        public Forms.Part Form { get; private set; }

        private void Search_ItemsSelected(object sender, Aras.ViewModel.Grids.Search.ItemsSelectedEventArgs e)
        {
            if (this.Search.Selected.Count() > 0)
            {
                this.Form.Binding = this.Form.Store.Get(this.Search.Selected.First().ID);
            }
            else
            {
                this.Form.Binding = null;
            }
        }

        public Parts(Aras.ViewModel.Manager.Session Session)
            : base(Session)
        {
            this.Children.NotifyListChanged = false;

            // Create Search Query
            this.SearchQuery = new Model.Design.Queries.Searches.Part(this.Session.Model);

            // Create Search
            this.Search = new Aras.ViewModel.Grids.Search(this.Session);
            this.Search.Width = 300;
            this.Children.Add(this.Search);
            this.Search.Region = Aras.ViewModel.Regions.Left;
            this.Search.Binding = this.SearchQuery.Store;
            this.Search.Splitter = true;
            this.Search.ItemsSelected += Search_ItemsSelected;

            // Create Form Query
            this.FormQuery = new Model.Design.Queries.Forms.Part(this.Session.Model);

            // Create Form
            this.Form = new Forms.Part(this.Session, this.FormQuery.Store);
            this.Children.Add(this.Form);

            this.Children.NotifyListChanged = true;

            // Select First Part
            if (this.SearchQuery.Store.Count() > 0)
            {
                this.Search.Select(this.SearchQuery.Store.First());
            }
        }
    }
}
