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

namespace Aras.ViewModel.Attributes
{
    public enum PropertyTypes { Boolean = 0, Int32 = 1, String = 2, Control = 3, ControlList = 4, NullableInt32 = 5, Float = 6, StringList = 7, Date = 8, Decimal = 9, Command=10 };

    [AttributeUsage(AttributeTargets.Property)]
    public class Property : Attribute
    {
        public String Name { get; private set; }

        public PropertyTypes Type { get; private set; }

        public Boolean ReadOnly { get; private set; }

        public Property(String Name, PropertyTypes Type, Boolean ReadOnly)
        {
            this.Name = Name;
            this.Type = Type;
            this.ReadOnly = ReadOnly;
        }
    }
}
