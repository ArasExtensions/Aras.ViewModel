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

namespace Aras.ViewModel.Cells
{
    public class List : Cell
    {
        [Attributes.Property("Values", Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<ListValue> Values { get; private set; }

        private ListValue _selected;
        [Attributes.Property("Selected", Attributes.PropertyTypes.Control, true)]
        public ListValue Selected
        {
            get
            {
                return this._selected;
            }
            private set
            {
                if (this._selected == null)
                {
                    if (value != null)
                    {
                        this._selected = value;
                        this.OnPropertyChanged("Selected");
                    }
                }
                else
                {
                    if (!this._selected.Equals(value))
                    {
                        this._selected = value;
                        this.OnPropertyChanged("Selected");
                    }
                }
            }
        }

        public override void SetValue(object Value)
        {
            base.SetValue(Value);

            if (Value == null)
            {
                this.Value = null;
                this.Selected = null;
            }
            else
            {
                if (Value is Aras.Model.Relationships.Value)
                {
                    // Set Label
                    this.Value = (System.String)((Aras.Model.Relationships.Value)Value).Property("value").Value;

                    // Set Selected
                    System.String thisvalue = (System.String)((Aras.Model.Relationships.Value)Value).Property("value").Value;

                    foreach(ListValue listvalue in this.Values)
                    {
                        if (listvalue.Value.Equals(thisvalue))
                        {
                            this.Selected = listvalue;
                            break;
                        }
                    }
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be Aras.Model.Relationships.Value");
                }
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {
            if (this.Binding != null)
            {
                if (System.String.IsNullOrEmpty(Value))
                {
                    this.SetValue(null);
                }
                else
                {
                    foreach (Model.Relationships.Value listvalue in ((Model.Properties.List)this.Binding).Values.Relationships("Value"))
                    {
                        System.String thisvalue = (System.String)listvalue.Property("value").Value;

                        if (!System.String.IsNullOrEmpty(thisvalue))
                        {
                            if (thisvalue.Equals(Value))
                            {
                                this.SetValue(listvalue);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (System.String.IsNullOrEmpty(Value))
                {
                    this.Selected = null;
                    this.Value = null;
                }
                else
                {
                    foreach (ListValue listvalue in this.Values)
                    {
                        if (listvalue.Value.Equals(Value))
                        {
                            this.Selected = listvalue;
                            this.Value = listvalue.Value;
                            break;
                        }
                    }
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.List))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.List");
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.Values.NotifyListChanged = false;

            this.Values.Clear();

            if (this.Binding != null)
            {
                foreach (Model.Relationships.Value listvalue in ((Model.Properties.List)this.Binding).Values.Relationships("Value"))
                {
                    ListValue thisvalue = new ListValue(this.Session);
                    thisvalue.Binding = listvalue;
                    this.Values.Add(thisvalue);

                }
            }

            this.Values.NotifyListChanged = true;
        }

        internal List(Column Column, Row Row)
            :base(Column, Row)
        {
            this.Values = new Model.ObservableList<ListValue>();
        }
    }
}
