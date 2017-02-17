/*  
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2016 Processwall Limited.

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

namespace Aras.ViewModel.Containers
{
    public abstract class Form : BorderContainer, IToolbarProvider
    {
        public Model.Stores.Item Store { get; private set; }

        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("Create")]
        public CreateCommand Create { get; private set; }

        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

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
                    Button refreshbutton = new Button(this.Session);
                    refreshbutton.Icon = "Refresh";
                    refreshbutton.Tooltip = "Refresh";
                    this._toolbar.Children.Add(refreshbutton);
                    refreshbutton.Command = this.Refresh;

                    // Add Create Button
                    Button createbutton = new Button(this.Session);
                    createbutton.Icon = "New";
                    createbutton.Tooltip = "Create";
                    this._toolbar.Children.Add(createbutton);
                    createbutton.Command = this.Create;

                    // Add Edit Button
                    Button editbutton = new Button(this.Session);
                    editbutton.Icon = "Edit";
                    editbutton.Tooltip = "Edit";
                    this._toolbar.Children.Add(editbutton);
                    editbutton.Command = this.Edit;

                    // Add Undo Button
                    Button undobutton = new Button(this.Session);
                    undobutton.Icon = "Undo";
                    undobutton.Tooltip = "Undo";
                    this._toolbar.Children.Add(undobutton);
                    undobutton.Command = this.Undo;

                    // Add Save Button
                    Button savebutton = new Button(this.Session);
                    savebutton.Icon = "Save";
                    savebutton.Tooltip = "Save";
                    this._toolbar.Children.Add(savebutton);
                    savebutton.Command = this.Save;
                }

                return this._toolbar;
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

                    if (this.Item.DatabaseState != Model.Item.DatabaseStates.New)
                    {
                        if (this.Transaction != null)
                        {
                            // Rollback existing Transaction
                            this.Transaction.RollBack();
                            this.Transaction = null;
                        }

                        if (this.Item.Locked(true))
                        {
                            // Create Transaction and add Item
                            this.Transaction = this.Session.Model.BeginTransaction();
                            this.Item.Update(this.Transaction);
                        }
                        else
                        {
                            this.Transaction = null;
                        }
                    }
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
                }
            }
            else
            {
                if (this.Transaction != null)
                {
                    // Rollback existing Transaction
                    this.Transaction.RollBack();
                    this.Transaction = null;
                }

                this.Item = null;
            }

            // Update Commands
            this.SetCommandsCanExecute();
        }

        protected Model.Item Item { get; private set; }

        internal Model.Transaction Transaction { get; private set; }

        private void CreateForm()
        {
            this.ResetError();

            // Undo Changes
            this.UndoForm();

            this.Transaction = this.Session.Model.BeginTransaction();
            this.Binding = this.Store.Create(this.Transaction);
            this.SetCommandsCanExecute();
        }

        private void SaveForm()
        {
            this.ResetError();

            if (this.Transaction != null)
            {
                try
                {
                    this.Transaction.Commit(true);
                    this.Transaction = null;
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
                }
            }
        }

        private void UndoForm()
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
            }
        }

        private void SetCommandsCanExecute()
        {
            if (this.Item != null)
            {
                if (this.Transaction == null)
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

            this.Create.UpdateCanExecute(true);
        }

        protected virtual void RefreshControl()
        {

        }

        public Form(Manager.Session Session, Model.Stores.Item Store)
            :base(Session)
        {
            this.Store = Store;
            this.Refresh = new RefreshCommand(this);
            this.Create = new CreateCommand(this);
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.Undo = new UndoCommand(this);
            this.Transaction = null;
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Form)this.Control).RefreshControl();
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
