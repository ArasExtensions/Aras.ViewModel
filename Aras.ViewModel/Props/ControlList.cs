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

namespace Aras.ViewModel.Properties
{
    public class ControlList : Property
    {
        internal override void SetObject(object value)
        {
            if (value == null || value is ObservableList<ViewModel.Control>)
            {
                base.SetObject(value);
            }
            else
            {
                throw new Exceptions.ValueTypeException("List<Aras.ViewModel.Control>");
            }
        }

        public ObservableList<ViewModel.Control> Value
        {
            get
            {
                return (ObservableList<ViewModel.Control>)this.Object;
            }
            set
            {
                this.Object = value;
            }
        }

        public Boolean Contains(ViewModel.Control Control)
        {
            if (Control == null)
            {
                return false;
            }
            else
            {
                if (this.Value == null)
                {
                    return false;
                }
                else
                {
                    return this.Value.Contains(Control);
                }
            }
        }

        public ControlList(ViewModel.Control Control, System.String Name, Boolean Required, Boolean ReadOnly)
            : base(Control, Name, Required, ReadOnly)
        {
            ObservableList<ViewModel.Control> values = new ObservableList<ViewModel.Control>();
            values.ListChanged += values_ListChanged;
            this.SetObject(values);
        }

        void values_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged();
        }
    }
}
