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

namespace Aras.ViewModel
{
    [Attributes.ClientControl("Aras.View.Containers.BorderContainer")]
    public abstract class Search<T> : Containers.BorderContainer where T : Model.Item
    {
        private List<String> _propertyNames;
        
        public IEnumerable<String> PropertyNames
        {
            get
            {
                return this._propertyNames;
            }
        }

        public void SetPropertyNames(String Names)
        {
            this._propertyNames.Clear();
            this.AddToPropertyNames(Names);
        }

        public void AddToPropertyNames(String Names)
        {
            String[] parts = Names.Split(',');

            foreach (String name in parts)
            {
                if (!this._propertyNames.Contains(name))
                {
                    this._propertyNames.Add(name);
                }
            }

            // Load Columns
            this.LoadColumns();

            // Refresh Control
            this.RefreshControl();
        }

        public Model.ObservableList<T> Selected { get; private set; }

        [ViewModel.Attributes.Command("NextPage")]
        public NextPageCommand NextPage { get; private set; }

        [ViewModel.Attributes.Command("PreviousPage")]
        public PreviousPageCommand PreviousPage { get; private set; }

        protected Properties.Integer Page { get; private set; }

        protected Properties.Integer PageSize { get; private set; }

        private System.Int32 NoPages { get; set; }

        private void ProcessQueryString()
        {
            if (String.IsNullOrEmpty(this.QueryString.Value))
            {
                this.Query.Condition = null;
            }
            else
            {
                if (this.PropertyTypes.Count() == 1)
                {
                    this.Query.Condition = Aras.Conditions.Like(this.PropertyTypes[0].Name, this.QueryString);
                }
                else
                {
                    this.Query.Condition = Aras.Conditions.Or(Aras.Conditions.Like(this.PropertyTypes[0].Name, this.QueryString), Aras.Conditions.Like(this.PropertyTypes[1].Name, this.QueryString));

                    if (this.PropertyTypes.Count() > 2)
                    {
                        for(int i=2; i<this.PropertyTypes.Count(); i++)
                        {
                            ((Model.Conditions.Or)this.Query.Condition).Add(Aras.Conditions.Like(this.PropertyTypes[i].Name, this.QueryString));
                        }
                    }
                }
            }
        }

        private Properties.String QueryString { get; set; }

        private Containers.ToolBar ToolBar { get; set; }

        private Grid Grid { get; set; }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Load Columns
            this.LoadColumns();

            // Load Rows
            this.RefreshControl();
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            // Refresh Query
            if (this.Query != null)
            {
                this.Query.Refresh();
                this.NoPages = this.Query.NoPages;
            }
            else
            {
                this.NoPages = 0;
            }

            // Load Grid
            this.LoadRows();

            // Refresh Buttons
            this.NextPage.Refesh();
            this.PreviousPage.Refesh();
        }

        private List<Model.PropertyType> PropertyTypes;

