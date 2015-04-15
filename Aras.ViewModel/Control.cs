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
    public abstract class Control : Base
    {
        public Aras.Model.Session Session { get; private set; }

        private Dictionary<String, Property> _properties;

        public IEnumerable<Property> Properties
        {
            get
            {
                return this._properties.Values;
            }
        }

        public Boolean HasProperty(String Name)
        {
            return this._properties.ContainsKey(Name);
        }

        public Property Property(String Name)
        {
            if (this._properties.ContainsKey(Name))
            {
                return this._properties[Name];
            }
            else
            {
                throw new Exceptions.PropertyNameException(Name);
            }
        }

        protected void RegisterProperty(Property Property)
        {
            this._properties[Property.Name] = Property;
        }

        private Dictionary<String, Command> _commands;

        public IEnumerable<Command> Commands
        {
            get
            {
                return this._commands.Values;
            }
        }

        public Command Command(String Name)
        {
            if (this._commands.ContainsKey(Name))
            {
                return this._commands[Name];
            }
            else
            {
                throw new Exceptions.CommandNameException(Name);
            }
        }

        protected void RegisterCommand(Command Command)
        {
            if (!this._commands.ContainsKey(Command.Name))
            {
                this._commands[Command.Name] = Command;
            }
            else
            {
                throw new Exceptions.DuplicateCommandNameException(Command.Name);
            }
        }

        protected void SetPropertyObject(String Name, object Value)
        {
            this.Property(Name).SetObject(Value);
        }

        protected Property CreateProperty(String Name, Boolean Required, Boolean ReadOnly, Model.Cache.Property Property)
        {
            Property ret = null;
            Property currentproperty = null;
            String currenttype = null;

            if (this.HasProperty(Name))
            {
                currentproperty = this.Property(Name);
                currenttype = currentproperty.GetType().Name;
            }

            switch(Property.GetType().Name)
            {
                case "String":

                    if (currentproperty == null || currenttype != "String")
                    {
                        ret = new Properties.String(this, Name, Required, ReadOnly, null);
                    }
                    else
                    {
                        ret = this.Property(Name);
                    }

                    break;
                default:
                    throw new NotImplementedException("Property type not supported: " + Property.GetType().Name);
            }

            ret.Binding = Property;
            this.RegisterProperty(ret);
            return ret;
        }

        public Control(Aras.Model.Session Session)
            :base()
        {
            this.Session = Session;
            this._properties = new Dictionary<String, Property>();
            this._commands = new Dictionary<String, Command>();
        }
    }
}
