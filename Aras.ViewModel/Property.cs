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

namespace Aras.ViewModel
{
    public abstract class Property : Control
    {
        private Boolean _required;
        [Attributes.Property("Required", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Required
        {
            get
            {
                return this._required;
            }
            set
            {
                if (this._required != value)
                {
                    this._required = value;
                    this.OnPropertyChanged("Required");
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

        private System.Boolean _intermediateChanges;
        [Attributes.Property("IntermediateChanges", Attributes.PropertyTypes.Boolean, true)]
        public System.Boolean IntermediateChanges
        {
            get
            {
                return this._intermediateChanges;
            }
            set
            {
                if (this._intermediateChanges != value)
                {
                    this._intermediateChanges = value;
                    this.OnPropertyChanged("IntermediateChanges");
                }
            }
        }

        public Model.PropertyType PropertyType { get; private set; }

        protected Boolean UpdatingBinding { get; set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Property))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Property");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                // Watch for Property Changes on Binding
                ((Model.Property)this.Binding).PropertyChanged += Property_PropertyChanged;

                // Set Enabled
                this.Enabled = !((Model.Property)this.Binding).ReadOnly;
            }
            else
            {
                this.Enabled = false;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Property)this.Binding).PropertyChanged -= Property_PropertyChanged;
            }
        }

        protected virtual void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadOnly":
                    this.Enabled = !((Model.Property)sender).ReadOnly;
                    break;
                default:
                    break;
            }
        }

        public Property(Manager.Session Session)
            :base(Session)
        {
            this.UpdatingBinding = false;
            this.IntermediateChanges = false;
            this.Required = false;
            this.Width = 180;
        }

        public Property(Manager.Session Session, Model.PropertyType PropertyType)
            : this(Session)
        {
            this.UpdatingBinding = false;
            this.IntermediateChanges = false;
            this.PropertyType = PropertyType;
            this.Label = PropertyType.Label;
        }
    }
}
