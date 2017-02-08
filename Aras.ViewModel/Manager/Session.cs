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

        public String Token
        {
            get
            {
                return this.ID;
            }
        }

        public ViewModel.Control Plugin(ControlType PluginType, String Context)
        {
            // Create Control
            ViewModel.Control plugin = (ViewModel.Control)Activator.CreateInstance(PluginType.Type, new object[1] { this });

            // Set Context
            ((Aras.ViewModel.Item)plugin).SetBinding(this.Model, Context);

            // Refresh Plugin
            plugin.Refresh.Execute();

            // Add Plugin to Queue
            this.AddControlToQueue(plugin, true);

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
            if (!this.ApplicationCache.ContainsKey(ApplicationType))
            {
                // Create Control
                this.ApplicationCache[ApplicationType] = (ViewModel.Containers.Application)Activator.CreateInstance(ApplicationType.Type, new object[1] { this });
                this.ApplicationCache[ApplicationType].Name = ApplicationType.Name;
                this.ApplicationCache[ApplicationType].Label = ApplicationType.Label;
                this.ApplicationCache[ApplicationType].Icon = ApplicationType.Icon;

                // Set Binding to Session
                this.ApplicationCache[ApplicationType].Binding = this.Model;
            }

            // Add Application to Queue
            this.AddControlToQueue(this.ApplicationCache[ApplicationType], true);

            return this.ApplicationCache[ApplicationType];
        }

        private object ControlCacheLock = new object();
        private Dictionary<Guid, ViewModel.Control> ControlCache;

        private Boolean AddControlToCache(ViewModel.Control Control)
        {
            lock (this.ControlCacheLock)
            {
                if (Control != null)
                {
                    if (!this.ControlCache.ContainsKey(Control.ID))
                    {
                        // Add Control to Cache
                        this.ControlCache[Control.ID] = Control;

                        // Link to PropertyChanged Event
                        Control.PropertyChanged += Control_PropertyChanged;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
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
            // Add Control to Queue so change is picked up
            ViewModel.Control control = (ViewModel.Control)sender;
            this.AddControlToQueue(control, false);
        }

        private object ControlQueueLock = new object();
        private List<ViewModel.Control> ControlQueue;

        private void AddControlToQueueRecursive(ViewModel.Control Control, Boolean Force)
        {
            // Check in Control is in Cache, if added to Cache or Force Queue
            if (!this.AddControlToCache(Control) || Force)
            {
                if (!this.ControlQueue.Contains(Control))
                {
                    this.ControlQueue.Add(Control);

                    // Queue Commands
                    foreach(String name in Control.Commands)
                    {
                        this.AddCommandToQueue(name, Control.GetCommand(name), Force);
                    }
                }
            }

            // Process Children
            foreach(Control child in Control.Controls)
            {
                this.AddControlToQueueRecursive(child, Force);
            }
        }

        private void AddControlToQueue(ViewModel.Control Control, Boolean Force)
        {
            lock (this.ControlQueueLock)
            {
                this.AddControlToQueueRecursive(Control, Force);
            }
        }

        public IEnumerable<ViewModel.Control> GetControlsFromQueue()
        {
            lock (this.ControlQueueLock)
            {
                List<ViewModel.Control> ret = this.ControlQueue;
                this.ControlQueue = new List<ViewModel.Control>();
                return ret;
            }
        }

        private object CommandCacheLock = new object();
        private Dictionary<Guid, ViewModel.Command> CommandCache;
        private Dictionary<Guid, String> CommandNameCache;

        private Boolean AddCommandToCache(String Name, ViewModel.Command Command)
        {
            lock (this.CommandCacheLock)
            {
                if (!this.CommandCache.ContainsKey(Command.ID))
                {
                    // Store Command in Cache
                    this.CommandCache[Command.ID] = Command;
                    this.CommandNameCache[Command.ID] = Name;

                    // Link to Event for when CanExecute Changes
                    Command.CanExecuteChanged += Command_CanExecuteChanged;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public String CommandName(Command Command)
        {
            lock (this.CommandCacheLock)
            {
                return this.CommandNameCache[Command.ID];
            }
        }

        void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            ViewModel.Command command = (ViewModel.Command)sender;
            this.AddCommandToQueue(this.CommandName(command), command, true);
        }

        private object CommandQueueLock = new object();
        private List<ViewModel.Command> CommandQueue;

        private void AddCommandToQueue(String Name, ViewModel.Command Command, Boolean Force)
        {
            lock (this.CommandQueueLock)
            {
                if (!this.AddCommandToCache(Name, Command) || Force)
                {
                    if (!this.CommandQueue.Contains(Command))
                    {
                        this.CommandQueue.Add(Command);
                    }
                }
            }
        }

        public IEnumerable<ViewModel.Command> GetCommandsFromQueue()
        {
            lock (this.CommandQueueLock)
            {
                List<ViewModel.Command> ret = this.CommandQueue;
                this.CommandQueue = new List<ViewModel.Command>();
                return ret;
            }
        }

        public ViewModel.Command Command(Guid ID)
        {
            return this.CommandCache[ID];
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
            this.CommandQueue = new List<ViewModel.Command>();
            this.CommandNameCache = new Dictionary<Guid, String>();
        }
    }
}