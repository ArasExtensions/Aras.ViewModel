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

namespace Aras.ViewModel.Grids
{
    public class Relationship : Containers.BorderContainer
    {
        public Containers.Form Form { get; private set; }

        public Model.RelationshipType RelationshipType { get; private set; }

        protected ViewModel.Grid Grid { get; private set; }

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

            if (this.Binding != null)
            {
                // Add RelationshipGrid PropertTypes to Select for the Relationship
                this.RelationshipType.AddRelationshipGridPropertyTypesToSelect();

                // Add RelationshipGrid PropertTypes to Select for the Related ItemType
                if (this.RelationshipType.RelatedItemType != null)
                {
                    this.RelationshipType.RelatedItemType.AddRelationshipGridPropertyTypesToSelect();
                }
            }

            this.LoadRows();
        }

        private List<Model.PropertyType> _propertyTypes;
        protected IEnumerable<Model.PropertyType> PropertyTypes
        {
            get
            {
                if (this._propertyTypes == null)
                {
                    this._propertyTypes = new List<Model.PropertyType>();

                    switch (this.RelationshipType.RelationshipGridView)
                    {
                        case Model.RelationshipGridViews.InterMix:
                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.Sort();

                            break;
                        case Model.RelationshipGridViews.Right:

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            break;
                        default:
                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            break;
                    }
                }

                return this._propertyTypes;
            }
        }

        protected void LoadColumns()
        {
            // Clear Columns
            this.Grid.Columns.Clear();

            foreach (Model.PropertyType proptype in this.PropertyTypes)
            {
                this.Grid.AddColumn(proptype.Name, proptype.Label, proptype.ColumnWidth);
            }

        }

        protected void LoadRows()
        {
            if (this.Binding != null)
            {
                List<Model.Relationship> currentitems = ((Model.Item)this.Binding).Store(this.RelationshipType).CurrentItems().ToList();
                this.Grid.NoRows = currentitems.Count();

                for(int i=0; i < currentitems.Count(); i++)
                {
                    Model.Relationship relationship = currentitems[i];

                    int j = 0;

                    foreach (Model.PropertyType proptype in this.PropertyTypes)
                    {
                        Model.Property property = null;

                        if (relationship.ItemType.Equals(proptype.Type))
                        {
                            property = relationship.Property(proptype);
                        }
                        else
                        {
                            if (relationship.Related != null)
                            {
                                property = relationship.Related.Property(proptype);
                            }
                        }

                        if (property != null)
                        {
                            if (this.Grid.Rows[i].Cells[j].Value == null)
                            {
                                this.Grid.Rows[i].Cells[j].Value = this.Session.CreateProperty(property);
                            }
                            else
                            {
                                this.Grid.Rows[i].Cells[j].Value.Binding = property;
                            }
                        }
                        else
                        {
                            this.Grid.Rows[i].Cells[j].Value = null;
                        }

                        j++;
                    }
                }
            }
            else
            {
                // Clear all Rows
                this.Grid.NoRows = 0;
            }
        }

        public void EditRelationship()
        {
            if (this.Form.Transaction != null)
            {
                if (this.Binding != null)
                {
                    foreach(Model.Relationship currentitem in ((Model.Item)this.Binding).Store(this.RelationshipType).CurrentItems())
                    {
                        currentitem.Update(this.Form.Transaction);
                    }
                }
            }
        }

        public Relationship(Containers.Form Form, Model.RelationshipType RelationshipType)
            :base(Form.Session)
        {
            // Store Form
            this.Form = Form;

            // Store RelationshipType
            this.RelationshipType = RelationshipType;

            // Create Grid
            this.Grid = new Grid(this.Session);
            this.Grid.AllowSelect = true;
            this.Grid.Width = this.Width;
            this.Children.Add(this.Grid);

            // Load Columns
            this.LoadColumns();
        }
    }
}
