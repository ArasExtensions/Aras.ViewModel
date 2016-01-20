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
using System.Text;
using System.Threading.Tasks;
using Aras.IOM;

namespace Aras.ViewModel.Design.Server
{
    public class Order
    {
        internal Item Item { get; private set; }

        private Innovator _innovator;
        internal Innovator Innovator
        {
            get
            {
                if (this._innovator == null)
                {
                    this._innovator = this.Item.getInnovator();
                }

                return this._innovator;
            }
        }

        private List<Configuration> _configurationsCache;
        private List<Configuration> ConfigurationsCache
        {
            get
            {
                if (this._configurationsCache == null)
                {
                    this._configurationsCache = new List<Configuration>();

                    Item configurationitems = this.Item.getRelationships();

                    for (int i=0; i<configurationitems.getItemCount(); i++)
                    {
                        Item configurationitem = configurationitems.getItemByIndex(i);
                        Configuration configuration = new Configuration(this, configurationitem);
                        this._configurationsCache.Add(configuration);
                    }
                }

                return this._configurationsCache;
            }
        }

        public IEnumerable<Configuration> Configurations
        {
            get
            {
                return this.ConfigurationsCache;
            }
        }

        public Configuration Configuration(String ContextName)
        {
            foreach (Configuration config in this.Configurations)
            {
                if (config.Context.Name == ContextName)
                {
                    return config;
                }
            }

            return null;
        }

        public Configuration ActiveConfiguration
        {
            get
            {
                // The Active Configuration if always the firt in the list
                return this.ConfigurationsCache.First();
            }
        }

        private List<Configuration> _parametersCache;
        private List<Configuration> ParametersCache
        {
            get
            {
                if (this._parametersCache == null)
                {
                    this._parametersCache = new List<Configuration>();

                    foreach (Context parameter in this.ActiveConfiguration.Context.Parameters)
                    {

                        foreach (Configuration config in this.Configurations)
                        {
                    
                            if (parameter.Name.Equals(config.Context.Name))
                            {
                                this._parametersCache.Add(config);
                            }
                        }
                    }

                }

                return this._parametersCache;
            }
        }

        public Configuration Parameter(int Index)
        {
            if (Index >= 0 && Index < this.ParametersCache.Count())
            {
                return this.ParametersCache[Index];
            }
            else
            {
                throw new ArgumentException("Invalid Index");
            }
        }

        public IEnumerable<Configuration> Parameters
        {
            get
            {
                return this.ParametersCache;
            }
        }

        public Item ReturnItem(String Value, Double Quantity)
        {
            Item ret = this.Innovator.newItem("v_Order Context", "get");
            ret.setID(this.Innovator.getNewID());
            ret.setProperty("source_id", this.Item.getID());
            ret.setProperty("related_id", null);
            ret.setProperty("value", Value);
            ret.setProperty("quantity", Quantity.ToString());
            return ret;
        }

        public Order(Item Item)
        {
            this.Item = Item;
        }
    }
}
