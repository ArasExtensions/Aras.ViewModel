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
