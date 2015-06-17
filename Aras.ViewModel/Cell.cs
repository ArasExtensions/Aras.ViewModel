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
    public abstract class Cell : Control
    {
        public Row Row { get; private set; }

        public Column Column { get; private set; }

        private object _object;
        public virtual object Object
        {
            get
            {
                return this._object;
            }
            set
            {
                if (value == null)
                {
                    if (this._object != null)
                    {
                        this._object = value;
                        this.OnPropertyChanged("Value");

                        if (this.Binding != null)
                        {
                            this.Binding.Object = this._object;
                        }
                    }
                }
                else
                {
                    if (!value.Equals(this._object))
                    {
                        this._object = value;
                        this.OnPropertyChanged("Value");

                        if (this.Binding != null)
                        {
                            this.Binding.Object = this._object;
                        }
                    }
                }
            }
        }

        public abstract String ValueString { get; set; }

        private Model.Property _binding;
        public Model.Property Binding
        {
            get
            {
                return this._binding;
            }
            set
            {
                if (value == null)
                {
                    if (this._binding != null)
                    {
                        this._binding.PropertyChanged -= Binding_PropertyChanged;
                        this._binding = null;
                        this.Object = null;
                    }
                }
                else
                {
                    if (this._binding == null)
                    {
                        this._binding = value;
                        this._binding.PropertyChanged += Binding_PropertyChanged;
                    }
                    else
                    {
                        if(!this._binding.Equals(value))
                        {
                            this._binding.PropertyChanged -= Binding_PropertyChanged;
                            this._binding = value;
                            this._binding.PropertyChanged += Binding_PropertyChanged;
                        }
                    }

                    this.Object = this._binding.Object;
                }
            }
        }

        void Binding_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Model.Property property = (Model.Property)sender;
            this.Object = property.Object;
        }

        internal Cell(Column Column, Row Row)
            :base(Column.Session)
        {
            this.Row = Row;
            this.Column = Column;
        }
    }
}
