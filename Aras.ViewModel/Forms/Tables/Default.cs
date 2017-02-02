/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2017 Processwall Limited.

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
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Forms.Tables
{
    public class Default : Form
    {
        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.Children.NotifyListChanged = false;
            this.Children.Clear();

            if (this.Binding != null)
            {
                foreach (Model.Property modelproperty in ((Model.Item)this.Binding).Properties)
                {
                    ViewModel.Property property = null;

                    switch (modelproperty.Type.GetType().Name)
                    {
                        case "Float":
                            property = new Properties.Float(this.Session);
                            break;
                        case "List":
                            property = new Properties.List(this.Session);
                            break;
                        case "String":
                            property = new Properties.String(this.Session);
                            break;
                        default:
                            throw new NotImplementedException("Property Type not implemented: " + modelproperty.Type.GetType().Name);
                    }

                    property.Binding = modelproperty;
                    this.Children.Add(property);
                }
            }

            this.Children.NotifyListChanged = true;
        }

        public Default(Manager.Session Session)
            :base(Session)
        {

        }
    }
}
