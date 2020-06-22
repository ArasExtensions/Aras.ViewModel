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
    public abstract class BaseController : ApiController
    {
        protected const String tokencookie = "token";

        internal ViewModel.Manager.Server Server { get; private set; }

        private ViewModel.Manager.Session _session;
        protected ViewModel.Manager.Session Session
        {
            get
            {
                if (this._session == null)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies[tokencookie];
                    String token = cookie.Value;
                    this._session = this.Server.Session(token);
                }

                String auth_token = HttpContext.Current.Request?.Headers["AUTH_TOKEN"];
                this._session.Model.IO.UpdateAccesToken(auth_token);

                return this._session;
            }
        }

        protected Exception ProcessException(Exception e)
        {
            if (e is ViewModel.Manager.Exceptions.SessionException)
            {
                return new Exceptions.SessionException(this, (ViewModel.Manager.Exceptions.SessionException)e);
            }
            else
            {
                return new Exceptions.FatalException(this, e);
            }
        }

        protected BaseController(ViewModel.Manager.Server Server)
        {
            this.Server = Server;
        }
    }
}
