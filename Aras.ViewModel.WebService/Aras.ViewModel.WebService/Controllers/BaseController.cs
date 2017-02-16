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

                return this._session;
            }
        }

        protected void UpdateResponse(Models.Response Response)
        {
            // Add Control Queue
            foreach (ViewModel.Control control in this.Session.GetControlsFromQueue())
            {
                Response.ControlQueue.Add(new Models.Control(control, this.Session.Database.Server.ControlType(control)));
            }

            // Add Command Queue
            foreach (ViewModel.Command command in this.Session.GetCommandsFromQueue())
            {
                Response.CommandQueue.Add(new Models.Command(command));
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
