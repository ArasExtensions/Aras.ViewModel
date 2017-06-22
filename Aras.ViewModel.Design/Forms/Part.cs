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

namespace Aras.ViewModel.Design.Forms
{
    [Aras.ViewModel.Attributes.Form("Part")]
    public class Part : Aras.ViewModel.Containers.Form
    {
        public Aras.ViewModel.Containers.TabContainer TabContainer { get; private set; }

        public Panes.Part DetailPane { get; private set; }

        public ViewModel.Containers.BorderContainers.RelationshipTree Tree { get; private set; }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.DetailPane.Binding = this.Binding;
        }

        public Part(Aras.ViewModel.Manager.Session Session, Aras.Model.Store Store)
            : base(Session, Store)
        {
            this.Children.NotifyListChanged = false;

            // Create TabContainer
            this.TabContainer = new Aras.ViewModel.Containers.TabContainer(this.Session);
            this.TabContainer.Region = Aras.ViewModel.Regions.Top;
            this.TabContainer.Splitter = true;
            this.Children.Add(this.TabContainer);

            // Create Detail Pane
            this.DetailPane = new Panes.Part(this.Session);
            this.DetailPane.Title = "Details";
            this.TabContainer.Children.Add(this.DetailPane);

            // Create Relationship Tree
            this.Tree = new Containers.BorderContainers.RelationshipTree(this, typeof(Design.NodeFormatters.Part));
            this.Tree.Region = Regions.Center;
            this.Tree.Splitter = true;
            this.Children.Add(this.Tree);

            this.Children.NotifyListChanged = true;
        }
    }
}
