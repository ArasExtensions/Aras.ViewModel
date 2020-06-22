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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace Aras.ViewModel.WebService.Controllers
{
    public class CommandsController : BaseController
    {
        [Route("commands/{ID}/execute")]
        [HttpPost]
        public Models.Response ExecuteCommand(String ID, List<String> Parameters)
        {
            try
            {
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Starting Put commands/{0}/execute", ID));

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
            finally
            {
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Completed Put commands/{0}/execute", ID));
            }
        }

        public CommandsController(ViewModel.Manager.Server Server)
            : base(Server)
        {
        }
    }
}
