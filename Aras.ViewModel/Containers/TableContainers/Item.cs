﻿/*  
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

namespace Aras.ViewModel.Containers.TableContainers
{
    public class Item : TableContainer
    {
        public Model.ItemType ItemType { get; private set; }

        private Dictionary<Model.PropertyType, ViewModel.Property> PropertyCache;

        private void BuildProperties(String Properties)
        {
            this.Children.NotifyListChanged = false;

            // Build Properties
            String[] proparray = Properties.Split(new char[1] { ',' });

            foreach (String prop in proparray)
            {
                if (this.ItemType.HasPropertyType(prop))
                {
                    Model.PropertyType proptype = this.ItemType.PropertyType(prop);

                    this.PropertyCache[proptype] = this.Session.CreateProperty(proptype, false);
                    this.Children.Add(this.PropertyCache[proptype]);
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Invalid Property: " + prop);
                }
            }

            // Run After Build Properties
            this.AfterBuildProperties();

            this.Children.NotifyListChanged = true;
        }

        protected virtual void AfterBuildProperties()
        {

        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            foreach (Model.PropertyType proptype in this.PropertyCache.Keys)
            {
                if (this.Binding != null)
                {
                    this.PropertyCache[proptype].Binding = ((Model.Item)this.Binding).Property(proptype);
                }
                else
                {
                    this.PropertyCache[proptype].Binding = null;
                }
            }
        }


        public Item(Manager.Session Session, Model.ItemType ItemType, String Properties)
            :base(Session)
        {
            // Create Property Cache
            this.PropertyCache = new Dictionary<Model.PropertyType, Property>();
            
            // Store ItemType
            this.ItemType = ItemType;

            // Build Properties
            this.BuildProperties(Properties);
        }
    }
}
