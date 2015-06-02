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

namespace Aras.ViewModel.Properties
{
    public class Int32 : Property
    {
        [Attributes.Property("MinValue")]
        public System.Int32 MinValue { get; private set; }

        [Attributes.Property("MaxValue")]
        public System.Int32 MaxValue { get; private set; }

        private System.Int32? _value;
        [Attributes.Property("Value")]
        public System.Int32? Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        public Int32(Session Session, Boolean Required, Boolean ReadOnly, System.Int32 MinValue, System.Int32 MaxValue, System.Int32? Default)
            : base(Session, Required, ReadOnly)
        {
            this.MinValue = MinValue;
            this.MaxValue = MaxValue;
            this.Value = Default;
        }

        public Int32(Session Session, Boolean Required, Boolean ReadOnly, System.Int32? Default)
            : this(Session, Required, ReadOnly, System.Int32.MinValue, System.Int32.MaxValue, Default)
        {
        }

        public Int32(Session Session, Boolean Required, Boolean ReadOnly)
            : this(Session, Required, ReadOnly, System.Int32.MinValue, System.Int32.MaxValue, null)
        {
        }
    }
}
