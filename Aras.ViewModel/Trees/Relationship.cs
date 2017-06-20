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

namespace Aras.ViewModel.Trees
{
    public class Relationship : Tree, IToolbarProvider
    {
        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        private Button _refreshButton;
        public Button RefreshButton
        {
            get
            {
                if (this._refreshButton == null)
                {
                    this._refreshButton = new Button(this.Session);
                    this._refreshButton.Icon = "Refresh";
                    this._refreshButton.Tooltip = "Refresh";
                    this._refreshButton.Command = this.Refresh;
                }

                return this._refreshButton;
            }
        }

        private Containers.Toolbar _toolbar;
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);

                    // Stop Notification
                    this._toolbar.Children.NotifyListChanged = false;

                    // Add Refresh Button
                    this._toolbar.Children.Add(this.RefreshButton);

                    // Start Notification
                    this._toolbar.Children.NotifyListChanged = true;
                }

                return this._toolbar;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Item))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be a Aras.Model.Item");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.Root = new RelationshipNode(this, null);
                this.Root.Binding = this.Binding;
            }
            else
            {
                this.Root = null;
            }
        }

        public void RefreshTree()
        {

        }

        public Relationship(Manager.Session Session, Type NodeFormatter)
            : base(Session, NodeFormatter)
        {

        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Relationship)this.Control).RefreshTree();
                this.CanExecute = true;
            }

            internal RefreshCommand(Control Control)
                : base(Control)
            {
                this.CanExecute = true;
            }
        }

    }
}
