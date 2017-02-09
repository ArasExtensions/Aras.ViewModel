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
                if (this.Binding is Model.Stores.Item)
                {
                    this.Query = ((Model.Stores.Item)this.Binding).Query();
                    this.Query.PageSize = (System.Int32)this.PageSize.Value;
                    this.Query.Paging = true;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Stores.Item");
                }
            }
            else
            {
                this.Query = null;
            }

            this.RefreshControl();
        }

        public IEnumerable<Model.Item> Displayed
        {
            get
            {
                if (this.Query != null)
                {
                    return this.Query.CurrentItems();
                }
                else
                {
                    return new List<Model.Item>();
                }
            }
        }

        public Model.ObservableList<Model.Item> Selected { get; private set; }

        public void Select(Model.Item Item)
        {
            if (Item != null)
            {
                List<Model.Item> displayed = this.Displayed.ToList();
                int index = displayed.IndexOf(Item);

                if (index >= 0)
                {
                    this.Grid.SelectedRows.Clear();
                    this.Grid.SelectedRows.Add(this.Grid.Rows[index]);
                }
            }
        }

        private Model.Queries.Item Query;

        private List<Model.PropertyType> PropertyTypes;

        protected override void LoadColumns()
        {
            if (this.Binding != null)
            {
                this.Grid.Columns.Clear();

                // Build List of PropertyTypes
                this.PropertyTypes = new List<Model.PropertyType>();

                foreach (String propertyname in this.PropertyNames)
                {
                    if (((Model.Stores.Item)this.Binding).ItemType.HasPropertyType(propertyname))
                    {
                        this.PropertyTypes.Add(((Model.Stores.Item)this.Binding).ItemType.PropertyType(propertyname));
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
                                case "Sequence":
                                    this.Grid.Rows[i].Cells[j].Value = new Properties.Sequence(this.Session);
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
                case "Sequence":
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

        private void SelectedRows_ListChanged(object sender, EventArgs e)
        {
            this.Selected.NotifyListChanged = false;
            this.Selected.Clear();

            foreach(Row row in this.Grid.SelectedRows)
            {
                if (row.Cells.Count() > 0)
                {
                    Cell cell = row.Cells.First();

                    if (cell.Value != null)
                    {
                        if ((cell.Value.Binding != null) && (cell.Value.Binding is Model.Property))
                        {
                            this.Selected.Add(((Model.Property)cell.Value.Binding).Item);
                        }
                    }
                }
            }

            this.Selected.NotifyListChanged = true;
        }

        public ItemType(Manager.Session Session)
            :base(Session)
        {
            this.Selected = new Model.ObservableList<Model.Item>();
            this.Grid.SelectedRows.ListChanged += SelectedRows_ListChanged;
        }
    }
}
