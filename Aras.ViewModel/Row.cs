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
    public class Row : Control
    {
        public Grid Grid { get; private set; }

        public Properties.ControlList Cells { get; private set; }

        private void AddCells()
        {
            foreach(Column column in Grid.Columns.Value)
            {
                this.Cells.Value.Add(new Cell(column, this));
            }
        }

        public Cell Cell(Column Column)
        {
            foreach(Cell cell in this.Cells.Value)
            {
                if (cell.Column.Equals(Column))
                {
                    return cell;
                }
            }

            return null;
        }

        internal Row(Grid Grid)
            :base(Grid.Session)
        {
            this.Grid = Grid;

            this.Cells = new Properties.ControlList(this, "Cells", true, false);
            this.RegisterProperty(this.Cells);

            this.AddCells();
        }
    }
}
