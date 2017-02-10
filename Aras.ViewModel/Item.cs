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

        [ViewModel.Attributes.Command("SaveUnLock")]
        public SaveUnLockCommand SaveUnLock { get; private set; }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        protected virtual Model.Item GetBindingItem(Model.Session Sesison, String ID)
        {
            throw new NotImplementedException();
        }

        public void SetBinding(Model.Session Session, String ID)
        {
            try
            {
                Model.Item context = this.GetBindingItem(Session, ID);
                context.Refresh();
                this.Binding = context;
            }
            catch(Exception e)
            {
                throw new Model.Exceptions.ServerException("Failed to set Context: " + ID, e);
            }
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            if (this.ModelItem != null)
            {
                this.ModelItem.Refresh();

                if (this.ModelItem.Locked(true))
                {
                    // Item is Locked by User set to Edit
                    this.EditItem();
                }
                else
                {
                    // Undo any changes
                    this.UndoItem();
                }
            }
            else
            {
                this.SetCommandsCanExecute();
            }
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
                this.SetCommandsCanExecute();
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
                    this.ModelItem.UnlockUpdate(this.ModelTransaction);
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

                    // Committ Transaction without UnLock
                    this.ModelTransaction.Commit(false);

                    // Remove current Transaction
                    this.ModelTransaction = null;

                    // Run After Save
                    this.AfterSave();

                    this.SetCommandsCanExecute();
                }

                // Put Item into Edit
                this.EditItem();
            }
            else
            {
                this.SetCommandsCanExecute();
            }
        }

        protected virtual void BeforeSaveUnLock()
        {

        }

        protected virtual void AfterSaveUnLock()
        {

        }

        private void SaveUnLockItem()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction != null)
                {
                    // Run Before Save
                    this.BeforeSave();

                    // Committ Transaction
                    this.ModelTransaction.Commit(true);
                    this.ModelTransaction = null;

                    // Run After Save
                    this.AfterSave();
                }
            }

            this.SetCommandsCanExecute();
        }

        public Item(Manager.Session Session)
            : base(Session)
        {
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.SaveUnLock = new SaveUnLockCommand(this);
            this.Undo = new UndoCommand(this);
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Item)this.Control).SaveItem();
            }

            internal SaveCommand(Item Item)
                :base(Item)
            {
            }
        }

        public class SaveUnLockCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Item)this.Control).SaveUnLockItem();
            }

            internal SaveUnLockCommand(Item Item)
                :base(Item)
            {
            }

        }
        public class EditCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Item)this.Control).EditItem();
            }

            internal EditCommand(Item Item)
                :base(Item)
            {
            }
        }

        public class UndoCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Item)this.Control).UndoItem();
            }

            internal UndoCommand(Item Item)
                :base(Item)
            {
            }
        }
    }
}
