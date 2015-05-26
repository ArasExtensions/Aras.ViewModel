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
        public System.Int32 MinValue 
        { 
            get
            {
                return (System.Int32)this.PropertiesCache["MinValue"];
            }
            set
            {
                if (!((System.Int32)this.PropertiesCache["MinValue"]).Equals(value))
                {
                    this.PropertiesCache["MinValue"] = value;
                    this.OnPropertyChanged("MinValue");
                }
            }
        }

        public System.Int32 MaxValue
        {
            get
            {
                return (System.Int32)this.PropertiesCache["MaxValue"];
            }
            set
            {
                if (!((System.Int32)this.PropertiesCache["MaxValue"]).Equals(value))
                {
                    this.PropertiesCache["MaxValue"] = value;
                    this.OnPropertyChanged("MaxValue");
                }
            }
        }

        public System.Int32? Value
        {
            get
            {
                return (System.Int32?)this.PropertiesCache["Value"];
            }
            set
            {
                if (this.PropertiesCache["Value"] == null)
                {
                    if (value != null)
                    {
                        this.PropertiesCache["Value"] = value;
                        this.OnPropertyChanged("Value");
                    }
                }
                else
                {
                    if (!((System.Int32)this.PropertiesCache["Value"]).Equals(value))
                    {
                        this.PropertiesCache["Value"] = value;
                        this.OnPropertyChanged("Value");
                    }
                }
            }
        }

        public Int32(Session Session, Boolean Required, Boolean ReadOnly, System.Int32 MinValue, System.Int32 MaxValue, System.Int32? Default)
            : base(Session, Required, ReadOnly)
        {
            this.PropertiesCache["MinValue"] = MinValue;
            this.PropertiesCache["MaxValue"] = MaxValue;
            this.PropertiesCache["Value"] = Default;
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
