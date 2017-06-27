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

namespace Aras.ViewModel.Containers
{
    [Attributes.ClientControl("Aras.View.Containers.Plugin")]
    public abstract class Plugin : Containers.BorderContainer
    {
        [Attributes.Property("Refresh", Attributes.PropertyTypes.Command, true)]
        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("SaveUnLock")]
        public SaveUnLockCommand SaveUnLock { get; private set; }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        protected virtual Model.Item GetBindingItem(String ID)
        {
            throw new NotImplementedException();
        }

        public void SetBinding(String ID)
        {
            try
            {
                // Get Context
                Model.Item context = this.GetBindingItem(ID);

                // Refresh Context
                if (context != null)
                {
                    context.Refresh();
                }
                    
                // Set Binding
                this.Binding = context;
            }
            catch (Exception e)
            {
                throw new Model.Exceptions.ServerException("Failed to set Context: " + ID, e);
            }
        }

        protected virtual void RefreshControl()
        {
            if (this.ModelItem != null)
            {
                this.ModelItem.Refresh();

                if (this.ModelItem.Locked == Model.Item.Locks.User)
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
                    return this.ModelItem.Store.Session;
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

            if (this.ModelItem.Locked == Model.Item.Locks.User)
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

        protected abstract Boolean CanSave { get; }

        protected abstract Boolean CanEdit { get; }

        private void SetCommandsCanExecute()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction == null)
                {
                    if (this.CanEdit)
                    {
                        this.Edit.UpdateCanExecute(true);
                    }
                    else
                    {
                        this.Edit.UpdateCanExecute(false);
                    }

                    this.Save.UpdateCanExecute(false);
                    this.Undo.UpdateCanExecute(false);
                }
                else
                {
                    this.Edit.UpdateCanExecute(false);

                    if (this.CanSave)
                    {
                        this.Save.UpdateCanExecute(true);
                    }
                    else
                    {
                        this.Save.UpdateCanExecute(false);
                    }

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

                if (this.ModelItem.Locked == Model.Item.Locks.User)
                {
                    // Currently Locked by User, ensure added to Transaction
                    this.ModelItem.Update(this.ModelTransaction);
                }
                else
                {
                    // Not currently Locked by User - try and Lock
                    this.ModelItem.Update(this.ModelTransaction);
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

        public Plugin(Manager.Session Session)
            :base(Session)
        {
            this.Refresh = new RefreshCommand(this);
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.SaveUnLock = new SaveUnLockCommand(this);
            this.Undo = new UndoCommand(this);
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Plugin)this.Control).RefreshControl();
                this.CanExecute = true;
            }

            internal RefreshCommand(Plugin Plugin)
                : base(Plugin)
            {
                this.CanExecute = true;
            }
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Plugin)this.Control).SaveItem();
            }

            internal SaveCommand(Plugin Plugin)
                : base(Plugin)
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
                ((Plugin)this.Control).SaveUnLockItem();
            }

            internal SaveUnLockCommand(Plugin Plugin)
                : base(Plugin)
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
                ((Plugin)this.Control).EditItem();
            }

            internal EditCommand(Plugin Plugin)
                : base(Plugin)
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
                ((Plugin)this.Control).UndoItem();
            }

            internal UndoCommand(Plugin Plugin)
                : base(Plugin)
            {
            }
        }
    }
}
