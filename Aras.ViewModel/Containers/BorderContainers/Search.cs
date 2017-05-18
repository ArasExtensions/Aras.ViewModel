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

namespace Aras.ViewModel.Containers.BorderContainers
{
    public class Search : BorderContainer
    {
        public Containers.Toolbar Toolbar { get; private set; }

        public Grids.Search Grid { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Store))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Store");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Set Search Binding
            this.Grid.Binding = this.Binding;
        }

        public Search(Manager.Session Session)
            :base(Session)
        {
            // Create Search
            this.Grid = new Grids.Search(this.Session);
            this.Grid.Region = Regions.Center;

            // Set Toolbar
            this.Toolbar = ((IToolbarProvider)this.Grid).Toolbar;
            this.Toolbar.Region = Regions.Top;

            // Add Children
            this.Children.Add(this.Toolbar);
            this.Children.Add(this.Grid);
        }

    }
}
