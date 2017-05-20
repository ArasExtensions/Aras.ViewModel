/*  
  Copyright 2017 Processwall Limited

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Web:     http://www.processwall.com
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel
{
    [Attributes.ClientControl("Aras.View.Grid")]
    public class Grid : Control
    {
        [Attributes.Property("Region", Attributes.PropertyTypes.Int32, true)]
        public Regions Region { get; set; }

        [Attributes.Property("Splitter", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Splitter { get; set; }

        [ViewModel.Attributes.Property("ShowHeader", Aras.ViewModel.Attributes.PropertyTypes.Boolean, true)]
        public Boolean ShowHeader { get; set; }

        [ViewModel.Attributes.Property("Select", Aras.ViewModel.Attributes.PropertyTypes.Command, true)]
        [ViewModel.Attributes.Command("Select")]
        public SelectCommand Select { get; private set; }

        [ViewModel.Attributes.Property("Columns", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Column> Columns { get; private set; }

        [ViewModel.Attributes.Property("Rows", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Row> Rows { get; private set; }

        public Model.ObservableList<Row> SelectedRows { get; private set; }

        public Column Column(String Name)
        {
            foreach(Column col in this.Columns)
            {
                if (col.Name.Equals(Name))
                {
                    return col;
                }
            }

            throw new Model.Exceptions.ArgumentException("Invalid Column Name");
        }

        public Column AddColumn(Model.PropertyType PropertyType, Model.Query Query)
        {
            Column ret = null;

            switch (PropertyType.GetType().Name)
            {
                case "Boolean":
                    ret = this.AddBooleanColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Date":
                    ret = this.AddDateColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Decimal":
                    ret = this.AddDecimalColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Federated":
                    ret = this.AddFederatedColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Integer":
                    ret = this.AddIntegerColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Item":
                    ret = this.AddItemColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth, Query);
                    break;
                case "List":
                    ret = this.AddListColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Sequence":
                    ret = this.AddSequenceColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "String":
                    ret = this.AddStringColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                case "Text":
                    ret = this.AddTextColumn(PropertyType.Name, PropertyType.Label, PropertyType.ColumnWidth);
                    break;
                default:
                    throw new Model.Exceptions.ArgumentException("PropertyType not implemented: " + PropertyType.GetType().Name);
            }

            return ret;
        }

        public Column AddColumn(Model.PropertyType PropertyType)
        {
            return this.AddColumn(PropertyType, null);
        }

        private void Column_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("Columns");
        }

        public Columns.Integer AddIntegerColumn(String Name, String Label, Int32 Width)
        {
            Columns.Integer col = new Columns.Integer(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Date AddDateColumn(String Name, String Label, Int32 Width)
        {
            Columns.Date col = new Columns.Date(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Federated AddFederatedColumn(String Name, String Label, Int32 Width)
        {
            Columns.Federated col = new Columns.Federated(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.String AddStringColumn(String Name, String Label, Int32 Width)
        {
            Columns.String col = new Columns.String(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Sequence AddSequenceColumn(String Name, String Label, Int32 Width)
        {
            Columns.Sequence col = new Columns.Sequence(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Item AddItemColumn(String Name, String Label, Int32 Width, Model.Query Query)
        {
            Columns.Item col = new Columns.Item(this, Name, Label, Width, Query);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.List AddListColumn(String Name, String Label, Int32 Width)
        {
            Columns.List col = new Columns.List(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Boolean AddBooleanColumn(String Name, String Label, Int32 Width)
        {
            Columns.Boolean col = new Columns.Boolean(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Decimal AddDecimalColumn(String Name, String Label, Int32 Width)
        {
            Columns.Decimal col = new Columns.Decimal(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Text AddTextColumn(String Name, String Label, Int32 Width)
        {
            Columns.Text col = new Columns.Text(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        public Columns.Float AddFloatColumn(String Name, String Label, Int32 Width)
        {
            Columns.Float col = new Columns.Float(this, Name, Label, Width);
            col.PropertyChanged += Column_PropertyChanged;
            this.Columns.Add(col);
            return col;
        }

        private Boolean _allowSelect;
        public Boolean AllowSelect
        {
            get
            {
                return this._allowSelect;
            }
            set
            {
                this._allowSelect = value;
                this.Select.UpdateCanExecute(value);
            }
        }

        private List<Row> RowCache;

        private Row AddRow()
        {
            if (this.RowCache.Count() > this.Rows.Count())
            {
                // Use Row from Cache
                Row row = this.RowCache[this.Rows.Count()];
                this.Rows.Add(row);
                
                // Queue Cached Row
                this.Session.QueueControlRecursive(row);

                return row;
            }
            else
            {
                // Create new Row
                Row row = new Row(this, this.RowCache.Count());
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

        public Grid(Manager.Session Session)
            :base(Session)
        {
            this._allowSelect = false;
            this.ShowHeader = true;
            this.Region = Regions.Center;
            this.Splitter = false;
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
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (((Grid)this.Control).AllowSelect)
                {
                    List<Row> newselection = new List<Row>();

                    if (Parameters != null)
                    {
                        foreach (Control row in Parameters)
                        {
                            if (row is Row && ((Grid)this.Control).Rows.Contains((Row)row))
                            {
                                newselection.Add((Row)row);
                            }
                        }
                    }

                    // Replace current selection
                    ((Grid)this.Control).SelectedRows.Replace(newselection);

                    // Set to Execute
                    this.CanExecute = true;
                }
            }

            internal SelectCommand(Grid Grid)
                :base(Grid)
            {
                this.CanExecute = ((Grid)this.Control).AllowSelect;
            }
        }

    }
}
