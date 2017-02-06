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
    public abstract class Search : Containers.BorderContainer, IToolbarProvider
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

        private Containers.Toolbar _toolbar;
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);
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

                    this._toolbar.Children.NotifyListChanged = true;
                }

                return this._toolbar;
            }
        }

        protected ViewModel.Grid Grid { get; private set; }

        [ViewModel.Attributes.Command("NextPage")]
        public NextPageCommand NextPage { get; private set; }

        [ViewModel.Attributes.Command("PreviousPage")]
        public PreviousPageCommand PreviousPage { get; private set; }

        public Properties.String QueryString { get; private set; }

        public Properties.Integer PageSize { get; private set; }

        public Properties.Integer Page { get; protected set; }

        public Properties.Integer NoPages { get; protected set; }

        protected abstract void LoadColumns();

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Load Columns
            this.LoadColumns();

            // Load Rows
            this.RefreshControl();
        }

        private void PageSize_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                this.RefreshControl();
            }
        }

        private void QueryString_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                this.RefreshControl();
            }
        }

        public Search(Manager.Session Session)
            : base(Session)
        {
            this._propertyNames = new List<String>();

            // Create Page
            this.Page = new Properties.Integer(this.Session);
            this.Page.Value = 1;

            // Create No Pages
            this.NoPages = new Properties.Integer(this.Session);
            this.NoPages.Value = 0;

            // Create Grid
            this.Grid = new Grid(this.Session);
            this.Grid.Width = this.Width;
            this.Children.Add(this.Grid);

            // Create Commands
            this.NextPage = new NextPageCommand(this);
            this.PreviousPage = new PreviousPageCommand(this);

            // Create Query String
            this.QueryString = new Properties.String(this.Session);
            this.QueryString.Enabled = true;
            this.QueryString.Tooltip = "Search String";
            this.QueryString.PropertyChanged += QueryString_PropertyChanged;

            // Create Page Size
            this.PageSize = new Properties.Integer(this.Session);
            this.PageSize.Tooltip = "Page Size";
            this.PageSize.Width = 40;
            this.PageSize.Enabled = true;
            this.PageSize.MinValue = 5;
            this.PageSize.MaxValue = 100;
            this.PageSize.Value = 25;
            this.PageSize.PropertyChanged += PageSize_PropertyChanged;
        }


        public class NextPageCommand : Aras.ViewModel.Command
        {
            public Search Search { get; private set; }

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
            {
                this.Search = Search;
                this.Refesh();
            }
        }

        public class PreviousPageCommand : Aras.ViewModel.Command
        {
            public Search Search { get; private set; }

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
            {
                this.Search = Search;
                this.Refesh();
            }
        }
    }
}
