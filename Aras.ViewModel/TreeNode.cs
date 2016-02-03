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

        private ObservableControlList<TreeNode> _children;
        [ViewModel.Attributes.Property("Name", Aras.ViewModel.Attributes.PropertyTypes.ControlList, true)]
        public ObservableControlList<TreeNode> Children
        {
            get
            {
                if (this._children == null)
                {
                    this._children = new ObservableControlList<TreeNode>();

                    if (!this.Tree.LazyLoad)
                    {
                        this.Load.Execute();
                    }
                }

                return this._children;
            }
        }

        [ViewModel.Attributes.Property("Loaded", Aras.ViewModel.Attributes.PropertyTypes.Boolean, true)]
        public Boolean Loaded { get; private set; }

        [ViewModel.Attributes.Command("Load")]
        public LoadCommand Load { get; private set; }

        protected virtual void LoadChildren()
        {

        }

        protected override void RefreshControl()
        {
            base.RefreshControl();
            this.LoadChildren();
        }

        public TreeNode(Tree Tree)
            :base()
        {
            this.Tree = Tree;
            this.Loaded = false;
        }

        public class LoadCommand : Aras.ViewModel.Command
        {
            public TreeNode TreeNode { get; private set; }

            protected override bool Run(object parameter)
            {
                if (!this.TreeNode.Loaded)
                {
                    this.TreeNode.LoadChildren();
                    this.TreeNode.Loaded = true;
                    this.SetCanExecute(false);
                }

                return true;
            }

            internal LoadCommand(TreeNode TreeNode)
            {
                this.TreeNode = TreeNode;
                this.SetCanExecute(true);
            }
        }
    }
}
