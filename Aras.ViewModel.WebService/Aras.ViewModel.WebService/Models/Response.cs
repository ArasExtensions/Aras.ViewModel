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

namespace Aras.ViewModel.WebService.Models
{
    public abstract class Response
    {
        public List<Control> ControlQueue { get; set; }

        public List<Command> CommandQueue { get; set; }

        private Control GetControlFromQueue(String ID)
        {
            foreach(Control control in this.ControlQueue)
            {
                if (control.ID.Equals(ID))
                {
                    return control;
                }
            }

            return null;
        }

        private Control QueueControl(ViewModel.Manager.Session Session, ViewModel.Control ViewModelControl)
        {
            // Get Model Control ID
            String id = ViewModel.Utilities.GuidToString(ViewModelControl.ID);
            
            // Check Queue for Control
            if (!this.QueuedControls.ContainsKey(id))
            {
                // Get Control Type
                Manager.ControlType type = Session.Database.Server.ControlType(ViewModelControl);
                
                // Create Control
                Control control = new Control(id, type.ClientType);

                // Add Properties to Model Control
                foreach (String name in ViewModelControl.Properties)
                {
                    // Get Type
                    Attributes.PropertyTypes propertytype = ViewModelControl.GetPropertyType(name);

                    // Create Property
                    Property property = new Property(name, propertytype, ViewModelControl.GetPropertyReadOnly(name));
                    control.Properties.Add(property);

                    // Add Value
                    object viewmodelvalue = ViewModelControl.GetPropertyValue(name);

                    switch (propertytype)
                    {
                        case ViewModel.Attributes.PropertyTypes.Boolean:

                            if (((Boolean)viewmodelvalue) == true)
                            {
                                property.Values.Add("1");
                            }
                            else
                            {
                                property.Values.Add("0");
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.Control:

                            if (viewmodelvalue != null)
                            {
                                if (this.QueuedViewModelControls.ContainsKey(((ViewModel.Control)viewmodelvalue).ID))
                                {
                                    // Control is in Queue to ensure added first
                                    Control propcontrol = this.QueueControl(Session, (ViewModel.Control)viewmodelvalue);

                                    property.Values.Add(propcontrol.ID);
                                }
                                else
                                {
                                    // Not in Queue to just add ID
                                    property.Values.Add(ViewModel.Utilities.GuidToString(((ViewModel.Control)viewmodelvalue).ID));
                                }
                            }
                            else
                            {
                                property.Values.Add(null);
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.ControlList:

                            if (viewmodelvalue != null)
                            {
                                IEnumerable<ViewModel.Control> controls = (IEnumerable<ViewModel.Control>)viewmodelvalue;

                                foreach (ViewModel.Control viewmodelcontrol in controls)
                                {
                                    if (this.QueuedViewModelControls.ContainsKey(viewmodelcontrol.ID))
                                    {
                                        // Control is in Queue to ensure added first
                                        Control propcontrol = this.QueueControl(Session, viewmodelcontrol);

                                        property.Values.Add(propcontrol.ID);
                                    }
                                    else
                                    {
                                        // Not in Queue to just add ID
                                        property.Values.Add(ViewModel.Utilities.GuidToString(viewmodelcontrol.ID));
                                    }
                                }
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.Int32:
                            property.Values.Add(((System.Int32)viewmodelvalue).ToString());
                            break;
                        case ViewModel.Attributes.PropertyTypes.NullableInt32:

                            if (viewmodelvalue == null)
                            {
                                property.Values.Add(null);
                            }
                            else
                            {
                                property.Values.Add(((System.Int32)viewmodelvalue).ToString());
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.String:
                            property.Values.Add((System.String)viewmodelvalue);
                            break;
                        case ViewModel.Attributes.PropertyTypes.Float:
                            property.Values.Add(((System.Double)viewmodelvalue).ToString());
                            break;
                        case ViewModel.Attributes.PropertyTypes.StringList:

                            if (viewmodelvalue != null)
                            {
                                IEnumerable<String> strings = (IEnumerable<String>)viewmodelvalue;

                                foreach (String value in (IEnumerable<String>)viewmodelvalue)
                                {
                                    property.Values.Add(value);
                                }
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.Date:

                            if (viewmodelvalue != null)
                            {
                                property.Values.Add(((System.DateTime)viewmodelvalue).ToString("yyyy-MM-ddTHH:mm:ssZ"));
                            }
                            else
                            {
                                property.Values.Add(null);
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.Decimal:

                            if (viewmodelvalue != null)
                            {
                                property.Values.Add(((System.Decimal)viewmodelvalue).ToString());
                            }
                            else
                            {
                                property.Values.Add(null);
                            }

                            break;
                        case ViewModel.Attributes.PropertyTypes.Command:

                            if (viewmodelvalue != null)
                            {
                                property.Values.Add(ViewModel.Utilities.GuidToString(((ViewModel.Command)viewmodelvalue).ID));
                            }
                            else
                            {
                                property.Values.Add(null);
                            }

                            break;
                        default:
                            throw new NotImplementedException("PropertyType not implemented: " + propertytype.ToString());
                    } 
                }

                // Queue Control
                this.ControlQueue.Add(control);
                this.QueuedControls[control.ID] = control;

                return control;
            }
            else
            {
                return this.QueuedControls[id];
            }
        }

        private Dictionary<String, Control> QueuedControls;
        private Dictionary<Guid, ViewModel.Control> QueuedViewModelControls;

        protected List<Control> QueueControls(ViewModel.Manager.Session Session, IEnumerable<ViewModel.Control> ViewModelControls)
        {
            this.QueuedControls = new Dictionary<String, Control>();
            this.QueuedViewModelControls = new Dictionary<Guid, ViewModel.Control>();

            List<Control> controls = new List<Control>();

            // Process Command Queue
            foreach (ViewModel.Command viewmodelcommand in Session.GetCommandsFromQueue())
            {
                Command command = new Command(viewmodelcommand);
                this.CommandQueue.Add(command);
            }

            // Merge Controls to be Queued
            foreach (ViewModel.Control viewmodelcontrol in Session.GetControlsFromQueue())
            {
                if (!this.QueuedViewModelControls.ContainsKey(viewmodelcontrol.ID))
                {
                    this.QueuedViewModelControls[viewmodelcontrol.ID] = viewmodelcontrol;
                }
            }

            foreach (ViewModel.Control viewmodelcontrol in ViewModelControls)
            {
                if (!this.QueuedViewModelControls.ContainsKey(viewmodelcontrol.ID))
                {
                    this.QueuedViewModelControls[viewmodelcontrol.ID] = viewmodelcontrol;
                }
            }

            // Process Controls
            foreach(ViewModel.Control viewmodelcontrol in this.QueuedViewModelControls.Values)
            {
                Control control = this.QueueControl(Session, viewmodelcontrol);

                if (ViewModelControls.Contains(viewmodelcontrol))
                {
                    controls.Add(control);
                }
            }

            return controls;
        }

        public Response(ViewModel.Manager.Session Session)
        {
            this.ControlQueue = new List<Control>();
            this.CommandQueue = new List<Command>();
        }
    }
}