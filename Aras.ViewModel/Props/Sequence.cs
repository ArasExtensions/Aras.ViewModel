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
    [Attributes.ClientControl("Aras.View.Properties.Sequence")]
    public class Sequence : Property
    {

        private System.String _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.String, false)]
        public System.String Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Sequence))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Sequence");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Always Disabled
            this.Enabled = false;

            // Set Value
            this.SetValue();
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();
        }

        private void SetValue()
        {
            if (this.Binding != null)
            {
                if ((System.String)((Model.Properties.Sequence)this.Binding).Value == null)
                {
                    this.Value = "<Assigned by Server>";
                }
                else
                {
                    this.Value = (System.String)((Model.Properties.Sequence)this.Binding).Value;
                }
            }
            else
            {
                this.Value = null;
            }
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (this.Binding != null)
            {
                switch (e.PropertyName)
                {
                    case "Value":

                        this.SetValue();

                        break;
                    case "ReadOnly":

                        // Always Disabled
                        this.Enabled = false;

                        break;
                    default:
                        break;
                }
            }
        }

        public Sequence(Manager.Session Session)
            : base(Session)
        {
      
        }

        public Sequence(Manager.Session Session, Model.PropertyTypes.Sequence PropertyType)
            : base(Session, PropertyType)
        {

        }
    }
}
