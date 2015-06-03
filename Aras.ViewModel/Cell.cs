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
    public class Cell : Control
    {
        public Row Row { get; private set; }

        public Column Column { get; private set; }

        private String _value;
        [Attributes.Property("Value")]
        public String Value 
        { 
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");

                    // Update Binding

                    if (this.Binding != null)
                    {
                        this.Binding.ValueString = value;
                    }
                }
            }
        }

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
                        this.Value = null;
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

                    this.Value = this._binding.ValueString;
                }
            }
        }

        void Binding_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Model.Property property = (Model.Property)sender;
            this.Value = property.ValueString;
        }

        internal Cell(Session Session, Row Row, Column Column)
            :base(Session)
        {
            this.Row = Row;
            this.Column = Column;
            this.Value = null;
        }
    }
}
