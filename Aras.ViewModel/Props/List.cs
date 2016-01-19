/*  
  Aras.Model provides a .NET cient library for Aras Innovator

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Properties
{
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

            if (this.Binding != null)
            {
                if (this.Binding is Model.Properties.List)
                {
                    if (Value == null)
                    {
                        ((Model.Properties.List)this.Binding).Value = null;
                    }
                    else
                    {
                        foreach(Model.ListValue listvalue in ((Model.Properties.List)this.Binding).Values.Relationships("Value"))
                        {
                            if (listvalue.Value.Equals(Value))
                            {
                                ((Model.Properties.List)this.Binding).Value = listvalue;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (Value == null)
                    {
                        ((Model.Properties.VariableList)this.Binding).Value = null;
                    }
                    else
                    {
                        foreach (Model.ListValue listvalue in ((Model.Properties.VariableList)this.Binding).Values.Relationships("Value"))
                        {
                            if (listvalue.Value.Equals(Value))
                            {
                                ((Model.Properties.VariableList)this.Binding).Value = listvalue;
                                break;
                            }
                        }
                    }
                }
            }
        }

        [Attributes.Property("Values", Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<ListValue> Values { get; private set; }

        public override object Binding
        {
            get
            {
                return base.Binding;
            }
            set
            {
                if (value == null)
                {
                    base.Binding = value;
                }
                else
                {
                    if (value is Model.Properties.List || value is Model.Properties.VariableList)
                    {
                        base.Binding = value;
                    }
                    else
                    {
                        throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.List or Aras.Model.Properties.VariableList");
                    }
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                Model.List list = null;
                Int32 selected = -1;

                if (this.Binding is Model.Properties.List)
                {
                    list = ((Model.Properties.List)this.Binding).Values;
                    selected = ((Model.Properties.List)this.Binding).Selected;
                }
                else
                {
                    list = ((Model.Properties.VariableList)this.Binding).Values;
                    selected = ((Model.Properties.VariableList)this.Binding).Selected;
                }

                this.Values.Clear();

                int cnt = 0;

                this.Values.NotifyListChanged = false;

                foreach(Model.ListValue modellistvalue in list.Relationships("Value"))
                {
                    ListValue listvalue = new ListValue();
                    listvalue.Binding = modellistvalue;
                    this.Values.Add(listvalue);

                    if (selected == cnt)
                    {
                        this.Value = listvalue.Value;
                    }

                    cnt++;
                }

                this.Values.NotifyListChanged = true;
            }
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (this.Binding != null)
            {
                if (e.PropertyName.Equals("Selected") || e.PropertyName.Equals("Value"))
                {
                    if (this.Binding is Model.Properties.VariableList)
                    {
                        if (((Model.Properties.VariableList)this.Binding).Selected == -1)
                        {
                            this.Value = null;
                        }
                        else
                        {
                            this.Value = ((Aras.Model.ListValue)((Model.Properties.VariableList)this.Binding).Value).Value;
                        }
                    }
                    else
                    {
                        if (((Model.Properties.List)this.Binding).Selected == -1)
                        {
                            this.Value = null;
                        }
                        else
                        {
                            this.Value = ((Aras.Model.ListValue)((Model.Properties.List)this.Binding).Value).Value;
                        }
                    }    
                }
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                this.Values.Clear();
            }
        }

        void Values_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Values");
        }

        public List()
            :base()
        {
            this.Values = new Model.ObservableList<ListValue>();
            this.Values.ListChanged += Values_ListChanged;
            this.Value = null;
        }


    }
}
