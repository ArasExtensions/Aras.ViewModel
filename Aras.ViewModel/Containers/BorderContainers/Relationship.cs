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

namespace Aras.ViewModel.Containers.BorderContainers
{
    public class Relationship : BorderContainer
    {
        public Containers.Toolbar Toolbar { get; private set; }

        public Grids.Relationship Grid { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (this.Binding != null)
            {
                if (!(this.Binding is Model.Item))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.Grid.Binding = this.Binding;
        }

        public Relationship(IItemControl Parent, Model.RelationshipType RelationshipType)
            :base(Parent.Session)
        {
            // Create Search
            this.Grid = new Grids.Relationship(Parent, RelationshipType);
            this.Grid.Region = Regions.Center;

            // Set Toolbar
            this.Toolbar = ((IToolbarProvider)this.Grid).Toolbar;
            this.Toolbar.Region = Regions.Top;

            // Add Children
            this.Children.Add(this.Toolbar);
            this.Children.Add(this.Grid);
        }
    }
}
