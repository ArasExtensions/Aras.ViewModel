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

namespace Aras.ViewModel.WebService.Controllers
{
    public class ControlsController : BaseController
    {
        [Route("controls/{ID}")]
        [HttpGet]
        public Models.Responses.Control GetControl(String ID)
        {
            try
            {
                Guid viewmodelid = ViewModel.Utilities.StringToGuid(ID);
                ViewModel.Control control = this.Session.Control(viewmodelid);
                return new Models.Responses.Control(this.Session, control);

            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        [Route("controls/{ID}")]
        [HttpPut]
        public Models.Response UpdateControl(String ID, Models.Control Control)
        {
            try
            {
                Guid viewmodelid = ViewModel.Utilities.StringToGuid(ID);
                ViewModel.Control control = this.Session.Control(viewmodelid);

                foreach (Models.Property property in Control.Properties)
                {
                    if (!property.ReadOnly)
                    {
                        switch (property.Type)
                        {
                            case ViewModel.Attributes.PropertyTypes.Boolean:
                                control.SetPropertyValue(property.Name, property.Values[0].Equals("1"));
                                break;
                            case ViewModel.Attributes.PropertyTypes.Int32:
                                control.SetPropertyValue(property.Name, Convert.ToInt32(property.Values[0]));
                                break;
                            case ViewModel.Attributes.PropertyTypes.NullableInt32:

                                if (property.Values[0] == null)
                                {
                                    control.SetPropertyValue(property.Name, null);
                                }
                                else
                                {
                                    control.SetPropertyValue(property.Name, Convert.ToInt32(property.Values[0]));
                                }

                                break;
                            case ViewModel.Attributes.PropertyTypes.String:
                                control.SetPropertyValue(property.Name, property.Values[0]);
                                break;
                            case ViewModel.Attributes.PropertyTypes.Float:
                                control.SetPropertyValue(property.Name, Convert.ToDouble(property.Values[0]));
                                break;
                            case ViewModel.Attributes.PropertyTypes.Control:
                                String id = property.Values[0];

                                if (id != null)
                                {
                                    control.SetPropertyValue(property.Name, this.Session.Control(ViewModel.Utilities.StringToGuid(id)));
                                }
                                else
                                {
                                    control.SetPropertyValue(property.Name, null);
                                }

                                break;
                            case ViewModel.Attributes.PropertyTypes.ControlList:

                                List<ViewModel.Control> values = new List<ViewModel.Control>();

                                foreach (String thisid in property.Values)
                                {
                                    values.Add(this.Session.Control(ViewModel.Utilities.StringToGuid(thisid)));
                                }

                                ((ViewModel.ObservableControlList<ViewModel.Control>)control.GetPropertyValue(property.Name)).Replace(values);

                                break;
                            case ViewModel.Attributes.PropertyTypes.Date:

                                if (property.Values[0] == null)
                                {
                                    control.SetPropertyValue(property.Name, null);
                                }
                                else
                                {
                                    control.SetPropertyValue(property.Name, Convert.ToDateTime(property.Values[0]));
                                }

                                break;
                            case ViewModel.Attributes.PropertyTypes.Decimal:

                                if (property.Values[0] == null)
                                {
                                    control.SetPropertyValue(property.Name, null);
                                }
                                else
                                {
                                    control.SetPropertyValue(property.Name, Convert.ToDecimal(property.Values[0]));
                                }

                                break;
                            case ViewModel.Attributes.PropertyTypes.Command:

                                if (property.Values[0] == null)
                                {
                                    control.SetPropertyValue(property.Name, null);
                                }
                                else
                                {
                                    control.SetPropertyValue(property.Name, this.Session.Command(ViewModel.Utilities.StringToGuid(property.Values[0])));
                                }

                                break;
                            default:
                                throw new NotImplementedException("PropertyType not implemented: " + property.Type);
                        }
                    }
                }

                return new Models.Responses.Empty(this.Session);
            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
        }

        public ControlsController(ViewModel.Manager.Server Server)
            : base(Server)
        {
        }
    }
}
