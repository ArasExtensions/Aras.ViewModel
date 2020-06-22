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
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aras.ViewModel.WebService.Controllers
{

    public class ApplicationsController : BaseController
    {
        private Models.ApplicationType Child(Models.ApplicationType Parent, String Path)
        {
            if (Path != null)
            {
                String thispath = null;
                String remainingpath = null;
                Int32 seppos = Path.IndexOf('/');

                if (seppos > 0)
                {
                    thispath = Path.Substring(0, seppos);
                    remainingpath = Path.Substring(seppos + 1, Path.Length - seppos - 1);
                }
                else
                {
                    thispath = Path;
                    remainingpath = null;
                }

                Models.ApplicationType thischild = null;

                foreach(Models.ApplicationType child in Parent.Children)
                {
                   if (child.Label.Equals(thispath))
                   {
                       thischild = child;
                       break;
                   }
                }

                if (thischild == null)
                {
                    thischild = new Models.ApplicationType();
                    thischild.ID = Guid.NewGuid().ToString();
                    thischild.Name = null;
                    thischild.Label = thispath;
                    thischild.Start = false;
                    thischild.Children = new List<Models.ApplicationType>();
                    Parent.Children.Add(thischild);
                }

                if (remainingpath == null)
                {
                    return thischild;
                }
                else
                {
                    return this.Child(thischild, remainingpath);
                }
            }
            else
            {
                return Parent;
            }
        }

        [Route("applicationtypes")]
        [HttpGet]
        public Models.ApplicationType GetAllApplicationTypes()
        {
            try
            {
                this.Server.Log.Add(Logging.Levels.Debug, "Starting Get applicationtypes");

                // Create Root
                Models.ApplicationType root = new Models.ApplicationType();
                root.ID = Guid.NewGuid().ToString();
                root.Name = null;
                root.Label = "Root";
                root.Start = false;
                root.Children = new List<Models.ApplicationType>();

                foreach (ViewModel.Manager.ControlTypes.ApplicationType apptype in this.Session.ApplicationTypes)
                {
                    Models.ApplicationType parent = this.Child(root, apptype.Path);
                    Models.ApplicationType modelapptype = new Models.ApplicationType();
                    modelapptype.ID = Guid.NewGuid().ToString();
                    modelapptype.Name = apptype.Name;
                    modelapptype.Label = apptype.Label;
                    modelapptype.Icon = apptype.Icon;
                    modelapptype.Start = apptype.Start;
                    parent.Children.Add(modelapptype);
                }

                return root;
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
            finally
            {
                this.Server.Log.Add(Logging.Levels.Debug, "Completed Get applicationtypes");
            }
        }

        [Route("applications")]
        [HttpPost]
        public Models.Responses.Control GetApplication(Models.ApplicationType ApplicationType)
        {
            try
            {
                this.Server.Log.Add(Logging.Levels.Debug, "Starting Put applications");

                // Get Application Type
                Manager.ControlTypes.ApplicationType apptype = this.Session.ApplicationType(ApplicationType.Name);
                
                // Get Application Control
                ViewModel.Containers.Application applicationcontrol = this.Session.Application(apptype);

                return new Models.Responses.Control(this.Session, applicationcontrol);
            }
            catch(Exception e)
            {
                throw this.ProcessException(e);
            }
            finally
            {
                this.Server.Log.Add(Logging.Levels.Debug, "Completed Put applications");
            }
        }

        public ApplicationsController(ViewModel.Manager.Server Server)
            : base(Server)
        {

        }
    }
}