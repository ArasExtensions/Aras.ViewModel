﻿/*  
  Aras.Model provides a .NET cient library for Aras Innovator

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
    [Attributes.ClientControl("Aras.View.Properties.String")]
    public class String : Property
    {
        private const System.Int32 MinLength = 1;
        private const System.Int32 MaxLength = 4000;
        private const System.Int32 DefaultLength = 32;

        private System.Int32 _length;
        [Attributes.Property("Length", Attributes.PropertyTypes.Int32, true)]
        public System.Int32 Length
        {
            get
            {
                return this._length;
            }
            set
            {
                if (!this._length.Equals(value))
                {
                    if (value >= MinLength && value <= MaxLength)
                    {
                        this._length = value;
                        this.OnPropertyChanged("Length");
                    }
                    else
                    {
                        throw new ArgumentException("Length must be between " + MinLength.ToString() + " and " + MaxLength.ToString());
                    }
                }
            }
        }

        private System.String _value;
        [Attributes.Property("Value", Attributes.PropertyTypes.String, false)]
        public System.String Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value == null)
                {
                    if (value != null)
                    {
                        this.SetValue(value);
                    }
                }
                else
                {
                    if (System.String.Compare(this._value, value) != 0)
                    {
                        this.SetValue(value);
                    }
                }
            }
        }

        private void SetValue(System.String Value)
        {
            if (Value == null || Value.Length <= this.Length)
            {
                this._value = Value;
                this.OnPropertyChanged("Value");

                if (this.Binding != null)
                {
                    this.UpdatingBinding = true;

                    if (this.Binding is Model.Properties.String)
                    {
                        ((Model.Properties.String)this.Binding).Value = this.Value;
                    }
                    else
                    {
                        ((Model.Properties.Text)this.Binding).Value = this.Value;
                    }

                    this.UpdatingBinding = false;
                }
            }
            else
            {
                throw new ArgumentException("Length must be no greater than " + this.Length.ToString());
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!((Binding is Model.Properties.String) || (Binding is Model.Properties.Text)))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.String or Aras.Model.Properties.Text");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Properties.String)
                {
                    this.Length = ((Model.Properties.String)this.Binding).Length;
                    this.Value = (System.String)((Model.Properties.String)this.Binding).Value;
                }
                else
                {
                    this.Length = MaxLength;
                    this.Value = (System.String)((Model.Properties.Text)this.Binding).Value;
                }
            }
            else
            {
                this.Value = null;
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

                        if (!this.UpdatingBinding)
                        {
                            if (this.Binding is Model.Properties.String)
                            {
                                this.Value = (System.String)((Model.Properties.String)this.Binding).Value;
                            }
                            else
                            {
                                this.Value = (System.String)((Model.Properties.Text)this.Binding).Value;
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        public String(Manager.Session Session)
            : base(Session)
        {
            this._length = DefaultLength;
        }

        public String(Manager.Session Session, Model.PropertyTypes.String PropertyType)
            : base(Session, PropertyType)
        {
            this._length = DefaultLength;
        }

        public String(Manager.Session Session, Model.PropertyTypes.Text PropertyType)
            : base(Session, PropertyType)
        {
            this._length = MaxLength;
        }
    }
}
