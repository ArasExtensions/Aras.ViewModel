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

namespace Aras.ViewModel
{
    public abstract class TreeNode : Control
    {
        public Tree Tree { get; private set; }

        public TreeNode Parent { get; private set; }

        public TreeNodeFormatter Formatter { get; private set; }

        private String _label;
        [ViewModel.Attributes.Property("Label", Aras.ViewModel.Attributes.PropertyTypes.String, true)]
        public String Label 
        { 
            get
            {
                return this._label;
            }
            set
            {
                if (this._label == null)
                {
                    if (value != null)
                    {
                        this._label = value;
                        this.OnPropertyChanged("Label");
                    }
                }
                else
                {
                    if (!this._label.Equals(value))
                    {
                        this._label = value;
                        this.OnPropertyChanged("Label");
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
                    this._children.ListChanged += Children_ListChanged;

                    if (!this.Tree.LazyLoad)
                    {
                        this.RunLoad();
                    }
                }

                return this._children;
            }
        }

        private void Children_ListChanged(object sender, EventArgs e)
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

        protected abstract void LoadChildren();

        private void RunLoad()
        {
            if (!this.ChildrenLoaded)
            {
                this.Children.Clear();
                this.LoadChildren();
                this.ChildrenLoaded = true;
            }
        }

        private void RunRefresh()
        {
            if (this.ChildrenLoaded)
            {
                this.ChildrenLoaded = false;
                this.RunLoad();
            
                foreach(TreeNode child in this.Children)
                {
                    child.RunRefresh();
                }
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            this.ChildrenLoaded = false;
            this.Children.Clear();
        }

        private void Formatter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Label":
                    this.Label = this.Formatter.Label;
                    break;
                case "OpenIcon":
                    this.OpenIcon = this.Formatter.OpenIcon;
                    break;
                case "ClosedIcon":
                    this.ClosedIcon = this.Formatter.ClosedIcon;
                    break;
                default:
                    break;
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

            // Create Formatter
            this.Formatter = (Aras.ViewModel.TreeNodeFormatter)this.Tree.NodeFormatterType.GetConstructor(new Type[] { typeof(Aras.ViewModel.TreeNode) }).Invoke(new object[] { this });
            this.Label = this.Formatter.Label;
            this.OpenIcon = this.Formatter.OpenIcon;
            this.ClosedIcon = this.Formatter.ClosedIcon;
            this.Formatter.PropertyChanged += Formatter_PropertyChanged;
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            protected override void Run(IEnumerable<Control> Parameters)
            {
                ((TreeNode)this.Control).RunRefresh();
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
                this.TreeNode.RunLoad();
                this.CanExecute = false;
            }

            internal LoadCommand(TreeNode TreeNode)
                :base(TreeNode)
            {
                this.CanExecute = true;
            }
        }
    }
}
