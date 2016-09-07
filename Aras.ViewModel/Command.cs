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
    public abstract class Command
    {
        public Guid ID { get; private set; }

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        private object _canExecuteLock = new object();
        private Boolean _canExecute;
        public Boolean CanExecute
        {
            get
            {
                lock (this._canExecuteLock)
                {
                    return this._canExecute;
                }
            }
            protected set
            {
                lock (this._canExecuteLock)
                {
                    if (value != this._canExecute)
                    {
                        this._canExecute = value;
                        this.OnCanExecuteChanged();
                    }
                }
            }
        }

        protected abstract void Run(IEnumerable<Control> Parameters);

        public void Execute(IEnumerable<Control> Parameters)
        {
            if (this.CanExecute)
            {
                this.CanExecute = false;
                this.Run(Parameters);
                this.CanExecute = true;
            }
        }

        public void Execute()
        {
            this.Execute(null);
        }

        public Command()
        {
            this.ID = Guid.NewGuid();
        }
    }
}

