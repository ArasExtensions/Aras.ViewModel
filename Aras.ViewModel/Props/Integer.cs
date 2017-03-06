/*  
  Aras.Model provides a .NET cient library for Aras Innovator

  Copyright (C) 2017 Processwall Limited.

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
    [Attributes.ClientControl("Aras.View.Properties.Integer")]
    public class Integer : Property
    {
        const System.Int32 DefaultMinValue = System.Int32.MinValue;
        const System.Int32 DefaultMaxValue = System.Int32.MaxValue;

        private System.Int32? _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.NullableInt32, false)]
        public System.Int32? Value
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
                        if ((System.Int32)value > this.MaxValue)
                        {
                            this.SetValue(this.MaxValue);
                        }
                        else if ((System.Int32)value < this.MinValue)
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
                        if ((System.Int32)value > this.MaxValue)
                        {
                            this.SetValue(this.MaxValue);
                        }
                        else if ((System.Int32)value < this.MinValue)
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

        private System.Int32 _minValue;
        [Attributes.Property("MinValue", Attributes.PropertyTypes.Int32, true)]
        public System.Int32 MinValue
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

        private System.Int32 _maxValue;
        [Attributes.Property("MaxValue", Attributes.PropertyTypes.Int32, true)]
        public System.Int32 MaxValue
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

        private void SetValue(System.Int32? Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                this.UpdatingBinding = true;
                ((Model.Properties.Integer)this.Binding).Value = this.Value;
                this.UpdatingBinding = false;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Integer))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Integer");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.Value = (System.Int32?)((Model.Properties.Integer)this.Binding).Value;
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

                        if (!this.UpdatingBinding)
                        {
                            this.Value = (System.Int32?)((Model.Properties.Integer)this.Binding).Value;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        public Integer(Manager.Session Session)
            : base(Session)
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }

        public Integer(Manager.Session Session, Model.PropertyTypes.Integer PropertyType)
            : base(Session, PropertyType)
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }
    }
}
