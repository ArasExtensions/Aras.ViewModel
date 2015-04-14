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

namespace Aras.ViewModel
{
    public class Session
    {
        public Manager Manager { get; private set; }

        private Model.Session ModelSession { get; set; }

        public Guid ID
        {
            get
            {
                return this.ModelSession.ID;
            }
        }

        public String Token
        {
            get
            {
                return Utilities.GuidToString(this.ID);
            }
        }

        private Dictionary<Guid, ViewModel.Application> ApplicationCache;

        public IEnumerable<ViewModel.Application> Applications
        {
            get
            {
                if (this.ApplicationCache == null)
                {
                    this.ApplicationCache = new Dictionary<Guid, ViewModel.Application>();

                    foreach(Type applicationtype in this.Manager.ApplicationTypes)
                    {
                        Application application = (Application)Activator.CreateInstance(applicationtype, new object[] { this.ModelSession });
                        this.ApplicationCache[application.ID] = application;
                        this.AddControlToCache(application);
                    }
                }

                return this.ApplicationCache.Values;
            }
        }

        private Dictionary<Guid, ViewModel.Control> ControlCache;
        private void AddControlToCache(ViewModel.Control Control)
        {
            if (!this.ControlCache.ContainsKey(Control.ID))
            {
                this.ControlCache[Control.ID] = Control;

                foreach (ViewModel.Property property in Control.Properties)
                {
                    if (property is ViewModel.Properties.Control)
                    {
                        ViewModel.Control childcontrol = ((ViewModel.Properties.Control)property).Value;
                        this.AddControlToCache(childcontrol);
                    }
                    else if (property is ViewModel.Properties.ControlList)
                    {
                        foreach(ViewModel.Control childcontrol in ((ViewModel.Properties.ControlList)property).Value)
                        {
                            this.AddControlToCache(childcontrol);
                        }
                    }
                }

                foreach(ViewModel.Command command in Control.Commands)
                {
                    this.AddCommandToCache(command);
                }
            }
        }

        public ViewModel.Control Control(Guid ID)
        {
            return this.ControlCache[ID];
        }

        private Dictionary<Guid, ViewModel.Command> CommandCache;

        private void AddCommandToCache(ViewModel.Command Command)
        {
            if (!this.CommandCache.ContainsKey(Command.ID))
            {
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
            List<ViewModel.Command> ret = new List<ViewModel.Command>();

            lock(this.CommandQueueLock)
            {
                while(this.CommandQueue.Count > 0)
                {
                    ret.Add(this.CommandQueue.Dequeue());
                }
            }

            return ret;
        }

        public ViewModel.Command Command(Guid ID)
        {
            return this.CommandCache[ID];
        }

        internal Session(Manager Manager, Model.Session ModelSession)
        {
            this.Manager = Manager;
            this.ModelSession = ModelSession;
            this.ControlCache = new Dictionary<Guid, ViewModel.Control>();
            this.CommandCache = new Dictionary<Guid, ViewModel.Command>();
            this.CommandQueue = new Queue<ViewModel.Command>();
        }
    }
}