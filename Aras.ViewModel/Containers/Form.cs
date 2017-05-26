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
    public abstract class Form : BorderContainer, IToolbarProvider, IItemControl
    {
        public event FormEventHandler Created;

        private void OnCreated()
        {
            if (this.Created != null)
            {
                this.Created(this, new FormEventArgs((Model.Item)this.Binding));
            }
        }

        public event FormEventHandler Edited;

        private void OnEdited()
        {
            if (this.Edited != null)
            {
                this.Edited(this, new FormEventArgs((Model.Item)this.Binding));
            }
        }

        public event FormEventHandler Promoted;

        private void OnPromoted()
        {
            if (this.Promoted != null)
            {
                this.Promoted(this, new FormEventArgs((Model.Item)this.Binding));
            }
        }

        public event FormEventHandler Saved;

        private void OnSaved()
        {
            if (this.Saved != null)
            {
                this.Saved(this, new FormEventArgs((Model.Item)this.Binding));
            }
        }

        public event FormEventHandler Undone;

        private void OnUndone()
        {
            if (this.Undone != null)
            {
                this.Undone(this, new FormEventArgs((Model.Item)this.Binding));
            }
        }

        public Model.Store Store { get; private set; }

        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        private Button _refreshButton;
        public Button RefreshButton
        {
            get
            {
                if (this._refreshButton == null)
                {
                    this._refreshButton = new Button(this.Session);
                    this._refreshButton.Icon = "Refresh";
                    this._refreshButton.Tooltip = "Refresh";
                    this._refreshButton.Command = this.Refresh;
                }

                return this._refreshButton;
            }
        }

        [ViewModel.Attributes.Command("Create")]
        public CreateCommand Create { get; private set; }

        private Button _createButton;
        public Button CreateButton
        {
            get
            {
                if (this._createButton == null)
                {
                    this._createButton = new Button(this.Session);
                    this._createButton.Icon = "New";
                    this._createButton.Tooltip = "Create";
                    this._createButton.Command = this.Create;
                }

                return this._createButton;
            }
        }

        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        private Button _saveButton;
        public Button SaveButton
        {
            get
            {
                if (this._saveButton == null)
                {
                    this._saveButton = new Button(this.Session);
                    this._saveButton.Icon = "Save";
                    this._saveButton.Tooltip = "Save";
                    this._saveButton.Command = this.Save;
                }

                return this._saveButton;
            }
        }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        private Button _editButton;
        public Button EditButton
        {
            get
            {
                if (this._editButton == null)
                {
                    this._editButton = new Button(this.Session);
                    this._editButton.Icon = "Edit";
                    this._editButton.Tooltip = "Edit";
                    this._editButton.Command = this.Edit;
                }

                return this._editButton;
            }
        }

        [ViewModel.Attributes.Command("Promote")]
        public PromoteCommand Promote { get; private set; }

        private Button _promoteButton;
        public Button PromoteButton
        {
            get
            {
                if (this._promoteButton == null)
                {
                    this._promoteButton = new Button(this.Session);
                    this._promoteButton.Icon = "Promote";
                    this._promoteButton.Tooltip = "Promote";
                    this._promoteButton.Command = this.Promote;
                }

                return this._promoteButton;
            }
        }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        private Button _undoButton;
        public Button UndoButton
        {
            get
            {
                if (this._undoButton == null)
                {
                    this._undoButton = new Button(this.Session);
                    this._undoButton.Icon = "Undo";
                    this._undoButton.Tooltip = "Undo";
                    this._undoButton.Command = this.Undo;
                }

                return this._undoButton;
            }
        }

        private Containers.Toolbar _toolbar;
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);

                    // Add Refresh Button
                    this._toolbar.Children.Add(this.RefreshButton);

                    // Add Create Button
                    this._toolbar.Children.Add(this.CreateButton);

                    // Add Edit Button
                    this._toolbar.Children.Add(this.EditButton);

                    // Add Undo Button
                    this._toolbar.Children.Add(this.UndoButton);

                    // Add Save Button
                    this._toolbar.Children.Add(this.SaveButton);

                    // Add Promote Button
                    this._toolbar.Children.Add(this.PromoteButton);

                }

                return this._toolbar;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Item)
                {
                    // Watch for changes in Item
                    ((Model.Item)this.Binding).PropertyChanged -= Item_PropertyChanged;

                    if (this.Transaction != null)
                    {
                        // Rollback existing Transaction
                        this.Transaction.RollBack();
                        this.Transaction = null;
                    }
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Item)
                {
                    // Set Item
                    this.Item = (Model.Item)this.Binding;

                    // Watch for changes in Item
                    this.Item.PropertyChanged += Item_PropertyChanged;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
                }
            }
            else
            {
                this.Transaction = null;
                this.Item = null;
            }

            // Update Commands
            this.SetCommandsCanExecute();
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "LockedBy":
                    this.SetCommandsCanExecute();
                    break;
                default:
                    break;
            }
        }

        protected Model.Item Item { get; private set; }

        public Model.Transaction Transaction { get; private set; }

        private void CreateForm()
        {
            this.ResetError();

            // Undo Changes
            this.UndoForm();

            this.Transaction = this.Session.Model.BeginTransaction();
            this.Binding = this.Store.Create(this.Transaction);
            this.SetCommandsCanExecute();

            this.OnCreated();
        }

        protected virtual void SaveForm()
        {
            this.ResetError();

            if (this.Transaction != null)
            {
                try
                {
                    this.Transaction.Commit(true);
                    this.Transaction = null;
                    this.OnSaved();
                }
                catch (Model.Exceptions.ServerException e)
                {
                    // Raise Error Message
                    this.OnError(e.Message);

                    // Add Item to a new Transaction
                    this.Transaction = this.Session.Model.BeginTransaction();
                    this.Item.Update(this.Transaction);
                }

                this.SetCommandsCanExecute();
            }
        }

        protected virtual void EditForm()
        {
            this.ResetError();

            if (this.Item != null)
            {
                if (this.Transaction == null)
                {
                    this.Transaction = this.Session.Model.BeginTransaction();
                    this.Item.Update(this.Transaction);
                    this.SetCommandsCanExecute();
                    this.OnEdited();
                }
            }
        }

        protected virtual void PromoteForm()
        {
            this.ResetError();

            if (this.Item != null)
            {
                if (this.Transaction == null)
                {
                    IEnumerable<Model.Relationships.LifeCycleState> newstates = this.Item.NextStates();
                   
                    if (newstates.Count() > 0)
                    {
                        this.Item.Promote(newstates.First());
                    }

                    this.SetCommandsCanExecute();
                    this.OnPromoted();
                }
            }
        }

        protected virtual void UndoForm()
        {
            this.ResetError();

            if (this.Transaction != null)
            {
                this.Transaction.RollBack();
                this.Transaction = null;

                if (this.Item.Action == Model.Item.Actions.Create)
                {
                    this.Binding = null;
                }

                this.SetCommandsCanExecute();
                this.OnUndone();
            }
        }

        private void SetCommandsCanExecute()
        {
            if (this.Item != null)
            {
                switch (this.Item.Locked)
                {
                    case Model.Item.Locks.None:

                        this.Edit.UpdateCanExecute(true);
                        this.EditButton.Tooltip = "Edit";
                        this.Save.UpdateCanExecute(false);
                        this.Undo.UpdateCanExecute(false);

                        if (this.Item.NextStates().Count() > 0)
                        {
                            this.Promote.UpdateCanExecute(true);
                        }
                        else
                        {
                            this.Promote.UpdateCanExecute(false);
                        }

                        if (this.Transaction != null)
                        {
                            // Rollback existing Transaction
                            this.Transaction.RollBack();
                            this.Transaction = null;
                        }

                        break;

                    case Model.Item.Locks.User:

                        this.Edit.UpdateCanExecute(false);
                        this.EditButton.Tooltip = "Edit";
                        this.Save.UpdateCanExecute(true);
                        this.Undo.UpdateCanExecute(true);
                        this.Promote.UpdateCanExecute(false);

                        if (this.Transaction == null)
                        {
                            // Create Transaction and Add Item
                            this.Transaction = this.Session.Model.BeginTransaction();
                            this.Item.Update(this.Transaction);
                        }

                        break;

                    case Model.Item.Locks.OtherUser:

                        this.Edit.UpdateCanExecute(false);
                        this.EditButton.Tooltip = "Locked by " + this.Item.LockedBy.FullName;
                        this.Save.UpdateCanExecute(false);
                        this.Undo.UpdateCanExecute(false);
                        this.Promote.UpdateCanExecute(false);

                        if (this.Transaction != null)
                        {
                            // Rollback existing Transaction
                            this.Transaction.RollBack();
                            this.Transaction = null;
                        }

                        break;
                }
            }

            this.Create.UpdateCanExecute(true);
        }

        protected virtual void RefreshForm()
        {
            if (this.Item != null)
            {
                // Refresh Item
                this.Item.Refresh();
            }

            // Update Form
            this.SetCommandsCanExecute();
        }

        public Form(Manager.Session Session, Model.Store Store)
            :base(Session)
        {
            this.Store = Store;
            this.Refresh = new RefreshCommand(this);
            this.Create = new CreateCommand(this);
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.Promote = new PromoteCommand(this);
            this.Undo = new UndoCommand(this);
            this.Transaction = null;
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Form)this.Control).RefreshForm();
                this.CanExecute = true;
            }

            internal RefreshCommand(Control Control)
                : base(Control)
            {
                this.CanExecute = true;
            }
        }

        public class CreateCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Aras.ViewModel.Control> Parameters)
            {
                ((Form)this.Control).CreateForm();
            }

            internal CreateCommand(Form Form)
                :base(Form)
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
                ((Form)this.Control).SaveForm();
            }

            internal SaveCommand(Form Form)
                :base(Form)
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
                ((Form)this.Control).EditForm();
            }

            internal EditCommand(Form Form)
                :base(Form)
            {
            }
        }

        public class PromoteCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Form)this.Control).PromoteForm();
            }

            internal PromoteCommand(Form Form)
                : base(Form)
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
                ((Form)this.Control).UndoForm();
            }

            internal UndoCommand(Form Form)
                :base(Form)
            {
            }
        }

    }
}
