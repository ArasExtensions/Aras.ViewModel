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
                ViewModel.Manager.Session session = database.Login(Credentials.Username, Credentials.Password);
                HttpCookie cookie = new HttpCookie(tokencookie, session.Token);
                HttpContext.Current.Response.Cookies.Add(cookie);

                Models.Response response = new Models.Response();
                return response;
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
