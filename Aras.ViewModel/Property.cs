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
    public abstract class Property : Control
    {
        private Boolean _required;
        [Attributes.Property("Required", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Required
        {
            get
            {
                return this._required;
            }
            set
            {
                if (this._required != value)
                {
                    this._required = value;
                    this.OnPropertyChanged("Required");
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

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Property))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Property");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                // Watch for Property Changes on Binding
                ((Model.Property)this.Binding).PropertyChanged += Property_PropertyChanged;

                // Set Enabled
                this.Enabled = !((Model.Property)this.Binding).ReadOnly;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Property)this.Binding).PropertyChanged -= Property_PropertyChanged;
            }
        }

        protected virtual void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadOnly":
                    this.Enabled = !((Model.Property)sender).ReadOnly;
                    break;
                default:
                    break;
            }
        }

        public Property(Manager.Session Session)
            :base(Session)
        {
            this.Required = false;
        }
    }
}
