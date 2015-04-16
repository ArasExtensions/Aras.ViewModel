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
using System.Text;
using System.IO;
using System.Reflection;

namespace Aras.ViewModel
{
    public class Manager
    {
        private const double MinExpireSession = 1;
        private const double MaxExpireSession = 60;
        private const double DefaultExpireSession = 10;

        public String URL { get; private set; }

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

        private Model.Server _server;
        public Model.Server Server
        {
            get
            {
                if (this._server == null)
                {
                    this._server = new Model.Server(this.URL);
                }

                return this._server;
            }
        }

        private List<Type> ApplicationTypesCache;

        internal IEnumerable<Type> ApplicationTypes
        {
            get
            {
                return this.ApplicationTypesCache;
            }
        }

        public DirectoryInfo AssemblyDirectory { get; set; }
  
        public void LoadAssembly(String Name)
        {
            Assembly assembly = Assembly.LoadFrom(this.AssemblyDirectory.FullName + "\\" + Name + ".dll");

            // Find all Application Types

            foreach(Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Application)))
                {
                    this.ApplicationTypesCache.Add(type);
                }
            }
        }

        public Model.Database Database(String Name)
        {
            return this.Server.Database(Name);
        }

        private Object _sessionCacheLock = new Object();
        private volatile Dictionary<Guid, Session> _sessionCache;

        private void AddSessionToCache(Session Session)
        {
            lock (this._sessionCacheLock)
            {
                this._sessionCache[Session.ID] = Session;
            }
        }

        private Session GetSessionFromCache(Guid ID)
        {
            lock (this._sessionCacheLock)
            {
                return this._sessionCache[ID];
            }
        }

        public Session Login(Model.Database Database, String Username, String Password)
        {
            Model.Session modelsession = Database.Login(Username, Password);
            Session session = new Session(this, modelsession);
            this.AddSessionToCache(session);
            return session;
        }

        public Session Session(String Token)
        {
            return this.GetSessionFromCache(Utilities.StringToGuid(Token));
        }

        public Manager(String URL)
        {
            this.ExpireSession = DefaultExpireSession;

            this.URL = URL;
            this._sessionCache = new Dictionary<Guid, Session>();
            this.ApplicationTypesCache = new List<Type>();

            // Set Default Assembly Directory
            this.AssemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }
    }
}