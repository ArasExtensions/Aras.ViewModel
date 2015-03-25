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

namespace Aras.ViewModel
{
    public abstract class Property : Base
    {
        public Control Control { get; private set; }

        public Model.Session Session
        {
            get
            {
                return this.Control.Session;
            }
        }

        public event EventHandler ObjectChanged;

        private void OnObjectChanged()
        {
            if (this.ObjectChanged != null)
            {
                this.ObjectChanged(this, EventArgs.Empty);
            }
        }

        public String Name { get; private set; }

        public Boolean Required { get; private set; }

        public Boolean ReadOnly { get; internal set; }

        private object _object;

        internal virtual void SetObject(object value)
        {
            if (value == null)
            {
                if (this._object != null)
                {
                    this._object = null;
                    this.OnObjectChanged();
                }
            }
            else
            {
                if (!value.Equals(this._object))
                {
                    this._object = value;
                    this.OnObjectChanged();
                }
            }
        }

        public object Object
        {
            get
            {
                return this._object;
            }
            set
            {
                if (!this.ReadOnly)
                {
                    this.SetObject(value);
                }
                else
                {
                    throw new Exceptions.ValueReadOnlyException();
                }
            }
        }

        public override string ToString()
        {
            if (this.Object == null)
            {
                return this.Name + ": null";
            }
            else
            {
                return this.Name + ": " + this.Object.ToString();
            }
        }

        public Property(Control Control, String Name, Boolean Required, Boolean ReadOnly)
           :base()
        {
            this.Control = Control;
            this.Name = Name;
            this.Required = Required;
            this.ReadOnly = ReadOnly; 
        }
    }
}
