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
    [Attributes.ClientControl("Aras.View.Panes.ContentPane")]
    public class ContentPane : Control
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

        private Control _content;
        [Attributes.Property("Content", Attributes.PropertyTypes.Control, true)]
        public Control Content
        {
            get
            {
                return this._content;
            }
            set
            {
                if (this._content != value)
                {
                    this._content = value;
                    this.OnPropertyChanged("Content");
                }
            }
        }

        public ContentPane(Manager.Session Session)
            :base(Session)
        {
            this._title = null;
            this._content = null;
        }
    }
}
