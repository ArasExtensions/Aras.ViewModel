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
    [Attributes.ClientControl("Aras.View.Properties.List")]
    public class List : Property
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
                if (value == null)
                {
                    if (this._value != null)
                    {
                        this.SetValue(value);
                    }
                }
                else
                {
                    if (!value.Equals(this._value))
                    {
                        this.SetValue(value);
                    }
                }
            }
        }

        private void SetValue(System.String Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");
            this.OnAfterSetValue();
        }

        protected virtual void OnAfterSetValue()
        {
            if (this.Binding != null)
            {
                this.UpdatingBinding = true;

                if (Value == null)
                {
                    ((Model.Properties.List)this.Binding).Value = null;
                }
                else
                {
                    foreach (Model.Relationships.Value listvalue in ((Model.Properties.List)this.Binding).Values.Relationships("Value"))
                    {
                        if (listvalue.Property("value").Value.Equals(Value))
                        {
                            ((Model.Properties.List)this.Binding).Value = listvalue;
                            break;
                        }
                    }
                }

                this.UpdatingBinding = false;
            }
        }

        [Attributes.Property("Values", Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<ListValue> Values { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.List))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.List");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                Model.Relationships.Value modelvalue = (Model.Relationships.Value)((Model.Properties.List)this.Binding).Value;

                // Set Value
                if (modelvalue != null)
                {
                    this.Value = (System.String)modelvalue.Property("value").Value;
                }
                else
                {
                    this.Value = null;
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
                if (e.PropertyName.Equals("Selected") || e.PropertyName.Equals("Value"))
                {
                    if (!this.UpdatingBinding)
                    {
                        if (((Model.Properties.List)this.Binding).Selected == -1)
                        {
                            this.Value = null;
                        }
                        else
                        {
                            this.Value = (System.String)((Aras.Model.Relationships.Value)((Model.Properties.List)this.Binding).Value).Property("value").Value;
                        }
                    }
                }
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

        }

        void Values_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Values");
        }

        public List(Manager.Session Session)
            :base(Session)
        {
            this.Values = new Model.ObservableList<ListValue>();
            this.Values.ListChanged += Values_ListChanged;
            this.Value = null;
        }

        public List(Manager.Session Session, Model.PropertyTypes.List PropertyType)
            : base(Session, PropertyType)
        {
            this.Values = new Model.ObservableList<ListValue>();
            this.Value = null;

            if (PropertyType != null)
            {
                if (!PropertyType.Required)
                {
                    // Add Blank Value if Property is not required
                    ListValue blank = new ListValue(this.Session);
                    blank.Value = null;
                    blank.Label = " ";
                    this.Values.Add(blank);
                }

                // Add Values
                foreach (Model.Relationships.Value modellistvalue in PropertyType.Values.Relationships("Value"))
                {
                    ListValue listvalue = new ListValue(this.Session);
                    listvalue.Binding = modellistvalue;
                    this.Values.Add(listvalue);
                }
            }

            // Watch for changes in Values
            this.Values.ListChanged += Values_ListChanged;
        }
    }
}
