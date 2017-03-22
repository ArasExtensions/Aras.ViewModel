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

namespace Aras.ViewModel.Dialogs
{
    public class Search : Dialog
    {
        public Model.Stores.Item Store { get; private set; }

        public Grids.Search Grid
        {
            get
            {
                return ((Containers.BorderContainers.Search)this.Content).Grid;
            }
        }

        public Search(Control Parent, Model.Stores.Item Store)
            :base(Parent)
        {
            // Save Store
            this.Store = Store;
            
            // Set Title
            if (this.Store != null)
            {
                this.Title = "Select " + this.Store.ItemType.SingularLabel;
            }
            else
            {
                this.Title = null;
            }
            
            // Set Default Width
            this.Width = 600;

            // Set Default Height
            this.Height = 800;

            // Create Border Content
            this.Content = new Containers.BorderContainers.Search(this.Session);
            this.Content.Binding = this.Store;
        }
    }
}
