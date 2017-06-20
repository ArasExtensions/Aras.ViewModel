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
    [Attributes.ClientControl("Aras.View.Tree")]
    public abstract class Tree : Control
    {
        [ViewModel.Attributes.Property("Select", Aras.ViewModel.Attributes.PropertyTypes.Command, true)]
        [ViewModel.Attributes.Command("Select")]
        public SelectCommand Select { get; private set; }

        [Attributes.Property("Region", Attributes.PropertyTypes.Int32, true)]
        public Regions Region { get; set; }

        [Attributes.Property("Splitter", Attributes.PropertyTypes.Boolean, true)]
        public Boolean Splitter { get; set; }

        private Boolean _lazyLoad;
        [ViewModel.Attributes.Property("LazyLoad", Aras.ViewModel.Attributes.PropertyTypes.Boolean, true)]
        public Boolean LazyLoad
        {
            get
            {
                return this._lazyLoad;
            }
            set
            {
                if (this._lazyLoad != value)
                {
                    this._lazyLoad = value;
                    this.OnPropertyChanged("LazyLoad");
                }
            }
        }

        private Type _nodeFormatterType;
        public Type NodeFormatterType
        {
            get
            {
                return this._nodeFormatterType;
            }
            private set
            {
                if ((value != null) && value.IsSubclassOf(typeof(Aras.ViewModel.TreeNodeFormatter)))
                {
                    this._nodeFormatterType = value;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Node Formatter must be a sub class of Aras.ViewModel.TreeNodeFormatter");
                }
            }
        }

        private TreeNode _root;
        [ViewModel.Attributes.Property("Root", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public TreeNode Root 
        { 
            get
            {
                return this._root;
            }
            set
            {
                if (this._root == null)
                {
                    if (value != null)
                    {
                        this._root = value;
                        this.OnPropertyChanged("Root");
                    }
                }
                else
                {
                    if (!this._root.Equals(value))
                    {
                        this._root = value;
                        this.OnPropertyChanged("Root");
                    }
                }
            }
        }

        private TreeNode _selected;
        public TreeNode Selected
        {
            get
            {
                return this._selected;
            }
            protected set
            {
                if (value != null)
                {
                    if (!value.Equals(this._selected))
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

        public Tree(Manager.Session Session, Type NodeFormatterType)
            :base(Session)
        {
            this.Select = new SelectCommand(this);
            this._lazyLoad = true;
            this.NodeFormatterType = NodeFormatterType;
        }

        public class SelectCommand : Aras.ViewModel.Command
        {
            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                if ((Parameters != null) && (Parameters.Count() > 0))
                {
                    ((Tree)this.Control).Selected = (TreeNode)Parameters.First();
                }
                else
                {
                    ((Tree)this.Control).Selected = null;
                }

                // Set to Execute
                this.CanExecute = true;
            }

            internal SelectCommand(Tree Tree)
                : base(Tree)
            {
                this.CanExecute = true;
            }
        }
    }
}
