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
    public class Command : Base
    {
        public Control Control { get; private set; }

        public String Name { get; private set; }

        public delegate void OnExecute(object parameter);

        private OnExecute _execute;

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        private Boolean _canExecute;
        public Boolean CanExecute
        {
            get
            {
                return this._canExecute;
            }
            internal set
            {
                if (value != this._canExecute)
                {
                    this._canExecute = value;
                    this.OnCanExecuteChanged();
                }
            }
        }

        public void Execute()
        {
            this.Execute(null);
        }

        public void Execute(object parameter)
        {
            if (this._execute != null)
            {
                this._execute(parameter);
            }
        }

        public Command(Control Control, String Name, OnExecute Execute, Boolean CanExecute)
            :base()
        {
            this.Control = Control;
            this.Name = Name;
            this._execute = Execute;
            this._canExecute = CanExecute;
        }
    }
}

