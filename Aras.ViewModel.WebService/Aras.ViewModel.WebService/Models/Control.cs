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
    public class Control
    {
        public String ID { get; set; }

        public String Type { get; set; }

        public List<Property> Properties { get; set; }

        public List<Command> Commands { get; set; }

        public Control()
        {

        }

        public Control(ViewModel.Control Control, Manager.ControlType ControlType)
        {
            // Set ID
            this.ID = ViewModel.Utilities.GuidToString(Control.ID);
            
            // Set Type
            this.Type = ControlType.ClientType;

            // Add Properties
            this.Properties = new List<Property>();

            foreach(String name in Control.Properties)
            {
                this.Properties.Add(new Property(name, Control));
            }

            // Add Commands
            this.Commands = new List<Command>();

            foreach (String name in Control.Commands)
            {
                this.Commands.Add(new Command(name, Control.GetCommand(name)));
            }
        }
    }
}