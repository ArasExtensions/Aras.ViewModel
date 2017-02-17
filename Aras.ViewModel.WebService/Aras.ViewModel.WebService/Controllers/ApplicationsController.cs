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
                // Create Root
                Models.ApplicationType root = new Models.ApplicationType();
                root.ID = Guid.NewGuid().ToString();
                root.Name = null;
                root.Label = "Root";
                root.Children = new List<Models.ApplicationType>();

                foreach (ViewModel.Manager.ControlTypes.ApplicationType apptype in this.Session.ApplicationTypes)
                {
                    Models.ApplicationType parent = this.Child(root, apptype.Path);
                    Models.ApplicationType modelapptype = new Models.ApplicationType();
                    modelapptype.ID = Guid.NewGuid().ToString();
                    modelapptype.Name = apptype.Name;
                    modelapptype.Label = apptype.Label;
                    modelapptype.Icon = apptype.Icon;
                    parent.Children.Add(modelapptype);
                }

                return root;
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        [Route("applications")]
        [HttpPut]
        public Models.Responses.Control GetApplication(Models.ApplicationType ApplicationType)
        {
            try
            {
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
        }

        public ApplicationsController(ViewModel.Manager.Server Server)
            : base(Server)
        {

        }
    }
}