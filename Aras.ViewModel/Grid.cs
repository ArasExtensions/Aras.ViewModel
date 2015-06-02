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
    public class Grid : Control
    {

        [Attributes.Property("Columns")]
        public Model.ObservableList<Column> Columns { get; private set; }

        public Column AddColumn(String Name, String Label)
        {
            Column col = new Column(this.Session, this, Name, Label);
            this.Columns.Add(col);
            return col;
        }

        [Attributes.Property("Rows")]
        public Model.ObservableList<Row> Rows {get; private set;} 

        public Row AddRow()
        {
            Row row = new Row(this.Session, this);

            foreach(Column col in this.Columns)
            {
                Cell cell = new Cell(this.Session, row, col);
                row.Cells.Add(cell);
            }

            this.Rows.Add(row);

            return row;
        }

        [Attributes.Property("Selected")]
        public Model.ObservableList<Row> Selected { get; private set; }
 
        public Grid(Session Session)
            :base(Session)
        {
            this.Columns = new Model.ObservableList<Column>();
            this.Columns.ListChanged += Columns_ListChanged;
            this.Rows = new Model.ObservableList<Row>();
            this.Rows.ListChanged += Rows_ListChanged;
            this.Selected = new Model.ObservableList<Row>();
            this.Selected.ListChanged += Selected_ListChanged;
        }

        void Selected_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Selected");
        }

        void Rows_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Rows");
        }

        void Columns_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Columns");

            // Clear Rows
            this.Rows.Clear();
        }
    }
}
