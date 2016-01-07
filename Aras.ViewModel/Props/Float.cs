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
    public class Float : Property
    {

        private System.Double? _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Float, true)]
        public System.Double? Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value == null)
                {
                    if (value != null)
                    {
                        this._value = value;
                        this.OnPropertyChanged("Value");
                    }
                }
                else
                {
                    if (!this._value.Equals(value))
                    {
                        this._value = value;
                        this.OnPropertyChanged("Value");
                    }
                }
            }
        }

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
                    if (value is Model.Properties.Float)
                    {
                        base.Binding = value;
                    }
                    else
                    {
                        throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Float");
                    }
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Properties.Float)this.Binding).PropertyChanged += Model_PropertyChanged;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Properties.Float)this.Binding).PropertyChanged -= Model_PropertyChanged;
            }
        }

        void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Value":
                    this.Value = (System.Double?)((Model.Properties.Float)this.Binding).Value;
                    break;
                default:
                    break;
            }
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Value":
                    ((Model.Properties.Float)this.Binding).Value = this.Value;
                    break;
                default:
                    break;
            }
        }

        public Float()
            : base()
        {
            this.PropertyChanged += ViewModel_PropertyChanged;
        }
    }
}
