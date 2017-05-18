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

namespace Aras.ViewModel.Cells
{
    public class Integer : Cell
    {
        const System.Int32 DefaultMinValue = System.Int32.MinValue;
        const System.Int32 DefaultMaxValue = System.Int32.MaxValue;

        public override void SetValue(object Value)
        {
            base.SetValue(Value);

            if (Value == null)
            {
                this.Value = null;
            }
            else
            {
                if (Value is System.Int32)
                {
                    this.Value = ((System.Int32)Value).ToString();
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Int32");
                }
            }
        }

        private System.Int32 _minValue;
        [Attributes.Property("MinValue", Attributes.PropertyTypes.Int32, true)]
        public System.Int32 MinValue
        {
            get
            {
                return this._minValue;
            }
            set
            {
                this._minValue = value;
                this.OnPropertyChanged("MinValue");
            }
        }

        private System.Int32 _maxValue;
        [Attributes.Property("MaxValue", Attributes.PropertyTypes.Int32, true)]
        public System.Int32 MaxValue
        {
            get
            {
                return this._maxValue;
            }
            set
            {
                this._maxValue = value;
                this.OnPropertyChanged("MaxValue");
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {
            if (Value == null)
            {
                this.SetValue(null);
            }
            else
            {
                System.Int32 thisvalue = 0;

                if (System.Int32.TryParse(Value, out thisvalue))
                {
                    this.SetValue(thisvalue);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Integer))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Integer");
            }
        }

        internal Integer(Column Column, Row Row)
            :base(Column, Row)
        {
            this._minValue = DefaultMinValue;
            this._maxValue = DefaultMaxValue;
        }
    }
}
