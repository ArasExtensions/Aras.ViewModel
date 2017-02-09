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

namespace Aras.ViewModel.Properties
{
    [Attributes.ClientControl("Aras.View.Properties.Date")]
    public class Date : Property
    {

        private System.DateTime? _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.Date, false)]
        public System.DateTime? Value
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

        private void SetValue(System.DateTime? Value)
        {
            this._value = Value;
            this.OnPropertyChanged("Value");

            if (this.Binding != null)
            {
                ((Model.Properties.Date)this.Binding).Value = this.Value;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Date))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Date");
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.Value = (System.DateTime?)((Model.Properties.Date)this.Binding).Value;
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
                        this.Value = (System.DateTime?)((Model.Properties.Date)this.Binding).Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public Date(Manager.Session Session)
            : base(Session)
        {

        }
    }
}
