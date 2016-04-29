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
        private const String ApplicationID = "AAD";
        private const double MinExpireSession = 1;
        private const double MaxExpireSession = 60;
        private const double DefaultExpireSession = 10;

        public Logging.Log Log { get; private set; }

        public Licence.IManager Licence { get; private set; }

        public Model.Server Model { get; private set; }

        internal void CheckLicence()
        {
            switch (this.Licence.Check(ApplicationID))
            {
                case Aras.Licence.LicenceStates.CorruptLicenceFile:
                    throw new Exceptions.LicenceException("Licence file is corrupted");
                case Aras.Licence.LicenceStates.Expired:
                    throw new Exceptions.LicenceException("Licence for " + ApplicationID + " has expired");
                case Aras.Licence.LicenceStates.InvalidApplicationID:
                    throw new Exceptions.LicenceException("No licence for ApplicationID: " + ApplicationID);
                case Aras.Licence.LicenceStates.InvalidHostID:
                    throw new Exceptions.LicenceException("Licence not available for this Server");
                case Aras.Licence.LicenceStates.MissingLicenceFile:
                    throw new Exceptions.LicenceException("Licence file not found");
                default:
                    break;
                throw new Exceptions.LicenceException(ApplicationID);
            }
        }

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

        public Database Database(String Name)
        {
            foreach (Database database in this.Databases)
            {
                if (database.Name.Equals(Name))
                {
                    return database;
                }
            }

            throw new Model.Exceptions.ArgumentException("Invalid Database Name: " + Name);
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
                this.ControlTypeCache = new Dictionary<String, Type>();

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
                // Load Controls
                Assembly assembly = Assembly.LoadFrom(AssemblyFile.FullName);

                // Find all Controls

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Control)) && !type.IsAbstract)
                    {
                        this.ControlTypeCache[type.FullName] = type;
                    }
                }
            }
        }

        private Dictionary<String, Type> ControlTypeCache;

        internal Type ControlType(String Name)
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

        public Server(String URL, Licence.IManager Licence, Logging.Log Log)
        {
            // Store Log
            this.Log = Log;

            // Store Licence
            this.Licence = Licence;

            // Set Default Session Expire
            this.ExpireSession = DefaultExpireSession;

            // Initialiase Session Cache
            this.SessionCache = new Dictionary<String, Session>();

            // Create Model Server
            this.Model = new Model.Server(URL);
            
            // Default Assembly Directory
            this.AssemblyDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        }

        public Server(String URL, Logging.Log Log)
            :this(URL, new DefaultLicence(), Log)
        {

        }
    }
}
