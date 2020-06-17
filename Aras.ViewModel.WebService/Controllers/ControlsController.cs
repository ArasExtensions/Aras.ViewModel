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
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Starting Get controls/{0}", ID));

                Guid viewmodelid = ViewModel.Utilities.StringToGuid(ID);
                ViewModel.Control control = this.Session.Control(viewmodelid);
                return new Models.Responses.Control(this.Session, control);

            }
            catch (Exception e)
            {
                throw this.ProcessException(e);
            }
            finally
            {
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Completed Get controls/{0}", ID));
            }
        }

        [Route("controls/{ID}")]
        [HttpPut]
        public Models.Response UpdateControl(String ID, Models.Control Control)
        {
            try
            {
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Completed Put controls/{0}", ID));

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
            finally
            {
                this.Server.Log.Add(Logging.Levels.Debug, String.Format("Completed Put controls/{0}", ID));
            }
        }

        public ControlsController(ViewModel.Manager.Server Server)
            : base(Server)
        {
        }
    }
}
