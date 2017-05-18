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
    public class Boolean : Cell
    {
        public override void SetValue(object Value)
        {
            base.SetValue(Value);

            if (Value == null)
            {
                this.Value = "0";
            }
            else
            {
                if (Value is System.Boolean)
                {
                    if ((System.Boolean)Value)
                    {
                        this.Value = "1";
                    }
                    else
                    {
                        this.Value = "0";
                    }
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Boolean");
                }
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {
            if (Value == null)
            {
                this.SetValue(false);
            }
            else
            {
                if (Value.Equals("1"))
                {
                    this.SetValue(true);
                }
                else
                {
                    this.SetValue(false);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Boolean))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Boolean");
            }
        }

        internal Boolean(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
