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

                    // Stop Notification
                    this._toolbar.Children.NotifyListChanged = false;

                    // Add Search Button
                    //Button searchbutton = new Button(this.Session);
                    //searchbutton.Icon = "Search";
                    //searchbutton.Tooltip = "Search";
                    //this._toolbar.Children.Add(searchbutton);
                    //searchbutton.Command = this.Refresh;

                    // Add Page Size
                    //this._toolbar.Children.Add(this.PageSize);

                    // Add Next Page Button
                    //Button nextbutton = new Button(this.Session);
                    //nextbutton.Icon = "NextPage";
                    //nextbutton.Tooltip = "Next Page";
                    //this._toolbar.Children.Add(nextbutton);
                    //nextbutton.Command = this.NextPage;

                    // Add Previous Page Button
                    //Button previousbutton = new Button(this.Session);
                    //previousbutton.Icon = "PreviousPage";
                    //previousbutton.Tooltip = "Previous Page";
                    //this._toolbar.Children.Add(previousbutton);
                    //previousbutton.Command = this.PreviousPage;

                    // Add Query String - disabled for now, need to work out how to query Related
                    // this._toolbar.Children.Add(this.QueryString);

                    //this._toolbar.Children.Add(new ToolbarSeparator(this.Session));

                    // Add Create Button
                    Button createbutton = new Button(this.Session);
                    createbutton.Icon = "New";
                    createbutton.Tooltip = "Create " + this.RelationshipType.SingularLabel;
                    this._toolbar.Children.Add(createbutton);
                    createbutton.Command = this.Create;

                    // Add Create Button
                    Button deletebutton = new Button(this.Session);
                    deletebutton.Icon = "Delete";
                    deletebutton.Tooltip = "Delete Selected " + this.RelationshipType.PluralLabel;
                    this._toolbar.Children.Add(deletebutton);
                    deletebutton.Command = this.Delete;

                    // Start Notification
                    this._toolbar.Children.NotifyListChanged = true;
                }

                return this._toolbar;
            }
        }

        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("NextPage")]
        public NextPageCommand NextPage { get; private set; }

        [ViewModel.Attributes.Command("PreviousPage")]
        public PreviousPageCommand PreviousPage { get; private set; }

        public Properties.String QueryString { get; private set; }

        public Properties.Integers.Spinner PageSize { get; private set; }

        public Properties.Integer Page { get; protected set; }

        public Properties.Integer NoPages { get; protected set; }

        [ViewModel.Attributes.Command("Create")]
        public CreateCommand Create { get; private set; }

        [ViewModel.Attributes.Command("Delete")]
        public DeleteCommand Delete { get; private set; }

        public Dialogs.Search Dialog { get; private set; }

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

        private Model.Condition Condition(Model.PropertyType PropertyType)
        {
            switch (PropertyType.GetType().Name)
            {
                case "String":
                case "Sequence":
                case "Text":
                    return Aras.Conditions.Like(PropertyType.Name, "%" + this.QueryString.Value + "%");
                case "Integer":
                    System.Int32 int32value = 0;

                    if (System.Int32.TryParse(this.QueryString.Value, out int32value))
                    {
                        return Aras.Conditions.Eq(PropertyType.Name, int32value);
                    }
                    else
                    {
                        return null;
                    }
                case "Decimal":
                    System.Decimal decimalvalue = 0;

                    if (System.Decimal.TryParse(this.QueryString.Value, out decimalvalue))
                    {
                        return Aras.Conditions.Eq(PropertyType.Name, decimalvalue);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        private Model.Query _query;
        private Model.Query Query
        {
            get
            {
                if (this._query == null)
                {
                    if (this.Binding != null)
                    {
                        Model.Item item = (Model.Item)this.Binding;
                        this._query = item.Store.Query;
                        this._query.PageSize = (System.Int32)this.PageSize.Value;
                        this._query.Paging = true;
                    }
                }

                return this._query;
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                ((Model.Item)this.Binding).PropertyChanged -= Relationship_PropertyChanged;
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Ensure Dialog is Closed
            if (this.Dialog != null)
            {
                this.Dialog.Open = false;
            }

            // Watch for Lock
            if (this.Binding != null)
            {
                ((Model.Item)this.Binding).PropertyChanged += Relationship_PropertyChanged;
            }

            this.LoadRows();
        }

        void Relationship_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Locked":
                    this.SetColumnEditing();
                    break;

                default:

                    break;
            }
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

                            if (this.RelationshipType.Related != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.Related.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.Sort();

                            break;
                        case Model.RelationshipGridViews.Right:

                            if (this.RelationshipType.Related != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.Related.RelationshipGridPropertyTypes);
                            }

                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            break;
                        default:
                            this._propertyTypes.AddRange(this.RelationshipType.RelationshipGridPropertyTypes);

                            if (this.RelationshipType.Related != null)
                            {
                                this._propertyTypes.AddRange(this.RelationshipType.Related.RelationshipGridPropertyTypes);
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
                this.Grid.AddColumn(proptype);
            }

            this.SetColumnEditing();
        }

        private void SetColumnEditing()
        {
            foreach (Model.PropertyType proptype in this.PropertyTypes)
            {
                if (proptype.Type.Equals(this.RelationshipType))
                {
                    Column col = this.Grid.Column(proptype.Name);

                    if (this.Binding != null)
                    {
                        Model.Item item = (Model.Item)this.Binding;

                        if (item.Locked == Model.Item.Locks.User)
                        {
                            col.Editable = true;
                        }
                        else
                        {
                            col.Editable = false;
                        }
                    }
                    else
                    {
                        col.Editable = false;
                    }
                }
            }
        }

        protected void LoadRows()
        {
            if (this.Binding != null)
            {
                Model.Item item = (Model.Item)this.Binding;
                List<Model.Item> currentitems = item.Relationships(this.RelationshipType).ToList();

                // Set Number of Rows in Grid
                this.Grid.NoRows = currentitems.Count();

                // Clear current Grid Selection
                this.Grid.SelectedRows.Clear();

                // Load Current Items into Grid
                for (int i = 0; i < this.Grid.NoRows; i++)
                {
                    Model.Relationship relationship = (Model.Relationship)currentitems[i];

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
                            this.Grid.Rows[i].Cells[j].Binding = property;
                        }
                        else
                        {
                            this.Grid.Rows[i].Cells[j].SetValue(null);
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

        protected void RefreshControl()
        {
            if (this.Query != null)
            {
                // Set Condition
                if (String.IsNullOrEmpty(this.QueryString.Value))
                {
                    this.Query.Condition = null;
                }
                else
                {
                    List<Model.Condition> conditions = new List<Model.Condition>();

                    foreach (Model.PropertyType proptype in this.Query.Store.ItemType.SearchPropertyTypes)
                    {
                        Model.Condition condition = this.Condition(proptype);

                        if (condition != null)
                        {
                            conditions.Add(condition);
                        }
                    }

                    if (conditions.Count() > 0)
                    {
                        if (conditions.Count() == 1)
                        {
                            this.Query.Condition = conditions[0];
                        }
                        else
                        {
                            this.Query.Condition = Aras.Conditions.Or(conditions[0], conditions[1]);

                            if (conditions.Count() > 2)
                            {
                                for (int i = 2; i < conditions.Count(); i++)
                                {
                                    ((Model.Conditions.Or)this.Query.Condition).Add(conditions[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Query.Condition = null;
                    }
                }

                // Set PageSize and required Page
                this.Query.Store.PageSize = (System.Int32)this.PageSize.Value;
                this.Query.Store.Page = (System.Int32)this.Page.Value;

                // Refresh Query
                this.Query.Store.Refresh();

                // Update NoPages
                this.NoPages.Value = this.Query.Store.NoPages;
            }
            else
            {
                this.NoPages.Value = 0;
            }

            // Load Grid
            this.LoadRows();

            // Refresh Buttons
            this.NextPage.Refesh();
            this.PreviousPage.Refesh();
        }

        public void EditRelationship()
        {
            if (this.Parent.Transaction != null)
            {
                if (this.Binding != null)
                {
                    foreach(Model.Relationship currentitem in ((Model.Item)this.Binding).Relationships(this.RelationshipType))
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
                    List<Model.Relationship> ret = new List<Model.Relationship>();

                    foreach (Model.Relationship rel in ((Model.Item)this.Binding).Relationships(this.RelationshipType))
                    {
                        ret.Add(rel);
                    }

                    return ret;
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
                    if (this.RelationshipType.Related != null)
                    {
                        if (this.Dialog == null)
                        {
                            // Create Search Dialog
                            //this.Dialog = new Dialogs.Search(this, this.Session.Model.Relationships(this.RelationshipType.RelatedItemType));

                            // Watch for changes in selection
                            this.Dialog.Grid.Selected.ListChanged += Selected_ListChanged;
                        }

                        // Open Search Dialog
                        this.Dialog.Open = true;
                    }
                    else
                    {
                        // Create null Relationship
                        //Model.Relationship relationship = ((Model.Item)this.Binding).Store(this.RelationshipType).Create(null, this.Parent.Transaction);

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
                        foreach (Model.Item relateditem in this.Dialog.Grid.Selected)
                        {
                            Model.Relationship relationship = (Model.Relationship)((Model.Item)this.Binding).Relationships(this.RelationshipType).Create(this.Parent.Transaction);
                            relationship.Related = relateditem;
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

        private void PageSize_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // Set Page to 1
                this.Page.Value = 1;

                this.RefreshControl();
            }
        }

        private void QueryString_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // Set Page to 1
                this.Page.Value = 1;

                this.RefreshControl();
            }
        }

        public Relationship(IItemControl Parent, Model.RelationshipType RelationshipType)
            :base(Parent.Session)
        {
            // Create Page
            this.Page = new Properties.Integer(this.Session);
            this.Page.Value = 1;

            // Create No Pages
            this.NoPages = new Properties.Integer(this.Session);
            this.NoPages.Value = 0;

            // Only create Dialog when needed
            this.Dialog = null;

            // Create Selected
            this.Selected = new Model.ObservableList<Model.Item>();
            
            // Create Comands
            this.Refresh = new RefreshCommand(this);
            this.NextPage = new NextPageCommand(this);
            this.PreviousPage = new PreviousPageCommand(this);
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

            // Create Query String
            this.QueryString = new Properties.String(this.Session);
            this.QueryString.Enabled = true;
            this.QueryString.IntermediateChanges = true;
            this.QueryString.Tooltip = "Search String";
            this.QueryString.PropertyChanged += QueryString_PropertyChanged;

            // Create Page Size
            this.PageSize = new Properties.Integers.Spinner(this.Session);
            this.PageSize.Tooltip = "Page Size";
            this.PageSize.Width = 40;
            this.PageSize.Enabled = true;
            this.PageSize.MinValue = 5;
            this.PageSize.MaxValue = 100;
            this.PageSize.Value = 25;
            this.PageSize.PropertyChanged += PageSize_PropertyChanged;

            // Load Columns
            this.LoadColumns();
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((Relationship)this.Control).RefreshControl();
                this.CanExecute = true;
            }

            internal RefreshCommand(Relationship Relationship)
                : base(Relationship)
            {
                this.CanExecute = true;
            }
        }

        public class NextPageCommand : Aras.ViewModel.Command
        {
            public Relationship Relationship
            {
                get
                {
                    return ((Relationship)this.Control);
                }
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Relationship.Page.Value < this.Relationship.NoPages.Value)
                {
                    this.Relationship.Page.Value = this.Relationship.Page.Value + 1;
                    this.Relationship.RefreshControl();
                }
            }

            internal void Refesh()
            {
                if (this.Relationship.Page.Value < this.Relationship.NoPages.Value)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            internal NextPageCommand(Relationship Relationship)
                : base(Relationship)
            {
                this.Refesh();
            }
        }

        public class PreviousPageCommand : Aras.ViewModel.Command
        {
            public Relationship Relationship
            {
                get
                {
                    return ((Relationship)this.Control);
                }
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (this.Relationship.Page.Value > 1)
                {
                    this.Relationship.Page.Value = this.Relationship.Page.Value - 1;
                    this.Relationship.RefreshControl();
                }
            }

            internal void Refesh()
            {
                if (this.Relationship.Page.Value > 1)
                {
                    this.CanExecute = true;
                }
                else
                {
                    this.CanExecute = false;
                }
            }

            internal PreviousPageCommand(Relationship Relationship)
                : base(Relationship)
            {
                this.Refesh();
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
