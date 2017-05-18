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
using System.Reflection;

namespace Aras.ViewModel.Manager
{
    public class ControlType : IEquatable<ControlType>
    {
        public Type Type { get; private set; }

        public String Name
        {
            get
            {
                return this.Type.FullName;
            }
        }

        private String ClientControl(Type ControlType)
        {
            // Get Atttribute
            ViewModel.Attributes.ClientControl clientcontrolatt = (ViewModel.Attributes.ClientControl)ControlType.GetCustomAttribute(typeof(ViewModel.Attributes.ClientControl));

            if (clientcontrolatt != null)
            {
                return clientcontrolatt.Name;
            }
            else
            {
                Type ControlBaseType = ControlType.BaseType;

                if (ControlBaseType != null)
                {
                    return this.ClientControl(ControlBaseType);
                }
                else
                {
                    return null;
                }
            }
        }

        private String _clientType;
        public String ClientType
        {
            get
            {
                if (this._clientType == null)
                {
                    this._clientType = this.ClientControl(this.Type);
                }

                return this._clientType;
            }
        }

        public bool Equals(ControlType other)
        {
            if (other != null)
            {
                return this.Name.Equals(other.Name);
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ControlType)
            {
                return this.Equals((ControlType)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal ControlType(Type Type)
        {
            this.Type = Type;
        }
    }
}
