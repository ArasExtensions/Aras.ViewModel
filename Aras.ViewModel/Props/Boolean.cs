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

namespace Aras.ViewModel.Properties
{
    [Attributes.ClientControl("Aras.View.Properties.Boolean")]
    public class Boolean : Property
    {

        private System.Boolean _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Boolean, false)]
        public System.Boolean Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this.SetValue(value);
                }
            }
        }

        private void SetValue(System.Boolean Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                this.UpdatingBinding = true;
                ((Model.Properties.Boolean)this.Binding).Value = this.Value;
                this.UpdatingBinding = false;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Boolean))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Boolean");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (((Model.Properties.Boolean)this.Binding).Value == null)
                {
                    this.Value = false;
                }
                else
                {
                    this.Value = (System.Boolean)((Model.Properties.Boolean)this.Binding).Value;
                }
            }
            else
            {
                this.Value = false;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (this.Binding != null)
            {
                switch (e.PropertyName)
                {
                    case "Value":

                        if (!this.UpdatingBinding)
                        {
                            this.Value = (System.Boolean)((Model.Properties.Boolean)this.Binding).Value;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        public Boolean(Manager.Session Session)
            : base(Session)
        {
            this.Width = null;
        }

        public Boolean(Manager.Session Session, Model.PropertyTypes.Boolean PropertyType)
            : base(Session, PropertyType)
        {
            this.Width = null;
        }
    }
}
