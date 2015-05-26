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
    public class Search : Control
    {
        private const System.Int32 MinPageSize = 5;
        private const System.Int32 MaxPageSize = 100;
        private const System.Int32 DefaultPageSize = 25;
        private const System.Int32 MinPage = 1;
        private const System.Int32 MaxPage = Int32.MaxValue;
        private const System.Int32 DefaultPage = 1;

        public Model.ItemType ItemType { get; private set; }

        public Model.ObservableLists.PropertyType SelectPropertyTypes { get; private set; }

        public Model.ObservableLists.PropertyType GridPropertyTypes { get; private set; }

        public System.Int32 PageSize 
        { 
            get
            {
                return (System.Int32)this.PropertiesCache["PageSize"];
            }
            set
            {
                if (value >= MinPageSize && value <= MaxPageSize)
                {
                    if (!((System.Int32)this.PropertiesCache["PageSize"]).Equals(value))
                    {
                        this.PropertiesCache["PageSize"] = value;
                        this.OnPropertyChanged("PageSize");
                    }
                }
                else
                {
                    throw new ArgumentException("PageSize must be between " + MinPageSize.ToString() + " to " + MaxPageSize.ToString());
                }
            }
        }

        public System.Int32 PageCount 
        { 
            get
            {
                return (System.Int32)this.PropertiesCache["PageCount"];
            }
            private set
            {
                if (!((System.Int32)this.PropertiesCache["PageCount"]).Equals(value))
                {
                    this.PropertiesCache["PageCount"] = value;
                    this.OnPropertyChanged("PageCount");
                }
            }
        }

        public System.Int32 Page 
        { 
            get
            {
                return (System.Int32)this.PropertiesCache["Page"];
            }
            set
            {
                if (!((System.Int32)this.PropertiesCache["Page"]).Equals(value))
                {
                    this.PropertiesCache["Page"] = value;
                    this.OnPropertyChanged("Page");
                }
            }
        }

        public Grid Grid
        {
            get
            {
                return (Grid)this.PropertiesCache["Grid"];
            }
        }

        [Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        public Model.ObservableLists.Item Items { get; private set; }

        public Model.ObservableLists.Item SelectedItems { get; private set; }

        private Model.Requests.Item _request;
        private Model.Requests.Item Request
        {
            get
            {
                if (this._request == null)
                {
                    this._request = this.Session.Model.Request(this.ItemType.Action("get"));

                    foreach (Model.PropertyType proptype in this.SelectPropertyTypes)
                    {
                        this._request.AddSelection(proptype);
                    }

                    this._request.Paging = true;
                    this._request.Page = this.Page;
                    this._request.PageSize = this.PageSize;
                }

                return this._request;
            }
        }

        public Search(Session Session, Model.ItemType ItemType)
            : base(Session)
        {
            this.ItemType = ItemType;
            this.SelectPropertyTypes = new Model.ObservableLists.PropertyType(this.ItemType);
            this.GridPropertyTypes = new Model.ObservableLists.PropertyType(this.ItemType);
            this.GridPropertyTypes.ListChanged += GridPropertyTypes_ListChanged;

            this.Items = new Model.ObservableLists.Item();
            this.Items.ListChanged += Items_ListChanged;
            this.SelectedItems = new Model.ObservableLists.Item();

            this.PropertiesCache["PageSize"] = DefaultPageSize;
            this.PropertiesCache["PageCount"] = 1;
            this.PropertiesCache["Page"] = DefaultPage;

            this.PropertiesCache["Grid"] = new Grid(this.Session);

            this.Refresh = new RefreshCommand(this);
        }

        void Items_ListChanged(object sender, EventArgs e)
        {
            // Ensure Grid has correct number of Rows
            int diff = this.Items.Count() - this.Grid.Rows.Count();

            if (diff > 0)
            {
                for(int i=0; i<diff; i++)
                {
                    this.Grid.AddRow();
                }
            }
            else if (diff < 0)
            {
                int startremove = this.Grid.Rows.Count() - Math.Abs(diff);
                this.Grid.Rows.RemoveRange(startremove, Math.Abs(diff));
            }

            // Update Cells
            for (int rowindex = 0; rowindex < this.Items.Count(); rowindex++)
            {
                Model.Item item = this.Items[rowindex];
                Row row = this.Grid.Rows[rowindex];

                for(int columnindex=0; columnindex<this.GridPropertyTypes.Count(); columnindex++)
                {
                    Model.PropertyType propertytype = this.GridPropertyTypes[columnindex];
                    Cell cell = row.Cells[columnindex];

                    switch(propertytype.GetType().Name)
                    {
                        case "String":
                            
                            if (cell.Value == null || !(cell.Value is Properties.String))
                            {
                                cell.Value = new Properties.String(this.Session, false, true);
                            }

                            break;
                        default:
                            throw new NotImplementedException("PropertyType not implemented: " + propertytype.GetType().Name);
                    }

                    // Binf Model Property to Cell Value
                    cell.Value.Binding = item.Property(propertytype.Name);
                }
            }
        }

        void GridPropertyTypes_ListChanged(object sender, EventArgs e)
        {
            // Check that all Grid Properties are in SelectPropertyTypes
            foreach(Aras.Model.PropertyType proptype in this.GridPropertyTypes)
            {
                if (!this.SelectPropertyTypes.Contains(proptype))
                {
                    throw new ArgumentException("All GridProprtyTypes must be present in SelectPropertyTypes");
                }
            }

            // Add in required Columns
            this.Grid.Columns.Clear();

            foreach(Aras.Model.PropertyType proptype in this.GridPropertyTypes)
            {
                this.Grid.AddColumn(proptype.Name, proptype.Label);
            }
        }

        public class RefreshCommand : Command
        {
            public Search Search { get; private set; }

            public override bool Execute(object parameter)
            {
                Model.Response response = this.Search.Request.Execute();

                this.Search.Items.NotifyListChanged = false;
                this.Search.Items.Clear();

                foreach(Model.Responses.Item item in response.Items)
                {
                    this.Search.Items.Add(item.Cache);
                }

                this.Search.Items.NotifyListChanged = true;

                return true;
            }

            public override async Task<bool> ExecuteAsync(object parameter)
            {
                Model.Response response = await this.Search.Request.ExecuteAsync();

                this.Search.Items.NotifyListChanged = false;
                this.Search.Items.Clear();

                foreach (Model.Responses.Item item in response.Items)
                {
                    this.Search.Items.Add(item.Cache);
                }

                this.Search.Items.NotifyListChanged = true;

                return true;
            }

            internal RefreshCommand(Search Search)
            {
                this.Search = Search;
            }
        }
    }
}
