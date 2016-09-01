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
        const System.Double DefaultMinValue = -10000.0;
        const System.Double DefaultMaxValue = 10000.0;

        private System.Double? _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Float, false)]
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
                        this.SetValue(value);
                    }
                }
                else
                {
                    if (!this._value.Equals(value))
                    {
                        this.SetValue(value);     
                    }
                }
            }
        }

        private System.Double _minValue;
        [Attributes.Property("MinValue", Attributes.PropertyTypes.Float, true)]
        public System.Double MinValue
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

        private System.Double _maxValue;
        [Attributes.Property("MaxValue", Attributes.PropertyTypes.Float, true)]
        public System.Double MaxValue
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

        private void SetValue(System.Double? Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                ((Model.Properties.Float)this.Binding).Value = this.Value;
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
                this.Value = (System.Double?)((Model.Properties.Float)this.Binding).Value;
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
                        this.Value = (System.Double?)((Model.Properties.Float)this.Binding).Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public Float()
            : base()
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }
    }
}
