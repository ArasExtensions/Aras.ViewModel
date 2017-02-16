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
using System.Web;

namespace Aras.ViewModel.WebService.Models
{
    public class Property
    {
        public String Name { get; set; }

        public ViewModel.Attributes.PropertyTypes Type { get; set; }

        public Boolean ReadOnly { get; set; }

        public List<String> Values { get; set; }

        public Property()
        {

        }

        public Property(String Name, ViewModel.Control Control)
        {
            this.Name = Name;
            this.ReadOnly = Control.GetPropertyReadOnly(Name);
            this.Values = new List<String>();
            this.Type = Control.GetPropertyType(Name); 

            object property = Control.GetPropertyValue(Name);

            switch (this.Type)
            {
                case ViewModel.Attributes.PropertyTypes.Boolean:

                    if (((Boolean)property) == true)
                    {
                        this.Values.Add("1");
                    }
                    else
                    {
                        this.Values.Add("0");
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.Control:

                    if (property != null)
                    {
                        this.Values.Add(ViewModel.Utilities.GuidToString(((ViewModel.Control)property).ID));
                    }
                    else
                    {
                        this.Values.Add(null);
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.ControlList:

                    if (property != null)
                    {
                        IEnumerable<ViewModel.Control> controls = (IEnumerable<ViewModel.Control>)property;

                        foreach (ViewModel.Control control in controls)
                        {
                            this.Values.Add(ViewModel.Utilities.GuidToString(control.ID));
                        }
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.Int32:
                    this.Values.Add(((System.Int32)property).ToString());
                    break;
                case ViewModel.Attributes.PropertyTypes.NullableInt32:

                    if (property == null)
                    {
                        this.Values.Add(null);
                    }
                    else
                    {
                        this.Values.Add(((System.Int32)property).ToString());
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.String:
                    this.Values.Add((System.String)property);
                    break;
                case ViewModel.Attributes.PropertyTypes.Float:
                    this.Values.Add(((System.Double)property).ToString());
                    break;
                case ViewModel.Attributes.PropertyTypes.StringList:

                    if (property != null)
                    {
                        IEnumerable<String> strings = (IEnumerable<String>)property;

                        foreach (String value in (IEnumerable<String>)property)
                        {
                            this.Values.Add(value);
                        }
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.Date:

                    if (property != null)
                    {
                        this.Values.Add(((System.DateTime)property).ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    }
                    else
                    {
                        this.Values.Add(null);
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.Decimal:

                    if (property != null)
                    {
                        this.Values.Add(((System.Decimal)property).ToString());
                    }
                    else
                    {
                        this.Values.Add(null);
                    }

                    break;
                case ViewModel.Attributes.PropertyTypes.Command:

                    if (property != null)
                    {
                        this.Values.Add(ViewModel.Utilities.GuidToString(((ViewModel.Command)property).ID));
                    }
                    else
                    {
                        this.Values.Add(null);
                    }

                    break;
                default:
                    throw new NotImplementedException("PropertyType not implemented: " + this.Type.ToString());
            }        
        }
    }
}