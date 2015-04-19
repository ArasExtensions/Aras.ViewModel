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
        public Properties.ControlList Columns { get; private set; }

        public Column AddColumn(String Name, String Label)
        {
            Column column = this.Column(Name);

            if (column == null)
            {
                column = new Column(this, Name, Label);
                this.Columns.Value.Add(column);
            }

            return column;
        }

        public Column Column(String Name)
        {
            foreach(Control control in this.Columns.Value)
            {
                Column column = (Column)control;

                if (column.Name.Value == Name)
                {
                    return column;
                }
            }

            return null;
        }

        public Properties.ControlList Rows { get; private set; }

        public Row Row(int Index)
        {
            return (Row)this.Rows.Value[Index];
        }

        private void checkSeleted()
        {
            if (this.Selected.Value != null)
            {
                if (!this.Rows.Contains(this.Selected.Value))
                {
                    this.Selected.Value = null;
                }
            }
        }

        public Properties.Control Selected { get; private set; }

        public void ClearRows()
        {
            this.Rows.Value.Clear();
            this.checkSeleted();
        }

        public Row AddRow()
        {
            Row row = new Row(this);
            this.Rows.Value.Add(row);
            return row;
        }

        public int NoRows
        {
            get
            {
                return this.Rows.Value.Count();
            }
        }

        public Grid(Model.Session Session)
            :base(Session)
        {
            this.Columns = new Properties.ControlList(this, "Columns", true, false);
            this.RegisterProperty(this.Columns);

            this.Rows = new Properties.ControlList(this, "Rows", true, false);
            this.RegisterProperty(this.Rows);

            this.Selected = new Properties.Control(this, "Selected", false, false, null);
            this.RegisterProperty(this.Selected);
            this.Selected.PropertyChanged += Selected_PropertyChanged;
        }

        void Selected_PropertyChanged(object sender, EventArgs e)
        {
            // Check new Selected Value is a Row
            this.checkSeleted();
        }
    }
}
