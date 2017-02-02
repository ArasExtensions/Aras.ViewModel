/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2016 Processwall Limited.

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

namespace Aras.ViewModel
{
    public class Form : Item
    {
        [ViewModel.Attributes.Property("Properties", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Property> Fields { get; private set; }

        private List<String> _propertyNames;
        public IEnumerable<String> PropertyNames
        {
            get
            {
                return this._propertyNames;
            }
        }

        public void AddPropertyNames(String Names)
        {
            foreach (String name in Names.Split(','))
            {
                if (!this._propertyNames.Contains(name))
                {
                    this._propertyNames.Add(name);
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.Fields.NotifyListChanged = false;
            this.Fields.Clear();

            if (this.Binding != null)
            {
                //this.Transaction = ((Model.Item)this.Binding).Transaction;

                foreach (String propertyname in this.PropertyNames)
                {
                    if (((Model.Item)this.Binding).HasProperty(propertyname))
                    {
                        Model.Property modelproperty = ((Model.Item)this.Binding).Property(propertyname);
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
                                throw new NotImplementedException("Property Type not implemented: " + ((Model.Item)this.Binding).HasProperty(propertyname));
                        }

                        property.Binding = modelproperty;
                        this.Fields.Add(property);
                    }
                }
            }

            this.Fields.NotifyListChanged = true;
        }

        public Form(Manager.Session Session)
            :base(Session)
        {
            this._propertyNames = new List<String>();
            this.Fields = new Model.ObservableList<Property>();
        }

    }
}
