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
    public abstract class Command : IEquatable<Command>
    {
        public Control Control { get; private set; }

        public Guid ID { get; private set; }

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        private readonly object _canExecuteLock = new object();
        private volatile Boolean _canExecute;
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
            }
        }

        public void Execute()
        {
            this.Execute(null);
        }

        public bool Equals(Command other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.ID.Equals(other.ID);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj is Command)
                {
                    return this.ID.Equals(((Command)obj).ID);
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public Command(Control Control)
        {
            this.Control = Control;
            this.ID = Guid.NewGuid();
            this.CanExecute = false;

            // Add Command to Cache
            this.Control.Session.CacheCommand(this);
        }
    }
}

