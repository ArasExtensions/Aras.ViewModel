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
using System.Web;

namespace Aras.ViewModel.Manager
{
    public class Session
    {
        public Database Database { get; private set; }

        public Logging.Log Log
        {
            get
            {
                return this.Database.Log;
            }
        }

        public Model.Session Model { get; private set; }

        public String ID
        {
            get
            {
                return this.Model.ID;
            }
        }

        private Object _expireLock = new Object();
        private DateTime _expire;
        public DateTime Expire
        {
            get
            {
                lock (this._expireLock)
                {
                    return this._expire;
                }
            }
        }

        internal void UpdateExpire()
        {
            lock (this._expireLock)
            {
                this._expire = DateTime.UtcNow.AddMinutes(this.Database.Server.ExpireSession);
            }
        }

        public Boolean Expired
        {
            get
            {
                return DateTime.Compare(this.Expire, DateTime.UtcNow) < 0;
            }
        }

        private String _token;
        public String Token
        {
            get
            {
                if (this._token == null)
                {
                    this._token = this.Database.Server.GetToken(this);
                }

                return this._token;
            }
        }

        public ViewModel.Control Plugin(ControlType PluginType, String Context)
        {
            // Create Control
            ViewModel.Control plugin = (ViewModel.Control)Activator.CreateInstance(PluginType.Type, new object[1] { this });

            // Set Context
            ((Aras.ViewModel.Containers.Plugin)plugin).SetBinding(Context);

            // Refresh Plugin
            ((Aras.ViewModel.Containers.Plugin)plugin).Refresh.Execute();

            return plugin;
        }

        public IEnumerable<ControlTypes.ApplicationType> ApplicationTypes
        {
            get
            {
                return this.Database.Server.ApplicationTypes;
            }
        }

        public ControlTypes.ApplicationType ApplicationType(String Name)
        {
            return this.Database.Server.ApplicationType(Name);
        }

        private Dictionary<ControlTypes.ApplicationType, ViewModel.Containers.Application> ApplicationCache;
        public ViewModel.Containers.Application Application(ControlTypes.ApplicationType ApplicationType)
        {
            // Create Control
            this.ApplicationCache[ApplicationType] = (ViewModel.Containers.Application)Activator.CreateInstance(ApplicationType.Type, new object[1] { this });
            this.ApplicationCache[ApplicationType].Name = ApplicationType.Name;
            this.ApplicationCache[ApplicationType].Label = ApplicationType.Label;
            this.ApplicationCache[ApplicationType].Icon = ApplicationType.Icon;

            // Set Binding to Session
            this.ApplicationCache[ApplicationType].Binding = this.Model;

            return this.ApplicationCache[ApplicationType];
        }

        internal void QueueControlRecursive(Control Control)
        {
            // Queue Control
            this.QueueControl(Control);

            // Queue Commands
            foreach (String name in Control.Commands)
            {
                this.QueueCommand(Control.GetCommand(name));
            }

            // Queue Children
            foreach(Control child in Control.Controls)
            {
                this.QueueControlRecursive(child);
            }
        }

        private object ControlCacheLock = new object();
        private volatile Dictionary<Guid, ViewModel.Control> ControlCache;

        internal void CacheControl(ViewModel.Control Control)
        {
            lock (this.ControlCacheLock)
            {
                if (Control != null)
                {
                    if (!this.ControlCache.ContainsKey(Control.ID))
                    {
                        // Add Control to Cache
                        this.ControlCache[Control.ID] = Control;

                        // Queue Control
                        this.QueueControl(Control);

                        // Link to PropertyChanged Event
                        Control.PropertyChanged += Control_PropertyChanged;
                    }
                }
            }
        }

        public ViewModel.Control Control(Guid ID)
        {
            lock (this.ControlCacheLock)
            {
                return this.ControlCache[ID];
            }
        }

        void Control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Add Control to Queue when Property Changes
            ViewModel.Control control = (ViewModel.Control)sender;

            if (control.HasProperty(e.PropertyName))
            {
                this.QueueControl(control);
            }
        }

        private object ControlQueueLock = new object();
        private volatile List<ViewModel.Control> ControlQueue;

        internal void QueueControl(ViewModel.Control Control)
        {
            if (!this.ControlQueue.Contains(Control))
            {
                this.ControlQueue.Add(Control);
            }
        }

        public IEnumerable<ViewModel.Control> GetControlsFromQueue()
        {
            lock (this.ControlQueueLock)
            {
                // Copy Queue
                List<ViewModel.Control> ret = this.ControlQueue;
                
                // Reset Queue
                this.ControlQueue = new List<ViewModel.Control>();

                // Reverse
                ret.Reverse();

                return ret;
            }
        }

