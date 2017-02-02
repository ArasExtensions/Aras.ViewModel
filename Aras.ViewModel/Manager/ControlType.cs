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
