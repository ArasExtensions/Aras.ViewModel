/*  
  Copyright 2017 Processwall Limited

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Web:     http://www.processwall.com
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
