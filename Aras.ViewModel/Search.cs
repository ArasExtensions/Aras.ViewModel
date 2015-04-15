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
        const System.Int32 MinPageSize = 5;
        const System.Int32 MaxPageSize = 100;
        const System.Int32 DefaultPageSize = 25;
        const System.Int32 MinPage = 1;
        const System.Int32 MaxPage = Int32.MaxValue;
        const System.Int32 DefaultPage = 1;

        public Model.ItemType ItemType { get; private set; }

        public IEnumerable<Model.PropertyType> PropertyTypes { get; private set; }

        public Properties.Int32 PageSize { get; private set; }

        public Properties.Int32 PageCount { get; private set; }

        public Properties.Int32 Page { get; private set; }

        public Grid GridControl { get; private set; }

        public Properties.Control Grid { get; private set; }

        public Command Refresh { get; private set; }

        private Model.Request.Item _request;
        private Model.Request.Item Request
        {
            get
            {
                if (this._request == null)
                {
                    this._request = this.Session.Create(this.ItemType.Action("get"));
                    
                    foreach (Model.PropertyType proptype in this.PropertyTypes)
                    {
                        this._request.AddSelection(proptype);
                    }

                    this._request.Paging = true;
                    this._request.Page = (int)this.Page.Value;
                    this._request.PageSize = (int)this.PageSize.Value;
                }

                return this._request;
            }
        }

        private async Task OnExecuteRefreshAsync(object parameter)
        {
            this.SetCommandCanExecute("Refresh", false);

            Model.Response.IEnumerable<Model.Response.Item> response = await this.Request.ExecuteAsync();

            // Set number of rows on Grid
            this.GridControl.NoRows = response.Count();

            // Add Items to Grid
            int rowindex = 0;

            foreach(Model.Response.Item item in response)
            {
                Row row = this.GridControl.Row(rowindex);
           
                foreach(Model.PropertyType proptype in this.PropertyTypes)
                {
                    Column column = this.GridControl.Column(proptype.Name);
                    Cell cell = row.Cell(column);
                    cell.SetModelProperty(item.Cache.Property(proptype));
                }

                rowindex++;
            }

            this.SetCommandCanExecute("Refresh", true);
        }

        void Page_ObjectChanged(object sender, EventArgs e)
        {
            this.Request.Page = (int)this.Page.Value;
        }

        void PageSize_ObjectChanged(object sender, EventArgs e)
        {
            this.Request.PageSize = (int)this.PageSize.Value;
        }

        public Search(Aras.Model.Session Session, Model.ItemType ItemType, IEnumerable<Model.PropertyType> PropertyTypes)
            : base(Session)
        {
            this.ItemType = ItemType;
            this.PropertyTypes = PropertyTypes;

            this.PageSize = new Properties.Int32(this, "PageSize", true, false, MinPageSize, MaxPageSize, DefaultPageSize);
            this.PageSize.PropertyChanged += PageSize_ObjectChanged;
            this.RegisterProperty(this.PageSize);
            
            this.PageCount = new Properties.Int32(this, "PageCount", true, true, 0, System.Int32.MaxValue, 0);
            this.RegisterProperty(this.PageCount);
            
            this.Page = new Properties.Int32(this, "Page", true, false, MinPage, MaxPage, DefaultPage);
            this.Page.PropertyChanged += Page_ObjectChanged;
            this.RegisterProperty(this.Page);

            this.GridControl = new Grid(this.Session);
            this.Grid = new Properties.Control(this, "Grid", true, true, this.GridControl);
            this.RegisterProperty(this.Grid);
            
            foreach(Model.PropertyType proptype in this.PropertyTypes)
            {
                this.GridControl.AddColumn(proptype.Name, proptype.Label);
            }

            this.Refresh = new Command(this, "Refresh", this.OnExecuteRefreshAsync, true);
            this.RegisterCommand(this.Refresh);
        }
    }
}
