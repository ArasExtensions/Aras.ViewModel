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

namespace Aras.ViewModel.Panes
{
    public class TitlePane : ContentPane
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

        private System.Boolean _open;
        [Attributes.Property("Open", Attributes.PropertyTypes.Boolean, false)]
        public System.Boolean Open
        {
            get
            {
                return this._open;
            }
            set
            {
                if (this._open != value)
                {
                    this._open = value;
                    this.OnPropertyChanged("Open");
                }
            }
        }

        public TitlePane(Manager.Session Session)
            : base(Session)
        {
            this._title = null;
            this._open = false;
        }
    }
}
