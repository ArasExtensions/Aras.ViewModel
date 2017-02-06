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

namespace Aras.ViewModel.Grids.Searches
{
    public class ItemType : Search
    {
        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.ItemType)
                {
                    this.Query = this.Session.Model.Store((Model.ItemType)this.Binding).Query();
                    this.Query.PageSize = (System.Int32)this.PageSize.Value;
                    this.Query.Paging = true;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.ItemType");
                }
            }
            else
            {
                this.Query = null;
            }

            this.RefreshControl();
        }

        public Model.ObservableList<Model.Item> Selected { get; private set; }

        private Model.Queries.Item Query;

        private List<Model.PropertyType> PropertyTypes;

        protected override void LoadColumns()
        {
            if ((this.Binding != null) && (this.Binding is Model.ItemType))
            {
                this.Grid.Columns.Clear();

                // Build List of PropertyTypes
                this.PropertyTypes = new List<Model.PropertyType>();

                foreach (String propertyname in this.PropertyNames)
                {
                    if (((Model.ItemType)this.Binding).HasPropertyType(propertyname))
                    {
                        this.PropertyTypes.Add(((Model.ItemType)this.Binding).PropertyType(propertyname));
                    }
                }

                foreach (Model.PropertyType proptype in this.PropertyTypes)
                {
                    this.Grid.AddColumn(proptype.Name, proptype.Label);
                }
            }
            else
            {
                // Clear Columns
                this.Grid.Columns.Clear();
            }
        }

        private void LoadRows()
        {
            if (this.Query != null)
            {
                this.Grid.NoRows = this.Query.Count();

                for (int i = 0; i < this.Grid.NoRows; i++)
                {
                    Model.Item item = this.Query[i];

                    for (int j = 0; j < this.PropertyTypes.Count(); j++)
                    {
                        Model.PropertyType proptype = this.PropertyTypes[j];
                        Model.Property property = item.Property(proptype);

                        if (this.Grid.Rows[i].Cells[j].Value == null)
                        {
                            switch (property.GetType().Name)
                            {
                                case "String":
                                    this.Grid.Rows[i].Cells[j].Value = new Properties.String(this.Session);
                                    break;
                                case "Integer":
                                    this.Grid.Rows[i].Cells[j].Value = new Properties.Integer(this.Session);
                                    break;
                                default:
                                    throw new Model.Exceptions.ArgumentException("PropertyType not implmented: " + property.GetType().Name);
                            }
                        }

                        this.Grid.Rows[i].Cells[j].Value.Binding = property;
                    }
                }
            }
            else
            {
                // Clear all Rows
                this.Grid.NoRows = 0;
            }
        }

        private Model.Condition Condition(Model.PropertyType PropertyType)
        {
            switch (PropertyType.GetType().Name)
            {
                case "String":
                    return Aras.Conditions.Like(PropertyType.Name, "%" + this.QueryString.Value + "%");
                case "Integer":
                    System.Int32 int32value = 0;

                    if (System.Int32.TryParse(this.QueryString.Value, out int32value))
                    {
                        return Aras.Conditions.Eq(PropertyType.Name, int32value);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    throw new NotImplementedException("Property Type not implemented: " + PropertyType.GetType().Name);
            }
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            if (this.Query != null)
            {
                // Set Condition
                if (String.IsNullOrEmpty(this.QueryString.Value))
                {
                    this.Query.Condition = null;
                }
                else
                {
                    List<Model.Condition> conditions = new List<Model.Condition>();

                    foreach(Model.PropertyType proptype in this.PropertyTypes)
                    {
                        Model.Condition condition = this.Condition(proptype);

                        if (condition != null)
                        {
                            conditions.Add(condition);
                        }
                    }

                    if (conditions.Count() == 1)
                    {
                        this.Query.Condition = conditions[0];
                    }
                    else
                    {
                        this.Query.Condition = Aras.Conditions.Or(conditions[0], conditions[1]);

                        if (conditions.Count() > 2)
                        {
                            for (int i = 2; i < conditions.Count(); i++)
                            {
                                ((Model.Conditions.Or)this.Query.Condition).Add(conditions[i]);
                            }
                        }
                    }
                }

                // Set PageSize and required Page
                this.Query.PageSize = (System.Int32)this.PageSize.Value;
                this.Query.Page = (System.Int32)this.Page.Value;

                // Refresh Query
                this.Query.Refresh();

                // Update NoPages
                this.NoPages.Value = this.Query.NoPages;
            }
            else
            {
                this.NoPages.Value = 0;
            }

            // Load Grid
            this.LoadRows();

            // Refresh Buttons
            this.NextPage.Refesh();
            this.PreviousPage.Refesh();
        }

        public ItemType(Manager.Session Session)
            :base(Session)
        {
            this.Selected = new Model.ObservableList<Model.Item>();
        }
    }
}
