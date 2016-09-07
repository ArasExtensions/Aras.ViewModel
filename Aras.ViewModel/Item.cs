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
    public class Item : Control
    {
        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        public virtual void SetBinding(Model.Session Sesison, String ID)
        {
            throw new NotImplementedException();
        }

        protected Model.Transaction ModelTransaction { get; private set; }

        protected Model.Session ModelSession
        {
            get
            {
                if (this.ModelItem != null)
                {
                    return this.ModelItem.Session;
                }
                else
                {
                    return null;
                }
            }
        }

        protected Model.Item ModelItem
        {
            get
            {
                return (Model.Item)this.Binding;
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Item))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.ModelItem.Locked(true))
            {
                // Item is Locked by User set to Edit
                this.EditItem();
            }
            else
            {
                this.ModelTransaction = null;
            }
        }

        private void SetCommandsCanExecute()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction == null)
                {
                    this.Edit.UpdateCanExecute(true);
                    this.Save.UpdateCanExecute(false);
                    this.Undo.UpdateCanExecute(false);
                }
                else
                {
                    this.Edit.UpdateCanExecute(false);
                    this.Save.UpdateCanExecute(true);
                    this.Undo.UpdateCanExecute(true);
                }
            }
            else
            {
                this.Edit.UpdateCanExecute(false);
                this.Save.UpdateCanExecute(false);
                this.Undo.UpdateCanExecute(false);
            }
        }

        protected virtual void BeforeEdit()
        {

        }

        protected virtual void AfterEdit()
        {

        }

        private void EditItem()
        {
            if (this.ModelItem != null)
            {
                // Run Before Edit
                this.BeforeEdit();

                // Ensure Transaction Exists
                if (this.ModelTransaction == null)
                {
                    this.ModelTransaction = this.ModelSession.BeginTransaction();
                }

                if (this.ModelItem.Locked(true))
                {
                    // Currently Locked by User, ensure added to Transaction
                    this.ModelItem.Update(this.ModelTransaction);
                }
                else
                {
                    // Not currently Locked by User - try and Lock
                    this.ModelItem.Update(this.ModelTransaction, true);
                }

                // Run After Lock
                this.AfterEdit();
            }

            this.SetCommandsCanExecute();
        }

        protected virtual void BeforeUndo()
        {

        }

        protected virtual void AfterUndo()
        {

        }

        private void UndoItem()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction != null)
                {
                    // Run Before Undo
                    this.BeforeUndo();

                    // Rollback Transaction
                    this.ModelTransaction.RollBack();
                    this.ModelTransaction = null;

                    // Run After UnLock
                    this.AfterUndo();
                }
            }

            this.SetCommandsCanExecute();
        }

        protected virtual void BeforeSave()
        {

        }

        protected virtual void AfterSave()
        {

        }

        private void SaveItem()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction != null)
                {
                    // Run Before Save
                    this.BeforeSave();

                    // Committ Transaction
                    this.ModelTransaction.Commit();
                    this.ModelTransaction = null;

                    // Run After Save
                    this.AfterSave();
                }
            }

            this.SetCommandsCanExecute();
        }

        public Item()
            : base()
        {
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.Undo = new UndoCommand(this);
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            public Item Item { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                this.Item.SaveItem();
                return true;
            }

            internal SaveCommand(Item Item)
            {
                this.Item = Item;
                this.CanExecute = false;
            }
        }

        public class EditCommand : Aras.ViewModel.Command
        {
            public Item Item { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                this.Item.EditItem();
                return true;
            }

            internal EditCommand(Item Item)
            {
                this.Item = Item;
                this.CanExecute = false;
            }
        }

        public class UndoCommand : Aras.ViewModel.Command
        {
            public Item Item { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                this.Item.UndoItem();
                return true;
            }

            internal UndoCommand(Item Item)
            {
                this.Item = Item;
                this.CanExecute = false;
            }
        }
    }
}
