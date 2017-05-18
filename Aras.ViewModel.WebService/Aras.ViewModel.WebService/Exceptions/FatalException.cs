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
using System.Web.Http;
using System.Net.Http;

namespace Aras.ViewModel.WebService.Exceptions
{
    public class FatalException : HttpResponseException
    {
   
        public FatalException(Controllers.BaseController Controller, Exception e)
            : base(Controller.Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message))
        {
            // Log Debug Message
            Controller.Server.Log.Add(Logging.Levels.Debug, e.Message + Environment.NewLine + e.ToString());
        }
    }
}