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
                lock(this._expireLock)
                {
                    return this._expire;
                }
            }
        }

        internal void UpdateExpire()
        {
            lock(this._expireLock)
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

        public ViewModel.Control Plugin(String Name, String Context)
        {
            // Check Licence
            this.Database.Server.CheckLicence();

            // Create Control
            Control plugin = (Control)Activator.CreateInstance(this.Database.Server.ControlType(Name), new object[] { });

            // Set Context
            ((ViewModel.Item)plugin).SetBinding(this.Model, Context);

            // Add Plugin to Cache
            this.AddControlToCache(plugin, false);

            return plugin;
        }

        public ViewModel.Control Application(String Name)
        {
            // Check Licence
            this.Database.Server.CheckLicence();

            // Create Control
            Control application = (Control)Activator.CreateInstance(this.Database.Server.ControlType(Name), new object[] { });

            // Set Binding to Session
            application.Binding = this.Model;

            // Add Application to Cache
            this.AddControlToCache(application, false);

            return application;
        }

        private object ControlCacheLock = new object();
        private Dictionary<Guid, ViewModel.Control> ControlCache;

        private void AddControlToCache(ViewModel.Control Control, Boolean Queue)
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

                        // Add Commands to Cache
                        foreach (String name in Control.Commands)
                        {
                            this.AddCommandToCache(name, Control.GetCommand(name));
                        }
                    }

                    // Add Child Controls to Cache
                    foreach (Control thiscontrol in Control.Controls)
                    {
                        this.AddControlToCache(thiscontrol, true);
                    }

                    if (Queue)
                    {
                        this.AddControlToQueue(Control);
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
            // Add Control to Queue so change is picked up
            ViewModel.Control control = (ViewModel.Control)sender;
            this.AddControlToQueue(control);

            // Check if changed Property is a Control or List of Controls
            if (control.HasProperty(e.PropertyName))
            {
                object property = control.GetPropertyValue(e.PropertyName);

                if (property is Control)
                {
                    this.AddControlToCache((Control)property, true);
                }
                else if (property is IEnumerable<Control>)
                {
                    foreach (Control childcontrol in ((IEnumerable<Control>)property))
                    {
                        this.AddControlToCache(childcontrol, true);
                    }
                }
            }
        }

        private object ControlQueueLock = new object();
        private Queue<ViewModel.Control> ControlQueue;

        private void AddControlToQueue(ViewModel.Control Control)
        {
            lock (this.ControlQueueLock)
            {
                this.ControlQueue.Enqueue(Control);
            }
        }

        public IEnumerable<ViewModel.Control> GetControlsFromQueue()
        {
            lock (this.ControlQueueLock)
            {
                List<ViewModel.Control> ret = new List<ViewModel.Control>();

                while (this.ControlQueue.Count > 0)
                {
                    ViewModel.Control control = this.ControlQueue.Dequeue();

                    if (!ret.Contains(control))
                    {
                        ret.Add(control);
                    }
                }

                return ret;
            }
        }

        private Dictionary<Guid, ViewModel.Command> CommandCache;

        private Dictionary<Guid, String> CommandNameCache;

        private void AddCommandToCache(String Name, ViewModel.Command Command)
        {
            if (!this.CommandCache.ContainsKey(Command.ID))
            {
                // Store Command Name
                this.CommandNameCache[Command.ID] = Name;

                // Store Command in Cache
                this.CommandCache[Command.ID] = Command;

                // Link to Event for when CanExecute Changes
                Command.CanExecuteChanged += Command_CanExecuteChanged;
            }
        }

        void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            ViewModel.Command command = (ViewModel.Command)sender;
            this.AddCommandToQueue(command);
        }

        private object CommandQueueLock = new object();
        private Queue<ViewModel.Command> CommandQueue;

        private void AddCommandToQueue(ViewModel.Command Command)
        {
            lock(this.CommandQueueLock)
            {
                this.CommandQueue.Enqueue(Command);
            }
        }

        public IEnumerable<ViewModel.Command> GetCommandsFromQueue()
        {
            lock (this.CommandQueueLock)
            {
                List<ViewModel.Command> ret = new List<ViewModel.Command>();

                while (this.CommandQueue.Count > 0)
                {
                    ViewModel.Command command = this.CommandQueue.Dequeue();

                    if (!ret.Contains(command))
                    {
                        ret.Add(command);
                    }
                }

                return ret;
            }
        }

        public ViewModel.Command Command(Guid ID)
        {
            return this.CommandCache[ID];
        }

        public String CommandName(Guid ID)
        {
            return this.CommandNameCache[ID];
        }

        internal Session(Database Database, Model.Session Model)
        {
            this.Database = Database;
            this.Model = Model;
            this.ControlCache = new Dictionary<Guid, ViewModel.Control>();
            this.ControlQueue = new Queue<ViewModel.Control>();
            this.CommandCache = new Dictionary<Guid, ViewModel.Command>();
            this.CommandNameCache = new Dictionary<Guid, String>();
            this.CommandQueue = new Queue<ViewModel.Command>();
        }
    }
}