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
            log.Level = (Aras.Logging.Levels)Properties.Settings.Default.LogLevel;

            // Create Mananger
            String url = Properties.Settings.Default.URL;

            if (String.IsNullOrEmpty(url))
            {
                // Assume installed one directory above Aras Server
                String webserviceurl = "http://localhost" + System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                int lastpos = webserviceurl.LastIndexOf('/');
                url = webserviceurl.Substring(0, lastpos);
            }

            // Create ViewModel
            this.ViewModel = new ViewModel.Manager.Server(url, log);

            // Set Assembly Directory
            this.ViewModel.AssemblyDirectory = new DirectoryInfo(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\bin");
            
            // Load Assemblies
            this.ViewModel.LoadAssembly("CMB.Model");
            this.ViewModel.LoadAssembly("CMB.ViewModel");

            // Create Activator
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new ControllerActivator(this.ViewModel));

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
