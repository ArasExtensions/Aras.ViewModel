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

namespace Aras.ViewModel
{
    public abstract class Form : Control
    {
        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("SaveUnLock")]
        public SaveUnLockCommand SaveUnLock { get; private set; }

        [ViewModel.Attributes.Command("Edit")]
        public EditCommand Edit { get; private set; }

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        [ViewModel.Attributes.Property("Children", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public Model.ObservableList<Control> Children { get; private set; }

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
                    refreshbutton.Binding = this.Refresh;

                    // Add Edit Button
                    Button editbutton = new Button(this.Session);
                    editbutton.Icon = "Edit";
                    editbutton.Tooltip = "Edit";
                    this._toolbar.Children.Add(editbutton);
                    editbutton.Binding = this.Edit;

                    // Add Undo Button
                    Button undobutton = new Button(this.Session);
                    undobutton.Icon = "Undo";
                    undobutton.Tooltip = "Undo";
                    this._toolbar.Children.Add(undobutton);
                    undobutton.Binding = this.Undo;

                    // Add Save Button
                    Button savebutton = new Button(this.Session);
                    savebutton.Icon = "Save";
                    savebutton.Tooltip = "Save";
                    this._toolbar.Children.Add(savebutton);
                    savebutton.Binding = this.Save;

                    // Add SaveUnlock Button
                    Button saveunlockbutton = new Button(this.Session);
                    saveunlockbutton.Icon = "SaveUnlock";
                    saveunlockbutton.Tooltip = "Save Unlock";
                    this._toolbar.Children.Add(saveunlockbutton);
                    saveunlockbutton.Binding = this.Save;
                }

                return this._toolbar;
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Transaction != null)
            {
                // Rollback existing Transaction
                this.Transaction.RollBack();
                this.Transaction = null;
            }

            if (this.Binding != null)
            {
                if (this.Binding is Model.Item)
                {
                    // Set Item
                    this.Item = (Model.Item)this.Binding;

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
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
                }
            }
            else
            {
                this.Item = null;
            }

            // Update Commands
            this.SetCommandsCanExecute();
        }

        protected Model.Item Item { get; private set; }

        protected Model.Transaction Transaction { get; private set; }

        private void SaveForm()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit(false);
                this.Transaction = this.Session.Model.BeginTransaction();
                this.Item.Update(this.Transaction);
                this.SetCommandsCanExecute();
            }
        }

        private void SaveUnLockForm()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit(true);
                this.SetCommandsCanExecute();
            }
        }

        private void EditForm()
        {
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
            if (this.Transaction != null)
            {
                this.Transaction.RollBack();
                this.Transaction = null;
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
                    this.SaveUnLock.UpdateCanExecute(false);
                    this.Undo.UpdateCanExecute(false);
                }
                else
                {
                    this.Edit.UpdateCanExecute(false);
                    this.Save.UpdateCanExecute(true);
                    this.SaveUnLock.UpdateCanExecute(true);
                    this.Undo.UpdateCanExecute(true);
                }
            }
            else
            {
                this.Edit.UpdateCanExecute(false);
                this.Save.UpdateCanExecute(false);
                this.SaveUnLock.UpdateCanExecute(false);
                this.Undo.UpdateCanExecute(false);
            }
        }

        public Form(Manager.Session Session)
            :base(Session)
        {
            this.Children = new Model.ObservableList<Control>();
            this.Edit = new EditCommand(this);
            this.Save = new SaveCommand(this);
            this.SaveUnLock = new SaveUnLockCommand(this);
            this.Undo = new UndoCommand(this);
            this.Transaction = null;
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            public Form Form { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Form.SaveForm();
            }

            internal SaveCommand(Form Form)
            {
                this.Form = Form;
                this.CanExecute = false;
            }
        }

        public class SaveUnLockCommand : Aras.ViewModel.Command
        {
            public Form Form { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Form.SaveUnLockForm();
            }

            internal SaveUnLockCommand(Form Form)
            {
                this.Form = Form;
                this.CanExecute = false;
            }

        }
        public class EditCommand : Aras.ViewModel.Command
        {
            public Form Form { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Form.EditForm();
            }

            internal EditCommand(Form Form)
            {
                this.Form = Form;
                this.CanExecute = false;
            }
        }

        public class UndoCommand : Aras.ViewModel.Command
        {
            public Form Form { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Form.UndoForm();
            }

            internal UndoCommand(Form Form)
            {
                this.Form = Form;
                this.CanExecute = false;
            }
        }

    }
}
