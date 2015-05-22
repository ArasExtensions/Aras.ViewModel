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
using System.ComponentModel;

namespace Aras.ViewModel
{
    public abstract class Control : IEquatable<Control>, INotifyPropertyChanged
    {
        public Session Session { get; private set; }

        public Guid ID { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String Name)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(Name);
                this.PropertyChanged(this, args);
            }
        }

        protected void OnAllPropertiesChanged()
        {
            if (this.PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(String.Empty);
                this.PropertyChanged(this, args);
            }
        }

        public bool Equals(Control other)
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
                if (obj is Control)
                {
                    return this.Equals((Control)obj);
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

        public Control(Session Session)
        {
            this.Session = Session;
            this.ID = Guid.NewGuid();
        }
    }
}
