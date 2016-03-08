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
    public class RelationshipTree : Tree
    {
        private IItemFormatter _itemFormatter;
        public IItemFormatter ItemFormatter 
        { 
            get
            {
                return this._itemFormatter;
            }
            set
            {
                if (this._itemFormatter != value)
                {
                    this._itemFormatter = value;
                    this.Refresh.Execute();
                }
            }
        }

        private List<Model.RelationshipType> _relationshipTypes;
        public IEnumerable<Model.RelationshipType> RelationshipTypes
        {
            get
            {
                return this._relationshipTypes;
            }
        }

        public void AddRelationshipType(Model.RelationshipType RelationshipType)
        {
            if (!this._relationshipTypes.Contains(RelationshipType))
            {
                this._relationshipTypes.Add(RelationshipType);
                this.Refresh.Execute();
            }
        }

        [ViewModel.Attributes.Command("Select")]
        public SelectCommand Select { get; private set; }

        [ViewModel.Attributes.Command("Cut")]
        public CutCommand Cut { get; private set; }

        [ViewModel.Attributes.Command("Copy")]
        public CopyCommand Copy { get; private set; }

        [ViewModel.Attributes.Command("Paste")]
        public PasteCommand Paste { get; private set; }

        [ViewModel.Attributes.Command("Delete")]
        public DeleteCommand Delete { get; private set; }

        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Command("Indent")]
        public IndentCommand Indent { get; private set; }

        [ViewModel.Attributes.Command("Outdent")]
        public OutdentCommand Outdent { get; private set; }

        private RelationshipTreeNode _selected;
        public RelationshipTreeNode Selected
        {
            get
            {
                return this._selected;
            }
            private set
            {
                if (value != null)
                {
                    if (value.Tree.ID.Equals(this.ID) && !value.Equals(this._selected))
                    {
                        this._selected = value;
                        this.OnPropertyChanged("Selected");
                    }
                }
                else
                {
                    if (this._selected != null)
                    {
                        this._selected = null;
                        this.OnPropertyChanged("Selected");
                    }
                }
            }
        }

        private Model.Item CopyPasteBuffer { get; set; }

        private Model.Transaction _transaction;
        private Model.Transaction Transaction
        {
            get
            {
                if (this._transaction == null)
                {
                    if (this.Binding != null)
                    {
                        this._transaction = ((Model.Item)this.Binding).Session.BeginTransaction();
                    }
                }

                return this._transaction;
            }
        }

        private void RefreshCommands()
        {
            if (this.Binding != null)
            {
                if (this.Selected != null)
                {
                    this.Copy.UpdateCanExecute(true);

                    if (this.Selected.ID.Equals(this.Node.ID))
                    {
                        this.Cut.UpdateCanExecute(false);
                        this.Delete.UpdateCanExecute(false);
                    }
                    else
                    {
                        this.Cut.UpdateCanExecute(true);
                        this.Delete.UpdateCanExecute(true);
                    }

                    if (this.CopyPasteBuffer != null)
                    {
                        this.Paste.UpdateCanExecute(true);
                    }
                    else
                    {
                        this.Paste.UpdateCanExecute(false);
                    }

                    if (this.Selected.Parent != null)
                    {
                        if (!this.Selected.Parent.Children.First().Equals(this.Selected))
                        {
                            this.Outdent.UpdateCanExecute(true);
                        }
                        else
                        {
                            this.Outdent.UpdateCanExecute(false);
                        }

                        if (this.Selected.Parent.Parent != null)
                        {
                            this.Indent.UpdateCanExecute(true);
                        }
                        else
                        {
                            this.Indent.UpdateCanExecute(false);
                        }
                    }
                    else
                    {
                        this.Indent.UpdateCanExecute(false);
                        this.Outdent.UpdateCanExecute(false);
                    }

                }
                else
                {
                    this.Copy.UpdateCanExecute(false);
                    this.Cut.UpdateCanExecute(false);
                    this.Delete.UpdateCanExecute(false);
                    this.Paste.UpdateCanExecute(false);
                    this.Indent.UpdateCanExecute(false);
                    this.Outdent.UpdateCanExecute(false);
                }

                if (this._transaction != null)
                {
                    this.Save.UpdateCanExecute(true);
                }
                else
                {
                    this.Save.UpdateCanExecute(false);
                }
            }
            else
            {
                this.Copy.UpdateCanExecute(false);
                this.Cut.UpdateCanExecute(false);
                this.Delete.UpdateCanExecute(false);
                this.Paste.UpdateCanExecute(false);
                this.Save.UpdateCanExecute(false);
                this.Indent.UpdateCanExecute(false);
                this.Outdent.UpdateCanExecute(false);
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Item)
                {
                    this.Node = new RelationshipTreeNode(this, null);
                    this.Node.Binding = this.Binding;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Model.Item");
                }
            }
            else
            {
                this.Node = null;
            }
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            if (this.Node != null)
            {
                this.Node.Refresh.Execute();
            }
        }

        public RelationshipTree(IItemFormatter ItemFormatter)
            :base()
        {
            this._relationshipTypes = new List<Model.RelationshipType>();
            this._itemFormatter = ItemFormatter;
            this.Node = null;
            this._transaction = null;
            this.CopyPasteBuffer = null;
            this.Cut = new CutCommand(this);
            this.Copy = new CopyCommand(this);
            this.Delete = new DeleteCommand(this);
            this.Paste = new PasteCommand(this);
            this.Save = new SaveCommand(this);
            this.Select = new SelectCommand(this);
            this.Indent = new IndentCommand(this);
            this.Outdent = new OutdentCommand(this);
        }

        public RelationshipTree()
            :this(new ItemFormatters.Default())
        {

        }

        public class SelectCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (Parameters != null)
                {
                    foreach (Control parameter in Parameters)
                    {
                        if (parameter is RelationshipTreeNode)
                        {
                            this.RelationshipTree.Selected = (RelationshipTreeNode)parameter;
                        }
                    }

                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal SelectCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = true;
            }
        }

        public class CutCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null))
                {
                    // Store Related Item in CopyPaste Buffer
                    this.RelationshipTree.CopyPasteBuffer = this.RelationshipTree.Selected.Item;

                    // Update Parent Item
                    ((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.Update(this.RelationshipTree.Transaction);

                    // Delete Relationship
                    this.RelationshipTree.Selected.Relationship.Delete(this.RelationshipTree.Transaction);

                    // Refesh Parent Node
                    this.RelationshipTree.Selected.Parent.Refresh.Execute();

                    // Select Parent
                    this.RelationshipTree.Selected = (RelationshipTreeNode)this.RelationshipTree.Selected.Parent;

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal CutCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class CopyCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.RelationshipTree.Selected != null)
                {
                    // Store Related Item in CopyPaste Buffer
                    this.RelationshipTree.CopyPasteBuffer = this.RelationshipTree.Selected.Item;

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal CopyCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class PasteCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.CopyPasteBuffer != null))
                {
                    // Update Parent Item
                    this.RelationshipTree.Selected.Item.Update(this.RelationshipTree.Transaction);

                    // Create Relationship - ******** Need to sort out when more than one Relationship Type ********
                    this.RelationshipTree.Selected.Item.Store(this.RelationshipTree.RelationshipTypes.First()).Create(this.RelationshipTree.CopyPasteBuffer, this.RelationshipTree.Transaction);

                    // Refresh Parent Node
                    this.RelationshipTree.Selected.Refresh.Execute();

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal PasteCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class DeleteCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null))
                {
                    // Update Parent Item
                    ((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.Update(this.RelationshipTree.Transaction);

                    // Delete Relationship
                    this.RelationshipTree.Selected.Relationship.Delete(this.RelationshipTree.Transaction);

                    // Set Selected to Parent
                    this.RelationshipTree.Selected = (RelationshipTreeNode)this.RelationshipTree.Selected.Parent;

                    // Refesh Parent Node
                    this.RelationshipTree.Selected.Refresh.Execute();

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal DeleteCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.RelationshipTree._transaction != null)
                {
                    this.RelationshipTree._transaction.Commit();
                    this.RelationshipTree._transaction = null;

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal SaveCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class IndentCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null) && (this.RelationshipTree.Selected.Parent.Parent != null))
                {
                    // Get Current Child Node
                    RelationshipTreeNode childnode = (RelationshipTreeNode)this.RelationshipTree.Selected;

                    // Get Child Item
                    Model.Item childitem = childnode.Item;

                    // Get Current Parent Node
                    RelationshipTreeNode currentparentnode = (RelationshipTreeNode)this.RelationshipTree.Selected.Parent;

                    // Get New Parent Node
                    RelationshipTreeNode newparentnode = (RelationshipTreeNode)this.RelationshipTree.Selected.Parent.Parent;

                    // Update Current Parent Item
                    currentparentnode.Item.Update(this.RelationshipTree.Transaction);

                    // Delete Current Relationship
                    childnode.Relationship.Delete(this.RelationshipTree.Transaction);

                    // Refesh Current Parent Node
                    currentparentnode.Refresh.Execute();

                    // UpdateNew Parent Item
                    newparentnode.Item.Update(this.RelationshipTree.Transaction);

                    // Add New Relationship
                    newparentnode.Item.Store(this.RelationshipTree.RelationshipTypes.First()).Create(childitem, this.RelationshipTree.Transaction);

                    // Refresh new Parent
                    newparentnode.Refresh.Execute();

                    // Select New Parent
                    this.RelationshipTree.Selected = newparentnode;

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal IndentCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class OutdentCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && !this.RelationshipTree.Selected.Parent.Children.First().Equals(this.RelationshipTree.Selected))
                {
                    // Get Child Node
                    RelationshipTreeNode childnode = this.RelationshipTree.Selected;

                    // Get Current Parent Node
                    RelationshipTreeNode currentparantnode = (RelationshipTreeNode)childnode.Parent;

                    // Work out new Parent Node
                    RelationshipTreeNode newparentnode = null;

                    foreach(RelationshipTreeNode child in currentparantnode.Children)
                    {
                        if (child.Equals(childnode))
                        {
                            break;
                        }
                        else
                        {
                            newparentnode = child;
                        }
                    }

                    // Update Current Parent Item
                    currentparantnode.Item.Update(this.RelationshipTree.Transaction);

                    // Delete Current Relationship
                    childnode.Relationship.Delete(this.RelationshipTree.Transaction);

                    // Refesh Current Parent Node
                    currentparantnode.Refresh.Execute();

                    // Update New Parent Item
                    newparentnode.Item.Update(this.RelationshipTree.Transaction);

                    // Create new Relationship
                    newparentnode.Item.Store(this.RelationshipTree.RelationshipTypes.First()).Create(childnode.Item, this.RelationshipTree.Transaction);

                    // Refresh New Parent Node
                    newparentnode.Refresh.Execute();

                    // Select New Parent Node
                    this.RelationshipTree.Selected = newparentnode;

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal OutdentCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }
    }
}