        private void LoadColumns()
        {
            if ((this.Binding != null) && (this.Binding is Model.Store<T>))
            {
                this.Grid.Columns.Clear();

                // Build List of PropertyTypes
                this.PropertyTypes = new List<Model.PropertyType>();

                foreach(String propertyname in this.PropertyNames)
                {
                    if (((Model.Store<T>)this.Binding).ItemType.HasPropertyType(propertyname))
                    {
                        this.PropertyTypes.Add(((Model.Store<T>)this.Binding).ItemType.PropertyType(propertyname));
                    }
                }

                foreach(Model.PropertyType proptype in this.PropertyTypes)
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


        protected abstract Model.Query<T> Query { get; }

        private void LoadRows()
        {
            if (this.Query != null)
            {
                this.Grid.NoRows = this.Query.Count();

                for(int i=0; i<this.Grid.NoRows; i++)
                {
                    T item = this.Query[i];

                    for(int j=0; j<this.PropertyTypes.Count(); j++)
                    {
                        Model.PropertyType proptype = this.PropertyTypes[j];
                        Model.Property property = item.Property(proptype);

                        if (this.Grid.Rows[i].Cells[j].Value == null)
                        {
                            switch(property.GetType().Name)
                            {
                                case "String":
                                    this.Grid.Rows[i].Cells[j].Value = new Properties.String();
                                    break;
                                case "Integer":
                                    this.Grid.Rows[i].Cells[j].Value = new Properties.Integer();
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

        public Search()
            :base()
        {
            // Create Lists
            this._propertyNames = new List<String>();
            this.Selected = new Model.ObservableList<T>();

            // Create Page
            this.Page = new Properties.Integer();

            // Create PageSize
            this.PageSize = new Properties.Integer();
            this.PageSize.MinValue = 5;
            this.PageSize.MaxValue = 100;
            this.PageSize.Tooltip = "Page Size";

            // Create ToolBar
            this.ToolBar = new Containers.ToolBar();
            this.ToolBar.Region = Regions.Top;
            this.Children.Add(this.ToolBar);

            // Create Grid
            this.Grid = new Grid();
            this.Grid.SelectedRows.ListChanged += SelectedRows_ListChanged;
            this.Children.Add(this.Grid);

            // Create Search Button
            Button searchbutton = new Button();
            searchbutton.Icon = "Search";
            searchbutton.Tooltip = "Search";
            searchbutton.Binding = this.Refresh;
            this.ToolBar.Children.Add(searchbutton);

            // Add Seperator
            this.ToolBar.Children.Add(new ToolBarSeparator());

            // Add PageSize
            this.ToolBar.Children.Add(this.PageSize);

            // Create Next Page Commands
            this.NextPage = new NextPageCommand(this);

            // Create Next Page Button
            Button nextpage = new Button();
            nextpage.Binding = this.NextPage;
            nextpage.Icon = "NextPage";
            nextpage.Tooltip = "Next Page";
            this.ToolBar.Children.Add(nextpage);

            // Create Previous Page Command
            this.PreviousPage = new PreviousPageCommand(this);

            // Create Previous Page Button
            Button prevpage = new Button();
            prevpage.Binding = this.PreviousPage;
            prevpage.Icon = "PreviousPage";
            prevpage.Tooltip = "Previous Page";
            this.ToolBar.Children.Add(prevpage);

            // Add Seperator
            this.ToolBar.Children.Add(new ToolBarSeparator());

            // Add Query String
            this.QueryString = new Properties.String();
            this.QueryString.Value = null;
            this.QueryString.Tooltip = "Search String";
            this.ToolBar.Children.Add(this.QueryString);
        }

        void SelectedRows_ListChanged(object sender, EventArgs e)
        {
            this.Selected.NotifyListChanged = false;
            this.Selected.Clear();

            if (this.Query != null)
            {
                Model.ObservableList<Row> SelectedRows = (Model.ObservableList<Row>)sender;

                foreach (Row row in SelectedRows)
                {
                    Int32 rowindex = this.Grid.Rows.IndexOf(row);

                    if (rowindex >= 0 && rowindex < this.Query.Count())
                    {
                        this.Selected.Add(this.Query[rowindex]);
                    }
                }
            }

            this.Selected.NotifyListChanged = true;
        }

        public class NextPageCommand : Aras.ViewModel.Command
        {
            public Search<T> Search { get; private set; }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Search.Page.Value < this.Search.NoPages)
                {
                    this.Search.Page.Value = this.Search.Page.Value + 1;
                }
            }

            internal void Refesh()
            {
                if (this.Search.Page.Value < this.Search.NoPages)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            
            }

            internal NextPageCommand(Search<T> Search)
            {
                this.Search = Search;
                this.Refesh();
            }
        }

        public class PreviousPageCommand : Aras.ViewModel.Command
        {
            public Search<T> Search { get; private set; }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Search.Page.Value > 1)
                {
                    this.Search.Page.Value = this.Search.Page.Value - 1;
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

            internal PreviousPageCommand(Search<T> Search)
            {
                this.Search = Search;
                this.Refesh();
            }
        }

    }
}
