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

namespace Aras.ViewModel.Fields
{
    public class String : Field
    {
        private const System.Int32 MinLength = 1;
        private const System.Int32 MaxLength = System.Int32.MaxValue;
        private const System.Int32 DefaultLength = 32;

        private System.Int32 _length;
        [Attributes.Property("Length")]
        public System.Int32 Length
        {
            get
            {
                return this._length;
            }
            set
            {
                if (!this._length.Equals(value))
                {
                    if (value >= MinLength && value <= MaxLength)
                    {
                        this._length = value;
                        this.OnPropertyChanged("Length");
                    }
                    else
                    {
                        throw new ArgumentException("Length must be between " + MinLength.ToString() + " and " + MaxLength.ToString());
                    }
                }
            }
        }

        private System.String _value;
        [Attributes.Property("Value")]
        public System.String Value
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
                        if (value.Length <= this.Length)
                        {
                            this._value = value;
                            this.OnPropertyChanged("Value");
                        }
                        else
                        {
                            throw new ArgumentException("Length must be no greater than " + this.Length.ToString());
                        }
                    }
                }
                else
                {
                    if (!this._value.Equals(value))
                    {
                        if (value == null || value.Length <= this.Length)
                        {
                            this._value = value;
                            this.OnPropertyChanged("Value");
                        }
                        else
                        {
                            throw new ArgumentException("Length must be no greater than " + this.Length.ToString());
                        }
                    }
                }
            }
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (e.PropertyName == "Binding")
            {
                if (this.Binding != null && this.Binding is Model.Properties.String)
                {
                    Model.Properties.String prop = (Model.Properties.String)this.Binding;
                    Model.PropertyTypes.String proptype = (Model.PropertyTypes.String)prop.PropertyType;
                    this.Length = proptype.Length;
                    this.Value = prop.Value;
                    this.Binding.PropertyChanged += Binding_PropertyChanged;
                }
            }
        }

        void Binding_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Model.Properties.String prop = (Model.Properties.String)this.Binding;
                this.Value = prop.Value;
            }
        }

        public String(Session Session, Boolean Required, Boolean ReadOnly, System.Int32 Length, System.String Default)
            : base(Session, Required, ReadOnly)
        {
            this.Length = Length;
            this.Value = Default;
        }

        public String(Session Session, Boolean Required, Boolean ReadOnly, System.Int32 Length)
            : this(Session, Required, ReadOnly, Length, null)
        {
        }

        public String(Session Session, Boolean Required, Boolean ReadOnly)
            : this(Session, Required, ReadOnly, DefaultLength, null)
        {
        }

    }
}