        private object CommandCacheLock = new object();
        private volatile Dictionary<Guid, ViewModel.Command> CommandCache;

        internal void CacheCommand(ViewModel.Command Command)
        {
            lock (this.CommandCacheLock)
            {
                if (!this.CommandCache.ContainsKey(Command.ID))
                {
                    // Store Command in Cache
                    this.CommandCache[Command.ID] = Command;

                    // Queue Command
                    this.QueueCommand(Command);

                    // Link to Event for when CanExecute Changes
                    Command.CanExecuteChanged += Command_CanExecuteChanged;
                }
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            ViewModel.Command command = (ViewModel.Command)sender;
            this.QueueCommand(command);
        }

        public ViewModel.Command Command(Guid ID)
        {
            return this.CommandCache[ID];
        }

        private object CommandQueueLock = new object();
        private volatile List<ViewModel.Command> CommandQueue;

        private void QueueCommand(ViewModel.Command Command)
        {
            if (!this.CommandQueue.Contains(Command))
            {
                this.CommandQueue.Add(Command);
            }
        }

        public IEnumerable<ViewModel.Command> GetCommandsFromQueue()
        {
            lock (this.CommandQueueLock)
            {
                // Copy Queue
                List<ViewModel.Command> ret = this.CommandQueue;

                // Reset Queue
                this.CommandQueue = new List<ViewModel.Command>();

                return ret;
            }
        }

        public ViewModel.Property CreateProperty(Model.Property Property, System.Boolean SubstitueStringForText)
        {
            ViewModel.Property viewmodelproperty = this.CreateProperty(Property.Type, SubstitueStringForText);
            viewmodelproperty.Binding = Property;
            return viewmodelproperty;
        }

        public ViewModel.Property CreateProperty(Model.PropertyType PropertyType, System.Boolean SubstitueStringForText)
        {
            ViewModel.Property viewmodelproperty = null;

            switch (PropertyType.GetType().Name)
            {
                case "String":
                    viewmodelproperty = new Properties.String(this, (Model.PropertyTypes.String)PropertyType);
                    break;
                case "Federated":
                    viewmodelproperty = new Properties.Federated(this, (Model.PropertyTypes.Federated)PropertyType);
                    break;
                case "Integer":
                    viewmodelproperty = new Properties.Integer(this, (Model.PropertyTypes.Integer)PropertyType);
                    break;
                case "Float":
                    viewmodelproperty = new Properties.Float(this, (Model.PropertyTypes.Float)PropertyType);
                    break;
                case "Sequence":
                    viewmodelproperty = new Properties.Sequence(this, (Model.PropertyTypes.Sequence)PropertyType);
                    break;
                case "Item":
                    viewmodelproperty = new Properties.Item(this, (Model.PropertyTypes.Item)PropertyType);
                    break;
                case "Decimal":
                    viewmodelproperty = new Properties.Decimal(this, (Model.PropertyTypes.Decimal)PropertyType);
                    break;
                case "Date":
                    viewmodelproperty = new Properties.Date(this, (Model.PropertyTypes.Date)PropertyType);
                    break;
                case "Text":

                    if (SubstitueStringForText)
                    {
                        viewmodelproperty = new Properties.String(this, (Model.PropertyTypes.Text)PropertyType);
                    }
                    else
                    {
                        viewmodelproperty = new Properties.Text(this, (Model.PropertyTypes.Text)PropertyType);
                    }

                    break;
                case "Boolean":
                    viewmodelproperty = new Properties.Boolean(this, (Model.PropertyTypes.Boolean)PropertyType);
                    break;
                case "List":
                    viewmodelproperty = new Properties.List(this, (Model.PropertyTypes.List)PropertyType);
                    break;
                default:
                    throw new Model.Exceptions.ArgumentException("PropertyType not implmented: " + PropertyType.GetType().Name);
            }

            return viewmodelproperty;
        }

        public override string ToString()
        {
            return this.Model.ToString();
        }

        internal Session(Database Database, Model.Session Model)
        {
            this.Database = Database;
            this.Model = Model;
            this.ApplicationCache = new Dictionary<ControlTypes.ApplicationType, Containers.Application>();
            this.ControlCache = new Dictionary<Guid, ViewModel.Control>();
            this.ControlQueue = new List<ViewModel.Control>();
            this.CommandCache = new Dictionary<Guid, ViewModel.Command>();
            this.CommandQueue = new List<Command>();
        }
    }
}