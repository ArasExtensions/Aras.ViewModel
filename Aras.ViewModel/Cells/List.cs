/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

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

namespace Aras.ViewModel.Cells
{
    public class List : Cell
    {
        public override System.String ValueString
        {
            get
            {
                if (this.Object == null)
                {
                    return null;
                }
                else
                {
                    return (System.String)this.Object;
                }
            }
            set
            {
                this.Object = value;
            }
        }

        public override object Object
        {
            get
            {
                return base.Object;
            }
            set
            {
                if (value == null)
                {
                    base.Object = value;
                }
                else
                {
                    if (value is System.String)
                    {
                        base.Object = value;
                    }
                    else
                    {
                        throw new ArgumentException("Object must be type System.String");
                    }
                }
            }
        }

        [Attributes.Property("Value", Attributes.PropertyTypes.String, false)]
        public System.String Value
        {
            get
            {
                return (System.String)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        public static implicit operator System.String(Cells.List Cell)
        {
            return Cell.Value;
        }

        [Attributes.Property("Values", Attributes.PropertyTypes.StringList, true)]
        public ObservableLists.String Values { get; private set; }

        [Attributes.Property("Labels", Attributes.PropertyTypes.StringList, true)]
        public ObservableLists.String Labels { get; private set; }

        protected override void OnBindingChanged()
        {
            base.OnBindingChanged();

            // Update List Values

            if (this.Binding != null && this.Binding is Model.Properties.List)
            {
                Model.Properties.List property = (Model.Properties.List)this.Binding;
                Model.PropertyTypes.List propertytype = (Model.PropertyTypes.List)property.PropertyType;

                this.Values.NotifyListChanged = false;
                this.Values.Clear();

                foreach (Model.ListValue value in propertytype.Values.Values)
                {
                    this.Values.Add(value.Label);
                }

                this.Values.NotifyListChanged = true;
            }
        }

        internal List(Columns.List Column, Row Row)
            :base(Column, Row)
        {
            this.Values = new ObservableLists.String();
            this.Labels = new ObservableLists.String();
        }
    }
}
