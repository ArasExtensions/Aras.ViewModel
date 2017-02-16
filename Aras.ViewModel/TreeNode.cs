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
    public abstract class TreeNode : Control
    {
        public Tree Tree { get; private set; }

        public TreeNode Parent { get; private set; }

        private String _name;
        [ViewModel.Attributes.Property("Name", Aras.ViewModel.Attributes.PropertyTypes.String, true)]
        public String Name 
        { 
            get
            {
                return this._name;
            }
            set
            {
                if (this._name == null)
                {
                    if (value != null)
                    {
                        this._name = value;
                        this.OnPropertyChanged("Name");
                    }
                }
                else
                {
                    if (!this._name.Equals(value))
                    {
                        this._name = value;
                        this.OnPropertyChanged("Name");
                    }
                }
            }
        }

        private String _closedIcon;
        [ViewModel.Attributes.Property("ClosedIcon", Aras.ViewModel.Attributes.PropertyTypes.String, true)]
        public String ClosedIcon
        {
            get
            {
                return this._closedIcon;
            }
            set
            {
                if (this._closedIcon == null)
                {
                    if (value != null)
                    {
                        this._closedIcon = value;
                        this.OnPropertyChanged("ClosedIcon");
                    }
                }
                else
                {
                    if (!this._closedIcon.Equals(value))
                    {
                        this._closedIcon = value;
                        this.OnPropertyChanged("ClosedIcon");
                    }
                }
            }
        }

        private String _openIcon;
        [ViewModel.Attributes.Property("OpenIcon", Aras.ViewModel.Attributes.PropertyTypes.String, true)]
        public String OpenIcon
        {
            get
            {
                return this._openIcon;
            }
            set
            {
                if (this._openIcon == null)
                {
                    if (value != null)
                    {
                        this._openIcon = value;
                        this.OnPropertyChanged("OpenIcon");
                    }
                }
                else
                {
                    if (!this._openIcon.Equals(value))
                    {
                        this._openIcon = value;
                        this.OnPropertyChanged("OpenIcon");
                    }
                }
            }
        }

        private ObservableControlList<TreeNode> _children;
        [ViewModel.Attributes.Property("Children", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public ObservableControlList<TreeNode> Children
        {
            get
            {
                if (this._children == null)
                {
                    this._children = new ObservableControlList<TreeNode>();
                    this._children.ListChanged += _children_ListChanged;

                    if (!this.Tree.LazyLoad)
                    {
                        this.Load.Execute();
                    }
                }

                return this._children;
            }
        }

        private void _children_ListChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Children");
        }

        private Boolean _childrenLoaded;
        [ViewModel.Attributes.Property("ChildrenLoaded", Aras.ViewModel.Attributes.PropertyTypes.Boolean, true)]
        public Boolean ChildrenLoaded 
        { 
            get
            {
                return this._childrenLoaded;
            }
            private set
            {
                if (this._childrenLoaded != value)
                {
                    this._childrenLoaded = value;
                    this.OnPropertyChanged("ChildrenLoaded");
                }
            }
        }

        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("Load")]
        public LoadCommand Load { get; private set; }

        protected virtual void LoadChildren()
        {

        }

        public void RefreshChildren()
        {
            this.LoadChildren();
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.ChildrenLoaded = false;
        }

        protected virtual void RefreshControl()
        {
            // Load Children
            this.LoadChildren();

            // Refresh Children
            foreach (TreeNode child in this.Children)
            {
                child.RefreshControl();
            }
        }

        public TreeNode(Tree Tree, TreeNode Parent)
            :base(Tree.Session)
        {
            this.Tree = Tree;
            this.Parent = Parent;
            this.ChildrenLoaded = false;
            this.Refresh = new RefreshCommand(this);
            this.Load = new LoadCommand(this);
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((TreeNode)this.Control).RefreshControl();
                this.CanExecute = true;
            }

            internal RefreshCommand(Control Control)
                : base(Control)
            {
                this.CanExecute = true;
            }
        }

        public class LoadCommand : Aras.ViewModel.Command
        {
            public TreeNode TreeNode
            {
                get
                {
                    return ((TreeNode)this.Control);
                }
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if (!this.TreeNode.ChildrenLoaded)
                {
                    this.TreeNode.LoadChildren();
                    this.TreeNode.ChildrenLoaded = true;
                    this.CanExecute = false;
                }
            }

            internal LoadCommand(TreeNode TreeNode)
                :base(TreeNode)
            {
                this.CanExecute = true;
            }
        }
    }
}
