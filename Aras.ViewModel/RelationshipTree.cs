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

        private IRelationshipFormatter _relationshipFormatter;
        public IRelationshipFormatter RelationshipFormatter 
        { 
            get
            {
                return this._relationshipFormatter;
            }
            set
            {
                if (this._relationshipFormatter != value)
                {
                    this._relationshipFormatter = value;
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

        [ViewModel.Attributes.Command("Add")]
        public AddCommand Add { get; private set; }

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

        [ViewModel.Attributes.Command("Undo")]
        public UndoCommand Undo { get; private set; }

        [ViewModel.Attributes.Command("Indent")]
        public IndentCommand Indent { get; private set; }

        [ViewModel.Attributes.Command("Outdent")]
        public OutdentCommand Outdent { get; private set; }

        [ViewModel.Attributes.Command("SearchClosed")]
        public SearchClosedCommand SearchClosed { get; private set; }

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

        private Boolean _showSearch;
        [ViewModel.Attributes.Property("ShowSearch", Aras.ViewModel.Attributes.PropertyTypes.Boolean, true)]
        public Boolean ShowSearch
        {
            get
            {
                return this._showSearch;
            }
            private set
            {
                if (this._showSearch != value)
                {
                    this._showSearch = value;
                    this.OnPropertyChanged("ShowSearch");
                }
            }
        }

        [ViewModel.Attributes.Property("Search", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public Searches.Item Search { get; private set; }
   
        private Dictionary<Model.Item, RelationshipTreeNode> NodeCache;

        internal RelationshipTreeNode GetNodeFromCache(Model.Item Item)
        {
            if (this.NodeCache.ContainsKey(Item))
            {
                return this.NodeCache[Item];
            }
            else
            {
                return null;
            }
        }

        internal void AddNodeToCache(RelationshipTreeNode Node)
        {
            this.NodeCache[Node.Item] = Node;
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
            this.Copy.UpdateCanExecute();
            this.Cut.UpdateCanExecute();
            this.Delete.UpdateCanExecute();
            this.Paste.UpdateCanExecute();
            this.Save.UpdateCanExecute();
            this.Indent.UpdateCanExecute();
            this.Outdent.UpdateCanExecute();
            this.Undo.UpdateCanExecute();
            this.Add.UpdateCanExecute();
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Item)
                {
                    // Ensure Permission is Selected for Items
                    ((Model.Item)this.Binding).ItemType.AddToSelect("permission_id");

                    // Create Root Node
                    this.Node = new RelationshipTreeNode(this, null);
                    this.Node.Binding = this.Binding;
                    this.AddNodeToCache((RelationshipTreeNode)this.Node);

                    // Set Binding for Search Control
                    this.Search.Binding = ((Model.Item)this.Binding).Session.Store(((Model.Item)this.Binding).ItemType);

                    // Watch for Selection on Search Control
                    this.Search.Selected.ListChanged -= Selected_ListChanged;
                    this.Search.Selected.ListChanged += Selected_ListChanged;
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

        public RelationshipTree(IRelationshipFormatter RelationshipFormatter, IItemFormatter ItemFormatter)
            :base()
        {
            this._relationshipTypes = new List<Model.RelationshipType>();
            this.NodeCache = new Dictionary<Model.Item, RelationshipTreeNode>();
            this._relationshipFormatter = RelationshipFormatter;
            this._itemFormatter = ItemFormatter;
            this.Node = null;
            this._transaction = null;
            this.CopyPasteBuffer = null;
            this.Add = new AddCommand(this);
            this.Cut = new CutCommand(this);
            this.Copy = new CopyCommand(this);
            this.Delete = new DeleteCommand(this);
            this.Paste = new PasteCommand(this);
            this.Save = new SaveCommand(this);
            this.Undo = new UndoCommand(this);
            this.Select = new SelectCommand(this);
            this.Indent = new IndentCommand(this);
            this.Outdent = new OutdentCommand(this);
            this.SearchClosed = new SearchClosedCommand(this);
            this.Search = new Searches.Item();
        }

        public RelationshipTree()
            : this(new RelationshipFormatters.Default(), new ItemFormatters.Default())
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

        public class AddCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if(this.RelationshipTree.Selected != null)
                {
                    if (this.RelationshipTree.Selected.Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.RelationshipTree.Selected != null)
                {
                    this.RelationshipTree.ShowSearch = true;
                }
                else
                {
                    this.RelationshipTree.ShowSearch = false;
                }

                return true;
            }

            internal AddCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class CutCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null))
                {
                    if (((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
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

        void Selected_ListChanged(object sender, EventArgs e)
        {
            if ((this.Selected != null) && (this.Search.Selected != null))
            {
                // Seach Selection - Add Item

                // Get Parent Item
                Model.Item parent = this.Selected.Item;

                // Update Parent Item
                parent.Update(this.Transaction);

                // Create Relationship - ******** Need to sort out when more than one Relationship Type ********
                parent.Store(this.RelationshipTypes.First()).Create(this.Search.Selected.First(), this.Transaction);

                // Refresh Parent Node
                this.Selected.Refresh.Execute();

                // Refresh Commands
                this.RefreshCommands();
            }

            // Close Search Dialogue
            this.ShowSearch = false;
        }

        public class CopyCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if (this.RelationshipTree.Selected != null)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
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

            internal void UpdateCanExecute()
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null) && (this.RelationshipTree.CopyPasteBuffer != null))
                {
                    if (((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
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

            internal void UpdateCanExecute()
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null))
                {
                    if (((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null))
                {
                    // Get Parent Node
                    RelationshipTreeNode parentnode = (RelationshipTreeNode)this.RelationshipTree.Selected.Parent;

                    // Get Node to Delete
                    RelationshipTreeNode todeletenode = (RelationshipTreeNode)this.RelationshipTree.Selected;

                    // Update Parent Item
                    parentnode.Item.Update(this.RelationshipTree.Transaction);

                    // Delete Relationship
                    todeletenode.Relationship.Delete(this.RelationshipTree.Transaction);

                    // Set Selected to Parent
                    this.RelationshipTree.Selected = parentnode;

                    // Refesh Parent Node
                    parentnode.RefreshChildren();

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

            internal void UpdateCanExecute()
            {
                if (this.RelationshipTree._transaction != null)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
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

        public class UndoCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if (this.RelationshipTree._transaction != null)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if (this.RelationshipTree._transaction != null)
                {
                    this.RelationshipTree._transaction.RollBack();
                    this.RelationshipTree._transaction = null;

                    // Refresh Tree
                    this.RelationshipTree.Refresh.Execute();

                    // Refresh Commands
                    this.RelationshipTree.RefreshCommands();
                }

                return true;
            }

            internal UndoCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class OutdentCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null) && (this.RelationshipTree.Selected.Parent.Parent != null))
                {
                    if (((RelationshipTreeNode)this.RelationshipTree.Selected.Parent).Item.CanUpdate && ((RelationshipTreeNode)this.RelationshipTree.Selected.Parent.Parent).Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
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

            internal OutdentCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class IndentCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            internal void UpdateCanExecute()
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null) && !this.RelationshipTree.Selected.Parent.Children.First().Equals(this.RelationshipTree.Selected))
                {
                    if (this.CurrentParentNode.Item.CanUpdate && this.NewParentNode.Item.CanUpdate)
                    {
                        this.CanExecute = true;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            private RelationshipTreeNode ChildNode
            {
                get
                {
                    return this.RelationshipTree.Selected;
                }
            }

            private RelationshipTreeNode CurrentParentNode
            {
                get
                {
                    return (RelationshipTreeNode)this.ChildNode.Parent;
                }
            }

            private RelationshipTreeNode NewParentNode
            {
                get
                {
                    RelationshipTreeNode newparentnode = null;

                    foreach (RelationshipTreeNode child in this.CurrentParentNode.Children)
                    {
                        if (child.Equals(this.ChildNode))
                        {
                            break;
                        }
                        else
                        {
                            newparentnode = child;
                        }
                    }

                    return newparentnode;
                }
            }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                if ((this.RelationshipTree.Selected != null) && (this.RelationshipTree.Selected.Parent != null) && !this.RelationshipTree.Selected.Parent.Children.First().Equals(this.RelationshipTree.Selected))
                {
                    // Get Child Node
                    RelationshipTreeNode childnode = this.ChildNode;

                    // Get Current Parent Node
                    RelationshipTreeNode currentparantnode = this.CurrentParentNode;

                    // Work out new Parent Node
                    RelationshipTreeNode newparentnode = this.NewParentNode;

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

            internal IndentCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = false;
            }
        }

        public class SearchClosedCommand : Aras.ViewModel.Command
        {
            public RelationshipTree RelationshipTree { get; private set; }

            protected override bool Run(IEnumerable<Control> Parameters)
            {
                this.RelationshipTree.ShowSearch = false;

                return true;
            }

            internal SearchClosedCommand(RelationshipTree RelationshipTree)
            {
                this.RelationshipTree = RelationshipTree;
                this.CanExecute = true;
            }
        }
    }
}
