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
    public class Configuration
    {
        public Order Order { get; private set; }

        internal Item Item { get; private set; }

        internal Innovator Innovator
        {
            get
            {
                return this.Order.Innovator;
            }
        }

        private Context _context;
        public Context Context
        {
            get
            {
                if (this._context == null)
                {
                    Item contextitem = this.Innovator.newItem("Variant Context", "get");
                    contextitem.setID(this.Item.getProperty("related_id"));
                    contextitem = contextitem.apply();
                    this._context = new Context(contextitem);
                }

                return this._context;
            }
        }

        public String Value
        {
            get
            {
                return this.Item.getProperty("value");
            }
        }

        public Double Quantity
        {
            get
            {
                Double ret = 0.0;
                Double.TryParse(this.Item.getProperty("quantity"), out ret);
                return ret;
            }
        }

        internal Configuration(Order Order, Item Item)
        {
            this.Order = Order;
            this.Item = Item;
        }
    }
}
