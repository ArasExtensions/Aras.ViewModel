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

            // Reset Query
            this._query = null;

            if (this.Binding != null)
            {
                // Add Search PropertTypes to Select
                ((Model.Stores.Item)this.Binding).ItemType.AddSearchPropertyTypesToSelect();
            }
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

        private Model.Queries.Item _query;
        private Model.Queries.Item Query
        {
            get
            {
                if (this._query == null)
                {
                    if (this.Binding != null)
                    {
                        this._query = ((Model.Stores.Item)this.Binding).Query();
                        this._query.PageSize = (System.Int32)this.PageSize.Value;
                        this._query.Paging = true;
                    }
                }

                return this._query;
            }
        }

        protected override void LoadColumns()
        {
            if (this.Binding != null)
            {
                this.Grid.Columns.Clear();

                foreach (Model.PropertyType proptype in this.Query.Store.ItemType.SearchPropertyTypes)
                {
                    this.Grid.AddColumn(proptype.Name, proptype.Label, proptype.ColumnWidth);
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
                    int j = 0;

                    foreach (Model.PropertyType proptype in this.Query.Store.ItemType.SearchPropertyTypes)
                    {
                        Model.Property property = item.Property(proptype);

                        if (this.Grid.Rows[i].Cells[j].Value == null)
                        {
                            this.Grid.Rows[i].Cells[j].Value = this.Session.CreateProperty(property);
                        }
                        else
                        {
                            this.Grid.Rows[i].Cells[j].Value.Binding = property;
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
                    return null;
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

                    foreach(Model.PropertyType proptype in this.Query.Store.ItemType.SearchPropertyTypes)
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

            List<Model.Item> items = this.Displayed.ToList();

            foreach (Row row in this.Grid.SelectedRows)
            {
                this.Selected.Add(items[row.Index]);
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
