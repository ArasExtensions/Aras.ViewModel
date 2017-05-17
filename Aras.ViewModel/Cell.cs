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
    [Attributes.ClientControl("Aras.View.Cell")]
    public abstract class Cell : Control
    {
        public Column Column { get; private set; }

        public Row Row { get; private set; }

        private String _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.String, true)]
        public String Value
        {
            get
            {
                return this._value;
            }
            protected set
            {
                if (this._value == null)
                {
                    if (value != null)
                    {
                        this._value = value;
                        this.OnPropertyChanged("Value");
                    }
                }
                else
                {
                    if (!this._value.Equals(value))
                    {
                        this._value = value;
                        this.OnPropertyChanged("Value");
                    }
                }
            }
        }

        [Attributes.Property("UpdateValue", Attributes.PropertyTypes.String, false)]
        public String UpdateValue
        {
            get
            {
                return null;
            }
            set
            {
                this.ProcessUpdateValue(value);        
            }
        }

        protected abstract void ProcessUpdateValue(String Value);

        private Boolean UpdatingBinding;
        public virtual void SetValue(Object Value)
        {
            if (this.Binding != null)
            {
                if (!this.UpdatingBinding)
                {
                    this.UpdatingBinding = true;

                    // Update Binding
                    ((Model.Property)this.Binding).Value = Value;

                    this.UpdatingBinding = false;
                }
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                // Stop Watching for Changes
                ((Model.Property)this.Binding).PropertyChanged -= Binding_PropertyChanged;
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();
        
            if (this.Binding != null)
            {
                // Set Value
                this.SetValue(((Model.Property)this.Binding).Value);
                
                // Watch for Changes
                ((Model.Property)this.Binding).PropertyChanged += Binding_PropertyChanged; 
            }
        }

        private void Binding_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Value":
                    this.SetValue(((Model.Property)sender).Value);
                    break;
                default:

                    break;
            }
        }

        internal Cell(Column Column, Row Row)
            :base(Column.Session)
        {
            this.Column = Column;
            this.Row = Row;
            this.UpdatingBinding = false;
        }

    }
}
