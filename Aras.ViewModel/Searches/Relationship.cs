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

namespace Aras.ViewModel.Searches
{
    public class Relationship : Search<Model.Relationship>
    {
        private Model.Query<Model.Relationship> _query;
        protected override Model.Query<Model.Relationship> Query
        {
            get
            {
                if (this._query == null)
                {
                    if ((this.Binding != null) && (this.Binding is Model.Stores.Relationship))
                    {
                        // Create Query
                        this._query = ((Model.Stores.Relationship)this.Binding).Query();

                        // Switch on Paging
                        this._query.Paging = true;

                        // Update Page Size on Control
                        this.PageSize.Value = this._query.PageSize;

                        // Update Page on Control
                        this.Page.Value = this._query.Page;
                    }
                }

                return this._query;
            }
        }

        public Relationship(Manager.Session Session)
            :base(Session)
        {

        }
    }
}
