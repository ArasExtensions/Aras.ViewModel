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
        [ViewModel.Attributes.Command("Select")]
        public SelectCommand Select { get; private set; }

        [ViewModel.Attributes.Property("Columns", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Column> Columns { get; private set; }

        [ViewModel.Attributes.Property("Rows", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Row> Rows { get; private set; }

        public Model.ObservableList<Row> SelectedRows { get; private set; }

        public Column AddColumn(String Name, String Label)
        {
            Column col = new Column(this, Name, Label);
            this.Columns.Add(col);
            return col;
        }

        public Boolean AllowSelect
        {
            get
            {
                return this.Select.CanExecute;
            }
            set
            {
                this.Select.UpdateCanExecute(value);
            }
        }

        private List<Row> RowCache;

        private Row AddRow()
        {
            if (this.RowCache.Count() > this.Rows.Count())
            {
                // Use Row from Cache and clear values
                Row row = this.RowCache[this.Rows.Count()];
                this.Rows.Add(row);
                return row;
            }
            else
            {
                // Create new Row
                Row row = new Row(this);
                this.Rows.Add(row);
                this.RowCache.Add(row);
                return row;
            }
        }

        public System.Int32 NoRows
        {
            get
            {
                return this.Rows.Count();
            }
            set
            {
                if (value > -1)
                {
                    if (this.Rows.Count() > value)
                    {
                        this.Rows.RemoveRange(value, (this.Rows.Count() - value));
                    }
                    else if (this.Rows.Count() < value)
                    {
                        this.Rows.NotifyListChanged = false;

                        Int32 notoadd = value - this.Rows.Count();
                        
                        for(int i=0; i<notoadd; i++)
                        {
                            this.AddRow();
                        }

                        this.Rows.NotifyListChanged = true;
                    }
                }
            }
        }

        void Rows_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Rows");
        }

        void Columns_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Columns");
        }

        void SelectedRows_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("SelectedRows");
        }

        public Grid()
            :base()
        {
            this.Select = new SelectCommand(this);
            this.Columns = new Model.ObservableList<Column>();
            this.Columns.ListChanged += Columns_ListChanged;
            this.Rows = new Model.ObservableList<Row>();
            this.Rows.ListChanged += Rows_ListChanged;
            this.SelectedRows = new Model.ObservableList<Row>();
            this.SelectedRows.ListChanged += SelectedRows_ListChanged;
            this.RowCache = new List<Row>();
        }

        public class SelectCommand : Aras.ViewModel.Command
        {
            public Grid Grid { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Grid.SelectedRows.NotifyListChanged = false;
                this.Grid.SelectedRows.Clear();

                if (Parameters != null)
                {
                    foreach (Control row in Parameters)
                    {
                        if (row is Row && this.Grid.Rows.Contains((Row)row))
                        {
                            this.Grid.SelectedRows.Add((Row)row);
                        }
                    }
                }

                this.Grid.SelectedRows.NotifyListChanged = true;
            }

            internal SelectCommand(Grid Grid)
            {
                this.Grid = Grid;
                this.CanExecute = true;
            }
        }

    }
}
