/*
  Aras.ViewModel provides a .NET library for building Aras Innovator Applications

  Copyright (C) 2017 Processwall Limited.

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

namespace Aras.ViewModel.Grids
{
    public class Relationship : Containers.BorderContainer, IToolbarProvider
    {
        private Containers.Toolbar _toolbar;
        public virtual Containers.Toolbar Toolbar
        {
            get
            {
                if (this._toolbar == null)
                {
                    // Create Toolbar
                    this._toolbar = new Containers.Toolbar(this.Session);
                    this._toolbar.Children.NotifyListChanged = false;

                    // Add Create Button
                    Button createbutton = new Button(this.Session);
                    createbutton.Icon = "New";
                    createbutton.Tooltip = "Create " + this.RelationshipType.SingularLabel;
                    this._toolbar.Children.Add(createbutton);
                    createbutton.Command = this.Create;

                    // Add Create Button
                    Button deletebutton = new Button(this.Session);
                    deletebutton.Icon = "Delete";
                    deletebutton.Tooltip = "Delete " + this.RelationshipType.PluralLabel;
                    this._toolbar.Children.Add(deletebutton);
                    deletebutton.Command = this.Delete;

                    this._toolbar.Children.NotifyListChanged = true;
                }

                return this._toolbar;
            }
        }

        [ViewModel.Attributes.Command("Create")]
        public CreateCommand Create { get; private set; }

        [ViewModel.Attributes.Command("Delete")]
        public DeleteCommand Delete { get; private set; }

        private Dialogs.Searches.ItemType _dialog;
        [Attributes.Property("Dialog", Attributes.PropertyTypes.Control, true)]
        public Dialogs.Searches.ItemType Dialog
        {
            get
            {
                return this._dialog;
            }
            private set
            {
                if (this._dialog != value)
                {
                    this._dialog = value;
                    this.OnPropertyChanged("Dialog");
                }
            }
        }

        public IItemControl Parent { get; private set; }

        public Model.RelationshipType RelationshipType { get; private set; }

        protected ViewModel.Grid Grid { get; private set; }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (this.Binding != null)
            {
                if (!(this.Binding is Model.Item))
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Item");
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                // Add RelationshipGrid PropertTypes to Select for the Relationship
                this.RelationshipType.AddRelationshipGridPropertyTypesToSelect();

                // Add RelationshipGrid PropertTypes to Select for the Related ItemType
                if (this.RelationshipType.RelatedItemType != null)
                {
                    this.RelationshipType.RelatedItemType.AddRelationshipGridPropertyTypesToSelect();
                }
            }

            // Ensure Dialog is Closed
            this.Dialog.Open = false;

            this.LoadRows();
        }

        private List<Model.PropertyType> _propertyTypes;
        protected IEnumerable<Model.PropertyType> PropertyTypes
        {
            get
            {
                if (this._propertyTypes == null)
                {
                    this._propertyTypes = new List<Model.PropertyType>();

                    switch (this.RelationshipType.RelationshipGridView)
                    {
                        case Model.RelationshipGridViews.InterMix:
                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.Sort();

                            break;
                        case Model.RelationshipGridViews.Right:

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            break;
                        default:
                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            if (this.RelationshipType.RelatedItemType != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.RelatedItemType.RelationshipGridPropertyTypes);
                            }

                            break;
                    }
                }

                return this._propertyTypes;
            }
        }

        protected void LoadColumns()
        {
            // Clear Columns
            this.Grid.Columns.Clear();

            foreach (Model.PropertyType proptype in this.PropertyTypes)
            {
                this.Grid.AddColumn(proptype.Name, proptype.Label, proptype.ColumnWidth);
            }

        }

        protected void LoadRows()
        {
            if (this.Binding != null)
            {
                // Get Current Items
                List<Model.Relationship> currentitems = ((Model.Item)this.Binding).Store(this.RelationshipType).CurrentItems().ToList();
                
                // Set Number of Rows in Grid
                this.Grid.NoRows = currentitems.Count();

                // Clear current Grid Selection
                this.Grid.SelectedRows.Clear();

                // Load Current Items into Grid
                for(int i=0; i < currentitems.Count(); i++)
                {
                    Model.Relationship relationship = currentitems[i];

                    int j = 0;

                    foreach (Model.PropertyType proptype in this.PropertyTypes)
                    {
                        Model.Property property = null;

                        if (relationship.ItemType.Equals(proptype.Type))
                        {
                            property = relationship.Property(proptype);
                        }
                        else
                        {
                            if (relationship.Related != null)
                            {
                                property = relationship.Related.Property(proptype);
                            }
                        }

                        if (property != null)
                        {
                            if (this.Grid.Rows[i].Cells[j].Value == null)
                            {
                                this.Grid.Rows[i].Cells[j].Value = this.Session.CreateProperty(property);
                            }
                            else
                            {
                                this.Grid.Rows[i].Cells[j].Value.Binding = property;
                            }
                        }
                        else
                        {
                            this.Grid.Rows[i].Cells[j].Value = null;
                        }

                        j++;
                    }
                }
            }
            else
            {
                // Clear all Rows
                this.Grid.NoRows = 0;
            }

            this.UpdateCommandsCanExecute();
        }

        public void EditRelationship()
        {
            if (this.Parent.Transaction != null)
            {
                if (this.Binding != null)
                {
                    foreach(Model.Relationship currentitem in ((Model.Item)this.Binding).Store(this.RelationshipType).CurrentItems())
                    {
                        currentitem.Update(this.Parent.Transaction);
                    }
                }
            }
        }

        public IEnumerable<Model.Relationship> Displayed
        {
            get
            {
                if (this.Binding != null)
                {
                    return ((Model.Item)this.Binding).Store(this.RelationshipType).CurrentItems();
                }
                else
                {
                    return new List<Model.Relationship>();
                }
            }
        }

        public Model.ObservableList<Model.Item> Selected { get; private set; }

        public void Select(Model.Relationship Relationship)
        {
            if (Relationship != null)
            {
                List<Model.Relationship> displayed = this.Displayed.ToList();
                int index = displayed.IndexOf(Relationship);

                if (index >= 0)
                {
                    this.Grid.SelectedRows.Clear();
                    this.Grid.SelectedRows.Add(this.Grid.Rows[index]);
                }
            }

            this.UpdateCommandsCanExecute();
        }

        private void SelectedRows_ListChanged(object sender, EventArgs e)
        {
            this.Selected.NotifyListChanged = false;
            this.Selected.Clear();

            List<Model.Relationship> relationships = this.Displayed.ToList();

            foreach (Row row in this.Grid.SelectedRows)
            {
                this.Selected.Add(relationships[row.Index]);
            }

            this.UpdateCommandsCanExecute();

            this.Selected.NotifyListChanged = true;
        }

        private void UpdateCommandsCanExecute()
        {
            if (this.Binding != null)
            {
                if (this.Parent.Transaction != null)
                {
                    this.Create.UpdateCanExecute(true);

                    if (this.Selected.Count() > 0)
                    {
                        this.Delete.UpdateCanExecute(true);
                    }
                    else
                    {
                        this.Delete.UpdateCanExecute(false);
                    }
                }
                else
                {
                    this.Create.UpdateCanExecute(false);
                    this.Delete.UpdateCanExecute(false);
                }
            }
            else
            {
                this.Create.UpdateCanExecute(false);
                this.Delete.UpdateCanExecute(false);
            }
        }

        private void CreateRelationship()
        {
            if (this.Binding != null)
            {
                if (this.Parent.Transaction != null)
                {
                    if (this.RelationshipType.RelatedItemType != null)
                    {
                        if (this.Dialog == null)
                        {
                            // Create Search Dialog
                            this.Dialog = new Dialogs.Searches.ItemType(this.Session);
                            this.Dialog.Binding = this.Session.Model.Store(this.RelationshipType.RelatedItemType);

                            // Watch for changes in selection
                            this.Dialog.Search.Selected.ListChanged += Selected_ListChanged;
                        }

                        // Open Search Dialog
                        this.Dialog.Open = true;
                    }
                    else
                    {
                        // Create null Relationship
                        Model.Relationship relationship = ((Model.Item)this.Binding).Store(this.RelationshipType).Create(null, this.Parent.Transaction);

                        // Load Rows
                        this.LoadRows();
                    }
                }
            }
        }

        private void Selected_ListChanged(object sender, EventArgs e)
        {
            if (this.Binding != null)
            {
                if (this.Dialog != null)
                {
                    if (this.Parent.Transaction != null)
                    {
                        foreach (Model.Item relateditem in this.Dialog.Search.Selected)
                        {
                            Model.Relationship relationship = ((Model.Item)this.Binding).Store(this.RelationshipType).Create(relateditem, this.Parent.Transaction);
                        }

                        this.LoadRows();
                    }

                    // Close Dialog
                    this.Dialog.Open = false;
                }
            }
        }

        private void DeleteRelationship()
        {
            if (this.Binding != null)
            {
                if (this.Parent.Transaction != null)
                {
                    // Delete Selected Relationships
                    foreach(Model.Relationship relationship in this.Selected)
                    {
                        relationship.Delete(this.Parent.Transaction);
                    }

                    // Load Roes
                    this.LoadRows();
                }
            }
        }

        private void Parent_Saved(object sender, EventArgs e)
        {
            this.UpdateCommandsCanExecute();
        }

        void Parent_Undone(object sender, EventArgs e)
        {
            this.UpdateCommandsCanExecute();
        }

        void Parent_Edited(object sender, EventArgs e)
        {

            this.UpdateCommandsCanExecute();
        }

        void Parent_Created(object sender, EventArgs e)
        {
            this.UpdateCommandsCanExecute();
        }

        public Relationship(IItemControl Parent, Model.RelationshipType RelationshipType)
            :base(Parent.Session)
        {
            // Only create Dialog when needed
            this._dialog = null;

            // Create Selected
            this.Selected = new Model.ObservableList<Model.Item>();
            
            // Create Comands
            this.Create = new CreateCommand(this);
            this.Delete = new DeleteCommand(this);

            // Store Parent
            this.Parent = Parent;
            
            // Watch Parent Events
            this.Parent.Created += Parent_Created;
            this.Parent.Edited += Parent_Edited;
            this.Parent.Undone += Parent_Undone;
            this.Parent.Saved += Parent_Saved;

            // Store RelationshipType
            this.RelationshipType = RelationshipType;

            // Create Grid
            this.Grid = new Grid(this.Session);
            this.Grid.SelectedRows.ListChanged += SelectedRows_ListChanged;
            this.Grid.AllowSelect = true;
            this.Grid.Width = this.Width;
            this.Children.Add(this.Grid);

            // Load Columns
            this.LoadColumns();
        }

        public class CreateCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Aras.ViewModel.Control> Parameters)
            {
                ((Relationship)this.Control).CreateRelationship();
                this.CanExecute = true;
            }

            internal CreateCommand(Relationship Relationship)
                : base(Relationship)
            {
                this.CanExecute = false;
            }
        }

        public class DeleteCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Aras.ViewModel.Control> Parameters)
            {
                ((Relationship)this.Control).DeleteRelationship();
                this.CanExecute = true;
            }

            internal DeleteCommand(Relationship Relationship)
                : base(Relationship)
            {
                this.CanExecute = false;
            }
        }
    }
}
