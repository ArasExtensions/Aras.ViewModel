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
using System.Web;

namespace Aras.ViewModel.WebService.Controllers
{
    public class DatabasesController : BaseController
    {
        [Route("databases")]
        [HttpGet]
        public IEnumerable<Models.Database> GetAllDatabases()
        {
            try
            {
                List<Models.Database> ret = new List<Models.Database>();

                foreach (ViewModel.Manager.Database database in this.Server.Databases)
                {
                    ret.Add(new Models.Database(database));
                }

                return ret;
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        [Route("databases/{Name}")]
        [HttpGet]
        public Models.Database GetDatabase(String Name)
        {
            try
            {
                // Get Database
                ViewModel.Manager.Database database = this.Server.Database(Name);

                // Return Database
                return new Models.Database(database);
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        [Route("databases/{Name}/login")]
        [HttpPut]
        public Models.Response Login(String Name, Models.Credentials Credentials)
        {
            try
            {
                // Get Database
                ViewModel.Manager.Database database = this.Server.Database(Name);

                // Login
                ViewModel.Manager.Session session = database.Login(Credentials.Username, Credentials.AccessToken);
                HttpCookie cookie = new HttpCookie(tokencookie, session.Token);
                HttpContext.Current.Response.Cookies.Add(cookie);

                return new Models.Responses.Empty(session);
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        public DatabasesController(ViewModel.Manager.Server Server)
            :base(Server)
        { 
        }
    }
}
