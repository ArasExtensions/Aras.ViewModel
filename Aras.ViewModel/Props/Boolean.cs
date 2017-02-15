/*  
  Aras.Model provides a .NET cient library for Aras Innovator

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

namespace Aras.ViewModel.Properties
{
    [Attributes.ClientControl("Aras.View.Properties.Boolean")]
    public class Boolean : Property
    {

        private System.Boolean _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Boolean, false)]
        public System.Boolean Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    this.SetValue(value);
                }
            }
        }

        private void SetValue(System.Boolean Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                ((Model.Properties.Boolean)this.Binding).Value = this.Value;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Boolean))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Boolean");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (((Model.Properties.Boolean)this.Binding).Value == null)
                {
                    this.Value = false;
                }
                else
                {
                this.Value = (System.Boolean)((Model.Properties.Boolean)this.Binding).Value;
                }
            }
            else
            {
                this.Value = false;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (this.Binding != null)
            {
                switch (e.PropertyName)
                {
                    case "Value":
                        this.Value = (System.Boolean)((Model.Properties.Boolean)this.Binding).Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public Boolean(Manager.Session Session)
            : base(Session)
        {

        }

        public Boolean(Manager.Session Session, Model.PropertyTypes.Boolean PropertyType)
            : base(Session, PropertyType)
        {
        
        }
    }
}
