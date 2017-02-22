/*  
  Aras.Model provides a .NET cient library for Aras Innovator

  Copyright (C) 2017 Processwall Limited.

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
    [Attributes.ClientControl("Aras.View.Dialog")]
    public class Dialog : Control
    {
        public Boolean _open;
        [ViewModel.Attributes.Property("Open", Aras.ViewModel.Attributes.PropertyTypes.Boolean, false)]
        public Boolean Open
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

                    if (this._open)
                    {
                        this.OnOpen();
                    }
                    else
                    {
                        this.OnClose();
                    }
                }
            }
        }

        public String _title;
        [ViewModel.Attributes.Property("Title", Aras.ViewModel.Attributes.PropertyTypes.String, true)]
        public String Title
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

        protected void OnOpen()
        {

        }

        protected void OnClose()
        {

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

        public Dialog(Control Parent)
            : base(Parent.Session)
        {
            this._title = null;
            this._open = false;
            this._content = null;

            // Add to Parent Dialogs
            Parent.Dialogs.Add(this);
        }
    }
}
