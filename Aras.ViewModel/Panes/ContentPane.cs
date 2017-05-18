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

        [Attributes.Property("Region", Attributes.PropertyTypes.Int32, true)]
        public Regions Region { get; set; }

        [Attributes.Property("Splitter", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Splitter { get; set; }

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
            this.Region = Regions.Center;
            this.Splitter = false;
            this._content = null;
        }
    }
}
