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
        [Route("applicationtypes")]
        [HttpGet]
        public IEnumerable<Models.ApplicationType> GetAllApplicationTypes()
        {
            try
            {
                List<Models.ApplicationType> ret = new List<Models.ApplicationType>();

                foreach (ViewModel.Manager.ApplicationType apptype in this.Session.Database.Server.ApplicationTypes)
                {
                    Models.ApplicationType modelapptype = new Models.ApplicationType();
                    modelapptype.Name = apptype.Name;
                    modelapptype.Label = apptype.Name;
                    ret.Add(modelapptype);
                }

                return ret;
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
                Models.Responses.Control ret = new Models.Responses.Control();
                Manager.ApplicationType apptype = this.Session.Database.Server.ApplicationType(ApplicationType.Name);
                ViewModel.Application applicationcontrol = this.Session.Application(apptype);
                ret.Value = new Models.Control(applicationcontrol, apptype);
                this.UpdateResponse(ret);

                return ret;
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