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
    public class Search : Containers.BorderContainer, IToolbarProvider
    {
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

        private Model.Condition Condition(Model.PropertyType PropertyType)
        {
            switch (PropertyType.GetType().Name)
            {
                case "String":
                case "Sequence":
                case "Text":
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
                case "Decimal":
                    System.Decimal decimalvalue = 0;

                    if (System.Decimal.TryParse(this.QueryString.Value, out decimalvalue))
                    {
                        return Aras.Conditions.Eq(PropertyType.Name, decimalvalue);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
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

        private Containers.Toolbar _toolbar;
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);

                    // Stop Notification
                    this._toolbar.Children.NotifyListChanged = false;

                    // Add Search Button
                    Button searchbutton = new Button(this.Session);
                    searchbutton.Icon = "Search";
                    searchbutton.Tooltip = "Search";
                    this._toolbar.Children.Add(searchbutton);
                    searchbutton.Command = this.Refresh;

                    // Add Page Size
                    this._toolbar.Children.Add(this.PageSize);

                    // Add Next Page Button
                    Button nextbutton = new Button(this.Session);
                    nextbutton.Icon = "NextPage";
                    nextbutton.Tooltip = "Next Page";
                    this._toolbar.Children.Add(nextbutton);
                    nextbutton.Command = this.NextPage;

                    // Add Previous Page Button
                    Button previousbutton = new Button(this.Session);
                    previousbutton.Icon = "PreviousPage";
                    previousbutton.Tooltip = "Previous Page";
                    this._toolbar.Children.Add(previousbutton);
                    previousbutton.Command = this.PreviousPage;

                    // Add Query String
                    this._toolbar.Children.Add(this.QueryString);

                    // Start Notification
                    this._toolbar.Children.NotifyListChanged = true;
                }

                return this._toolbar;
            }
        }

        protected ViewModel.Grid Grid { get; private set; }

        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("NextPage")]
        public NextPageCommand NextPage { get; private set; }

        [ViewModel.Attributes.Command("PreviousPage")]
        public PreviousPageCommand PreviousPage { get; private set; }

        public Properties.String QueryString { get; private set; }

        public Properties.Integers.Spinner PageSize { get; private set; }

        public Properties.Integer Page { get; protected set; }

        public Properties.Integer NoPages { get; protected set; }

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

        protected void LoadColumns()
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

            // Load Columns
            this.LoadColumns();

            // Load Rows
            this.RefreshControl();
        }

        private void PageSize_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // Set Page to 1
                this.Page.Value = 1;

                this.RefreshControl();
            }
        }

        private void QueryString_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // Set Page to 1
                this.Page.Value = 1;

                this.RefreshControl();
            }
        }

        protected void RefreshControl()
        {
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

                    foreach (Model.PropertyType proptype in this.Query.Store.ItemType.SearchPropertyTypes)
                    {
                        Model.Condition condition = this.Condition(proptype);

                        if (condition != null)
                        {
                            conditions.Add(condition);
                        }
                    }

                    if (conditions.Count() > 0)
                    {
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
                    else
                    {
                        this.Query.Condition = null;
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

        public Search(Manager.Session Session)
            : base(Session)
        {           
            // Create Page
            this.Page = new Properties.Integer(this.Session);
            this.Page.Value = 1;

            // Create No Pages
            this.NoPages = new Properties.Integer(this.Session);
            this.NoPages.Value = 0;

            // Create Grid
            this.Grid = new Grid(this.Session);
            this.Grid.AllowSelect = true;
            this.Grid.Width = this.Width;
            this.Children.Add(this.Grid);
            this.Grid.SelectedRows.ListChanged += SelectedRows_ListChanged;

            // Create Selected
            this.Selected = new Model.ObservableList<Model.Item>();

            // Create Commands
            this.Refresh = new RefreshCommand(this);
            this.NextPage = new NextPageCommand(this);
            this.PreviousPage = new PreviousPageCommand(this);

            // Create Query String
            this.QueryString = new Properties.String(this.Session);
            this.QueryString.Enabled = true;
            this.QueryString.IntermediateChanges = true;
            this.QueryString.Tooltip = "Search String";
            this.QueryString.PropertyChanged += QueryString_PropertyChanged;

            // Create Page Size
            this.PageSize = new Properties.Integers.Spinner(this.Session);
            this.PageSize.Tooltip = "Page Size";
            this.PageSize.Width = 40;
            this.PageSize.Enabled = true;
            this.PageSize.MinValue = 5;
            this.PageSize.MaxValue = 100;
            this.PageSize.Value = 25;
            this.PageSize.PropertyChanged += PageSize_PropertyChanged;
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Search)this.Control).RefreshControl();
                this.CanExecute = true;
            }

            internal RefreshCommand(Control Control)
                : base(Control)
            {
                this.CanExecute = true;
            }
        }

        public class NextPageCommand : Aras.ViewModel.Command
        {
            public Search Search
            {
                get
                {
                    return ((Search)this.Control);
                }
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Search.Page.Value < this.Search.NoPages.Value)
                {
                    this.Search.Page.Value = this.Search.Page.Value + 1;
                    this.Search.RefreshControl();
                }
            }

            internal void Refesh()
            {
                if (this.Search.Page.Value < this.Search.NoPages.Value)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            internal NextPageCommand(Search Search)
                :base(Search)
            {
                this.Refesh();
            }
        }

        public class PreviousPageCommand : Aras.ViewModel.Command
        {
            public Search Search
            {
                get
                {
                    return ((Search)this.Control);
                }
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Search.Page.Value > 1)
                {
                    this.Search.Page.Value = this.Search.Page.Value - 1;
                    this.Search.RefreshControl();
                }
            }

            internal void Refesh()
            {
                if (this.Search.Page.Value > 1)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            internal PreviousPageCommand(Search Search)
                :base(Search)
            {
                this.Refesh();
            }
        }
    }
}
