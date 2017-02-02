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
using System.IO;
using System.Reflection;

namespace Aras.ViewModel.Manager
{
    public class Server
    {
        private const double MinExpireSession = 1;
        private const double MaxExpireSession = 60;
        private const double DefaultExpireSession = 10;

        public Logging.Log Log { get; private set; }

        public Model.Server Model { get; private set; }

        private List<Database> _databases;
        public IEnumerable<Database> Databases
        {
            get
            {
                if (this._databases == null)
                {
                    this._databases = new List<Database>();

                    foreach (Model.Database modeldatabase in this.Model.Databases)
                    {
                        this._databases.Add(new Database(this, modeldatabase));
                    }
                }

                return this._databases;
            }
        }

        public Database Database(String ID)
        {
            foreach (Database database in this.Databases)
            {
                if (database.ID.Equals(ID))
                {
                    return database;
                }
            }

            throw new Model.Exceptions.ArgumentException("Invalid Database Name: " + ID);
        }

        private double _expireSession;
        public double ExpireSession
        {
            get
            {
                return this._expireSession;
            }
            set
            {
                if (value < MinExpireSession)
                {
                    this._expireSession = MinExpireSession;
                }
                else if (value > MaxExpireSession)
                {
                    this._expireSession = MaxExpireSession;
                }
                else
                {
                    this._expireSession = value;
                }
            }
        }

        public DirectoryInfo AssemblyDirectory
        {
            get
            {
                return this.Model.AssemblyDirectory;
            }
            set
            {
                this.Model.AssemblyDirectory = value;

                // Reset ControlCache
                this.ControlTypeCache = new Dictionary<String, ControlType>();

                // Load Base Control Library
                this.LoadAssembly("Aras.ViewModel");
            }
        }

        public void LoadAssembly(String AssemblyFile)
        {
            // Load Assembly into Model
            this.Model.LoadAssembly(AssemblyFile);

            // Load Controls
            this.LoadAssembly(new FileInfo(this.AssemblyDirectory.FullName + "\\" + AssemblyFile + ".dll"));
        }

        private void LoadAssembly(FileInfo AssemblyFile)
        {
            if (AssemblyFile.Exists)
            {
                // Load Assembly
                Assembly assembly = Assembly.LoadFrom(AssemblyFile.FullName);

                // Cache all Controls
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Control)) && !type.IsAbstract)
                    {
                        if (!this.ControlTypeCache.ContainsKey(type.FullName))
                        {
                            if (type.IsSubclassOf(typeof(Containers.Application)))
                            {
                                this.ApplicationTypeCache[type.FullName] = new ControlTypes.ApplicationType(type);
                                this.ControlTypeCache[type.FullName] = this.ApplicationTypeCache[type.FullName];
                            }
                            else if (type.IsSubclassOf(typeof(Containers.Plugin)))
                            {
                                this.PluginTypeCache[type.FullName] = new ControlTypes.PluginType(type);
                                this.ControlTypeCache[type.FullName] = this.PluginTypeCache[type.FullName];
                            }
                            else
                            {
                                this.ControlTypeCache[type.FullName] = new ControlType(type);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<String, ControlType> ControlTypeCache;
        private Dictionary<String, ControlTypes.PluginType> PluginTypeCache;
        private Dictionary<String, ControlTypes.ApplicationType> ApplicationTypeCache;

        public IEnumerable<ControlType> ControlTypes
        {
            get
            {
                return this.ControlTypeCache.Values;
            }
        }

        public ControlType ControlType(String Name)
        {
            if (this.ControlTypeCache.ContainsKey(Name))
            {
                return this.ControlTypeCache[Name];
            }
            else
            {
                throw new Model.Exceptions.ArgumentException("Invalid Control Type: " + Name);
            }
        }

        public ControlType ControlType(Control Control)
        {
            String name = Control.GetType().FullName;

            if (this.ControlTypeCache.ContainsKey(name))
            {
                return this.ControlTypeCache[name];
            }
            else
            {
                throw new Model.Exceptions.ArgumentException("Invalid Control Type: " + name);
            }
        }

        internal IEnumerable<ControlTypes.ApplicationType> ApplicationTypes
        {
            get
            {
                return this.ApplicationTypeCache.Values;
            }
        }

        internal ControlTypes.ApplicationType ApplicationType(String Name)
        {
            if (this.ApplicationTypeCache.ContainsKey(Name))
            {
                return this.ApplicationTypeCache[Name];
            }
            else
            {
                throw new Model.Exceptions.ArgumentException("Invalid Application Type: " + Name);
            }
        }

        private Object _sessionCacheLock = new Object();
        private Dictionary<String, Session> SessionCache;

        internal void AddSessionToCache(Session Session)
        {
            lock (this._sessionCacheLock)
            {
                this.SessionCache[Session.ID] = Session;
            }
        }

        internal Boolean SessionInCache(String ID)
        {
            return this.SessionCache.ContainsKey(ID);
        }

        internal Session GetSessionFromCache(String ID)
        {
            lock (this._sessionCacheLock)
            {
                if (this.SessionCache.ContainsKey(ID))
                {
                    return this.SessionCache[ID];
                }
                else
                {
                    throw new Exceptions.SessionException();
                }
            }
        }

        public Session Session(String Token)
        {
            return this.GetSessionFromCache(Token);
        }

        public override string ToString()
        {
            return this.Model.ToString();
        }

        public Server(String URL, Logging.Log Log)
        {
            // Initialise Caches
            this.ControlTypeCache = new Dictionary<String, ControlType>();
            this.PluginTypeCache = new Dictionary<String, ControlTypes.PluginType>();
            this.ApplicationTypeCache = new Dictionary<String, ControlTypes.ApplicationType>();

            // Store Log
            this.Log = Log;

            // Create Model Server
            this.Model = new Model.Server(URL);

            // Set Default Session Expire
            this.ExpireSession = DefaultExpireSession;

            // Initialiase Session Cache
            this.SessionCache = new Dictionary<String, Session>();

            // Set Default Assembly Directory
            this.AssemblyDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        }
    }
}
