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

namespace Aras.ViewModel.Containers
{
    [Attributes.ClientControl("Aras.View.Containers.Application")]
    public abstract class Application : Containers.BorderContainer, IToolbarProvider
    {
        private Containers.Toolbar _toolbar;
        [Attributes.Property("Toolbar", Attributes.PropertyTypes.Control, true)]
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);
                }

                return this._toolbar;
            }
        }

        private System.String _name;
        [Attributes.Property("Name", Attributes.PropertyTypes.String, true)]
        public System.String Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        private System.String _label;
        [Attributes.Property("Label", Attributes.PropertyTypes.String, true)]
        public System.String Label
        {
            get
            {
                return this._label;
            }
            set
            {
                if (this._label != value)
                {
                    this._label = value;
                    this.OnPropertyChanged("Label");
                }
            }
        }

        private System.String _icon;
        [Attributes.Property("Icon", Attributes.PropertyTypes.String, true)]
        public System.String Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                if (this._icon != value)
                {
                    this._icon = value;
                    this.OnPropertyChanged("Icon");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();
        }

        private List<Control> AddedToolbars;
        private void Children_ListChanged(object sender, EventArgs e)
        {
            // Check for Toolbar Providers
            foreach(Control control in this.Children)
            {
                if (control is IToolbarProvider)
                {
                    if (!this.AddedToolbars.Contains(control))
                    {
                        if (this.Toolbar.Children.Count() > 0)
                        {
                            // Add Seperator
                            ToolbarSeparator sep = new ToolbarSeparator(this.Session);
                            this.Toolbar.Children.Add(sep);
                        }

                        // Add Toolbar Controls from Child
                        foreach(Control toolbarcontrol in ((IToolbarProvider)control).Toolbar.Children)
                        {
                            this.Toolbar.Children.Add(toolbarcontrol);
                        }

                        this.AddedToolbars.Add(control);
                    }
                }
            }
        }

        public Application(Manager.Session Session)
            : base(Session)
        {
            this.AddedToolbars = new List<Control>();
            this.Children.ListChanged += Children_ListChanged;
        }
    }
}
