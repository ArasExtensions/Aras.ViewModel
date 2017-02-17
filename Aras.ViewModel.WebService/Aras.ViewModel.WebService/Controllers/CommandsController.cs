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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace Aras.ViewModel.WebService.Controllers
{
    public class CommandsController : BaseController
    {
        [Route("commands/{ID}/execute")]
        [HttpPut]
        public Models.Response ExecuteCommand(String ID, List<String> Parameters)
        {
            try
            {
                // Get Parameters
                List<ViewModel.Control> viewmodelparameters = new List<ViewModel.Control>();

                if (Parameters != null)
                {
                    foreach (String parameter in Parameters)
                    {
                        Guid parameterviewmodelid = ViewModel.Utilities.StringToGuid(parameter);
                        ViewModel.Control parametercontrol = this.Session.Control(parameterviewmodelid);
                        viewmodelparameters.Add(parametercontrol);
                    }
                }

                // Run Command
                Guid viewmodelid = ViewModel.Utilities.StringToGuid(ID);
                this.Session.Command(viewmodelid).Execute(viewmodelparameters);

                // Return Response
                return new Models.Responses.Empty(this.Session);
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        public CommandsController(ViewModel.Manager.Server Server)
            : base(Server)
        {
        }
    }
}
