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
