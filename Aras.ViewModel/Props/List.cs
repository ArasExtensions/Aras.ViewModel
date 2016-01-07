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
        private System.Int32 _selected;
        [Attributes.Property("Selected", Attributes.PropertyTypes.Int32, false)]
        public System.Int32 Selected
        {
            get
            {
                return this._selected;
            }
            set
            {
                if (this._selected != value)
                {
                    this._selected = value;
                    this.OnPropertyChanged("Selected");
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

                foreach(Model.ListValue modellistvalue in list.Relationships("Value"))
                {
                    ListValue listvalue = new ListValue();
                    listvalue.Binding = modellistvalue;
                    this.Values.Add(listvalue);
                }

                this.Selected = selected;
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

        public List()
            :base()
        {
            this.Values = new Model.ObservableList<ListValue>();
            this.Selected = -1;
        }
    }
}
