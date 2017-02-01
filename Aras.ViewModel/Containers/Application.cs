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

namespace Aras.ViewModel.Containers
{
    [Attributes.ClientControl("Aras.View.Containers.Application")]
    public abstract class Application : Containers.BorderContainer
    {

        private System.String _name;
        [Attributes.Property("Name", Attributes.PropertyTypes.String, true)]
        public System.String Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        private System.String _label;
        [Attributes.Property("Label", Attributes.PropertyTypes.String, true)]
        public System.String Label
        {
            get
            {
                return this._label;
            }
            set
            {
                if (this._label != value)
                {
                    this._label = value;
                    this.OnPropertyChanged("Label");
                }
            }
        }

        private System.String _icon;
        [Attributes.Property("Icon", Attributes.PropertyTypes.String, true)]
        public System.String Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                if (this._icon != value)
                {
                    this._icon = value;
                    this.OnPropertyChanged("Icon");
                }
            }
        }

        protected Model.Session Session { get; private set; }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Session)
                {
                    this.Session = (Model.Session)this.Binding;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Application binding must be of type Aras.Model.Session");
                }
            }
            else
            {
                this.Session = null;
            }
        }

        public Application()
            :base()
        {

        }
    }
}
