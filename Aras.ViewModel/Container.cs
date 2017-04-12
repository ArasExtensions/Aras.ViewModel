/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2016 Processwall Limited.

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
    public abstract class Container : Control
    {
        private System.String _title;
        [Attributes.Property("Title", Attributes.PropertyTypes.String, true)]
        public System.String Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }

        [Attributes.Property("Region", Attributes.PropertyTypes.Int32, true)]
        public Regions Region { get; set; }

        [Attributes.Property("Splitter", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Splitter { get; set; }

        [ViewModel.Attributes.Property("Children", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Control> Children { get; private set; }

        private void Children_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Children");
        }

        public Container(Manager.Session Session)
            :base(Session)
        {
            this.Region = Regions.Center;
            this.Splitter = false;
            this.Children = new Model.ObservableList<Control>();
            this.Children.ListChanged += Children_ListChanged;
        }
    }
}
