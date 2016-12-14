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
    public class Context
    {
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

        internal Item Item { get; private set; }

        public String Name
        {
            get
            {
                return this.Item.getProperty("name");
            }
        }

        private List<Context> _parameters;
        public List<Context> Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new List<Context>();

                    Item parameteritems = this.Innovator.newItem("Variants Context Parameters", "get");
                    parameteritems.setProperty("source_id", this.Item.getID());
                    parameteritems.setAttribute("select", "id,related_id(id,name)");
                    parameteritems = parameteritems.apply();

                    if (!parameteritems.isError())
                    {
                        for (int i = 0; i < parameteritems.getItemCount(); i++)
                        {
                            Item parameteritem = parameteritems.getItemByIndex(i);
                            this._parameters.Add(new Context(parameteritem.getRelatedItem()));
                        }
                    }
                }

                return this._parameters;
            }
        }

        internal Context(Item Item)
        {
            this.Item = Item;
        }
    }
}
