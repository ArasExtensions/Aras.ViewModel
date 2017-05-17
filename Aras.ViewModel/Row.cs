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
    [Attributes.ClientControl("Aras.View.Row")]
    public class Row : Control
    {
        public Grid Grid { get; private set; }

        public Int32 Index { get; private set; }

        [ViewModel.Attributes.Property("Cells", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Cell> Cells { get; private set; }

        private Cell CreateCell(Column Column, Row Row)
        {
            Cell ret = null;

            switch(Column.GetType().Name)
            {
                case "Boolean":
                    ret = new Cells.Boolean(Column, Row);
                    break;
                case "Decimal":
                    ret = new Cells.Decimal(Column, Row);
                    break;
                case "Float":
                    ret = new Cells.Float(Column, Row);
                    break;
                case "Integer":
                    ret = new Cells.Integer(Column, Row);
                    break;
                case "Item":
                    ret = new Cells.Item(Column, Row);
                    break;
                case "List":
                    ret = new Cells.List(Column, Row);
                    break;
                case "String":
                    ret = new Cells.String(Column, Row);
                    break;
                case "Sequence":
                    ret = new Cells.Sequence(Column, Row);
                    break;
                case "Text":
                    ret = new Cells.Text(Column, Row);
                    break;
                default:
                    throw new Model.Exceptions.ArgumentException("Column Type not implemented: " + Column.GetType().Name);
            }

            // Watch for Changes in Cell
            ret.PropertyChanged += Cell_PropertyChanged;

            return ret;
        }

        private void Cell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("Cells");
        }

        private void UpdateCells()
        {
            for (int i = 0; i < this.Grid.Columns.Count(); i++)
            {
                if (i == this.Cells.Count())
                {
                    this.Cells.Add(this.CreateCell(this.Grid.Columns[i], this));
                }
                else
                {
                    if (!this.Cells[i].Column.Equals(this.Grid.Columns[i]))
                    {
                        this.Cells[i] = this.CreateCell(this.Grid.Columns[i], this);
                    }
                }
            }

            if (this.Cells.Count() > this.Grid.Columns.Count())
            {
                this.Cells.RemoveRange(this.Grid.Columns.Count(), this.Cells.Count() - this.Grid.Columns.Count());
            }
        }

        void Columns_ListChanged(object sender, EventArgs e)
        {
            this.UpdateCells();
        }

        internal Row(Grid Grid, Int32 Index)
            :base(Grid.Session)
        {
            this.Grid = Grid;
            this.Index = Index;
            this.Cells = new Model.ObservableList<Cell>();
            this.UpdateCells();
            this.Grid.Columns.ListChanged += Columns_ListChanged;
        }
    }
}
