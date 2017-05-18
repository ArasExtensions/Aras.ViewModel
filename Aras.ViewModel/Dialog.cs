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

        protected virtual void OnOpen()
        {

        }

        protected virtual void OnClose()
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
