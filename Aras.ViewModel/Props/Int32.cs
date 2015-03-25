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
        internal override void SetObject(object value)
        {
            if (value == null)
            {
                base.SetObject(value);
            }
            else if (value is System.Int32)
            {
                if ((System.Int32)value >= this.MinValue && (System.Int32)value <= this.MaxValue)
                {
                    base.SetObject(value);
                }
                else
                {
                    throw new Exceptions.ValueRangeException();
                }
            }
            else
            {
                throw new Exceptions.ValueTypeException("System.Int32");
            }
        }

        public System.Int32? Value
        {
            get
            {
                return (System.Int32)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        public System.Int32 MinValue { get; private set; }

        public System.Int32 MaxValue { get; private set; }

        public Int32(ViewModel.Control Control, System.String Name, Boolean Required, Boolean ReadOnly, System.Int32 MinValue, System.Int32 MaxValue, System.Int32? Default)
            : base(Control, Name, Required, ReadOnly)
        {
            this.MinValue = MinValue;
            this.MaxValue = MaxValue;
            this.SetObject(Default);
        }
    }
}
