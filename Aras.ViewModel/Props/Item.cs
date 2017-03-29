/*  
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
    [Attributes.ClientControl("Aras.View.Properties.Item")]
    public class Item : Property
    {
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
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        public Dialogs.Search Dialog { get; private set; }

        [Attributes.Property("Select", Attributes.PropertyTypes.Command, true)]
        [ViewModel.Attributes.Command("Select")]
        public SelectCommand Select { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (Binding != null)
            {
                if (!(Binding is Model.Properties.Item))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Properties.Item");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                // Set Value
                this.SetValue();

                // Set Select Command CanExecute
                this.Select.SetCanExecute(!((Model.Properties.Item)this.Binding).ReadOnly);

                // Watch for changes in Binding
                ((Model.Properties.Item)this.Binding).PropertyChanged += Item_PropertyChanged;
            }
            else
            {
                this.Value = null;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ReadOnly":
                    this.Select.SetCanExecute(!((Model.Properties.Item)this.Binding).ReadOnly);
                    break;
                default:
                    break;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                // Stop watching for changes in current Binding
                ((Model.Properties.Item)this.Binding).PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void SetValue()
        {
            if (((Model.Properties.Item)this.Binding).Value != null)
            {
                this.Value = (System.String)((Model.Item)((Model.Properties.Item)this.Binding).Value).Property("keyed_name").Value;
            }
            else
            {
                this.Value = null;
            }
        }

        private Model.Item _propertyItem;
        public Model.Item PropetyItem
        {
            get
            {
                if (this.Binding != null)
                {
                    return ((Model.Properties.Item)this.Binding).Item;
                }
                else
                {
                    return this._propertyItem;
                }
            }
            set
            {
                if (this.Binding == null)
                {
                    this._propertyItem = value;

                    if (this._propertyItem != null)
                    {
                        this.Value = (System.String)this._propertyItem.Property("keyed_name").Value;
                    }
                    else
                    {
                        this.Value = null;
                    }
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Not able to set PropertyItem when Binding is set");
                }
            }
        }

        protected override void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.Property_PropertyChanged(sender, e);

            if (this.Binding != null)
            {
                switch (e.PropertyName)
                {
                    case "Value":

                        this.SetValue();
                    
                        break;
                    default:
                        break;
                }
            }
        }

        private void SelectValue()
        {
            if (this.Dialog == null)
            {
                // Create Search Dialog
                this.Dialog = new Dialogs.Search(this, this.Session.Model.Store(((Model.PropertyTypes.Item)this.PropertyType).ValueType));

                // Watch for changes in selection
                this.Dialog.Grid.Selected.ListChanged += Selected_ListChanged;
            }

            // Open Search Dialog
            this.Dialog.Open = true;
        }

        void Selected_ListChanged(object sender, EventArgs e)
        {
            if (this.Binding != null)
            {
                this.UpdatingBinding = true;

                if (this.Dialog.Grid.Selected.Count() > 0)
                {
                    ((Model.Properties.Item)this.Binding).Value = this.Dialog.Grid.Selected.First();
                }
                else
                {
                    ((Model.Properties.Item)this.Binding).Value = null;
                }

                this.UpdatingBinding = false;
            }
            else
            {
                if (this.Dialog.Grid.Selected.Count() > 0)
                {
                    this.PropetyItem = this.Dialog.Grid.Selected.First();
                }
                else
                {
                    this.PropetyItem = null;
                }
            }

            // Close Dialog
            this.Dialog.Open = false;
        }

        private void Control_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Enabled":

                    if (this.Binding == null)
                    {
                        this.Select.SetCanExecute(this.Enabled);
                    }

                    break;
                default:
                    break;
            }
        }

        public Item(Manager.Session Session)
            : base(Session)
        {
            this.Dialog = null;
            this.Select = new SelectCommand(this);
            this.PropertyChanged += Control_PropertyChanged;
        }

        public Item(Manager.Session Session, Model.PropertyTypes.Item PropertyType)
            : base(Session, PropertyType)
        {
            this.Dialog = null;
            this.Select = new SelectCommand(this);
            this.PropertyChanged += Control_PropertyChanged;
        }

        public class SelectCommand : Aras.ViewModel.Command
        {
            internal void SetCanExecute(System.Boolean Value)
            {
                this.CanExecute = Value;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Item)this.Control).SelectValue();
                this.CanExecute = true;
            }

            internal SelectCommand(Control Control)
                : base(Control)
            {
                this.CanExecute = false;
            }
        }
    }
}
