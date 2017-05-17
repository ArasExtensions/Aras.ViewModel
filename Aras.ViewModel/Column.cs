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
    public abstract class Column : Control
    {
        public Grid Grid { get; private set; }

        private String _name;
        [Attributes.Property("Name", Attributes.PropertyTypes.String, true)]
        public String Name
        {
            get
            {
                return this._name;
            }
            private set
            {
                if (this._name == null)
                {
                    if (value != null)
                    {
                        this._name = value;
                        this.OnPropertyChanged("Name");
                    }
                }
                else
                {
                    if (!this._name.Equals(value))
                    {
                        this._name = value;
                        this.OnPropertyChanged("Name");
                    }
                }
            }
        }

        private String _label;
        [Attributes.Property("Label", Attributes.PropertyTypes.String, true)]
        public String Label
        {
            get
            {
                return this._label;
            }
            set
            {
                if (this._label == null)
                {
                    if (value != null)
                    {
                        this._label = value;
                        this.OnPropertyChanged("Label");
                    }
                }
                else
                {
                    if (!this._label.Equals(value))
                    {
                        this._label = value;
                        this.OnPropertyChanged("Label");
                    }
                }
            }
        }

        private Boolean _editable;
        [Attributes.Property("Editable", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Editable
        {
            get
            {
                return this._editable;
            }
            set
            {
                if (!this._editable.Equals(value))
                {
                    this._editable = value;
                    this.OnPropertyChanged("Editable");
                }
            }
        }

        public override string ToString()
        {
            return this.Label;
        }

        internal Column(Grid Grid, String Name, String Label, Int32 Width)
            :base(Grid.Session)
        {
            this.Grid = Grid;
            this.Name = Name;
            this.Label = Label;
            this.Width = Width;
            this._editable = false;
        }

    }
}
