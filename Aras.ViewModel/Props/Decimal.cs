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
    [Attributes.ClientControl("Aras.View.Properties.Decimal")]
    public class Decimal : Property
    {
        const System.Decimal DefaultMinValue = System.Decimal.MinValue;
        const System.Decimal DefaultMaxValue = System.Decimal.MaxValue;

        private System.Decimal? _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Decimal, false)]
        public System.Decimal? Value
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
                        if ((System.Decimal)value > this.MaxValue)
                        {
                            this.SetValue(this.MaxValue);
                        }
                        else if ((System.Decimal)value < this.MinValue)
                        {
                            this.SetValue(this.MinValue);
                        }
                        else
                        {
                            this.SetValue(value);
                        }
                    }
                }
                else
                {
                    if (value != null)
                    {
                        if ((System.Decimal)value > this.MaxValue)
                        {
                            this.SetValue(this.MaxValue);
                        }
                        else if ((System.Decimal)value < this.MinValue)
                        {
                            this.SetValue(this.MinValue);
                        }
                        else
                        {
                            if (!this._value.Equals(value))
                            {
                                this.SetValue(value);
                            }
                        }
                    }
                    else
                    {
                        this.SetValue(value);
                    }
                }
            }
        }

        private System.Decimal _minValue;
        [Attributes.Property("MinValue", Attributes.PropertyTypes.Decimal, true)]
        public System.Decimal MinValue
        {
            get
            {
                return this._minValue;
            }
            set
            {
                this._minValue = value;
                this.OnPropertyChanged("MinValue");
            }
        }

        private System.Decimal _maxValue;
        [Attributes.Property("MaxValue", Attributes.PropertyTypes.Decimal, true)]
        public System.Decimal MaxValue
        {
            get
            {
                return this._maxValue;
            }
            set
            {
                this._maxValue = value;
                this.OnPropertyChanged("MaxValue");
            }
        }

        private void SetValue(System.Decimal? Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                ((Model.Properties.Decimal)this.Binding).Value = this.Value;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Decimal))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Decimal");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.Value = (System.Decimal?)((Model.Properties.Decimal)this.Binding).Value;
            }
            else
            {
                this.Value = null;
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
                        this.Value = (System.Decimal?)((Model.Properties.Decimal)this.Binding).Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public Decimal(Manager.Session Session)
            : base(Session)
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }

        public Decimal(Manager.Session Session, Model.PropertyTypes.Decimal PropertyType)
            : base(Session, PropertyType)
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }
    }
}
