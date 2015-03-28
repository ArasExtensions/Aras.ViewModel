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
        private Dictionary<String, Column> _columns;
        public IEnumerable<Column> Columns
        {
            get
            {
                return this._columns.Values;
            }
        }

        public Column AddColumn(String Name, String Label)
        {
            if (!this._columns.ContainsKey(Name))
            {
                this._columns[Name] = new Column(this, Name, Label);
            }

            return this._columns[Name];
        }

        public Column Column(String Name)
        {
            return this._columns[Name];
        }

        private List<Row> _rows;
        public IEnumerable<Row> Rows
        {
            get
            {
                return this._rows;
            }
        }

        public Row Row(int Index)
        {
            return this._rows[Index];
        }

        public void ClearRows()
        {
            this._rows.Clear();
        }

        public Row AddRow()
        {
            Row row = new Row(this);
            this._rows.Add(row);
            return row;
        }

        public Row InsertRow(Row Row)
        {
            if (Row.Grid.Equals(this))
            {
                Row row = new Row(this);
                this._rows.Insert(this._rows.IndexOf(Row), row);
                return row;
            }
            else
            {
                throw new ArgumentException("Row not associated with Grid");
            }
        }

        public int NoRows
        {
            get
            {
                return this._rows.Count();
            }
            set
            {
                if (this._rows.Count() < value)
                {
                    // Need to add some more Rows
                    int diff = value-this._rows.Count;

                    for(int i=0; i<diff; i++)
                    {
                        this.AddRow();
                    }
                }
                else
                {
                    // Need to remove Rows
                    this._rows.RemoveRange(value, this._rows.Count - value);
                }
            }
        }
        public Grid(Model.Session Session)
            :base(Session)
        {
            this._columns = new Dictionary<String, Column>();
            this._rows = new List<Row>();
        }
    }
}
