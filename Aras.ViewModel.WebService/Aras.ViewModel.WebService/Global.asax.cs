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
using System.Web.Http;
using System.Web.Routing;
using System.Web.Http.Dispatcher;
using System.IO;

namespace Aras.ViewModel.WebService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private ViewModel.Manager.Server ViewModel;

        protected void Application_Start()
        {
            // Create Log
            Logging.Events log = new Aras.Logging.Events("Aras Web Service");
            log.Level = (Aras.Logging.Log.Levels)Properties.Settings.Default.LogLevel;

            // Create Mananger
            String url = Properties.Settings.Default.URL;

            if (String.IsNullOrEmpty(url))
            {
                // Assume installed one directory above Aras Server
                String webserviceurl = "http://localhost" + System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                int lastpos = webserviceurl.LastIndexOf('/');
                url = webserviceurl.Substring(0, lastpos);
            }

            this.ViewModel = new ViewModel.Manager.Server(url , log);

            // Set Assembly Directory
            this.ViewModel.AssemblyDirectory = new DirectoryInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\bin");

            // Load Assemblies
            this.ViewModel.LoadAssembly("Aras.Model.Design");
            this.ViewModel.LoadAssembly("Aras.ViewModel.Design");

            // Create Activator
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new ControllerActivator(this.ViewModel));

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
