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
    public class RelationshipTreeNode : TreeNode
    {
        public RelationshipTree RelationshipTree
        {
            get
            {
                return (RelationshipTree)this.Tree;
            }
        }

        public Model.Relationship Relationship
        {
            get
            {
                if (this.Binding != null)
                {
                    if (this.Binding is Model.Relationship)
                    {
                        return (Model.Relationship)this.Binding;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public Model.Item Item
        {
            get
            {
                if (this.Binding != null)
                {
                    if (this.Binding is Model.Relationship)
                    {
                        return ((Model.Relationship)this.Binding).Related;
                    }
                    else
                    {
                        return (Model.Item)this.Binding;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        internal Model.Store Store { get; private set; }

        protected override void LoadChildren()
        {
            base.LoadChildren();

            /*
            if (this.Item != null)
            {
                this.Children.NotifyListChanged = false;

                this.Children.Clear();

                // Check Store has correct Source Item
                if (!this.Item.ID.Equals(this.Store.Source.ID))
                {
                    //this.Store = new Model.Stores.Relationship<Model.Relationship>(this.Item, this.RelationshipTree.RelationshipType);
                }

                // Refresh Store
                //this.Store.Refresh();

                foreach (Model.Relationship relationship in this.Store.CurrentItems())
                {
                    RelationshipTreeNode node = this.RelationshipTree.GetNodeFromCache(relationship.ID);

                    if (node == null)
                    {
                        node = new RelationshipTreeNode(this.RelationshipTree, this);
                        node.Binding = relationship;
                        this.RelationshipTree.AddNodeToCache(node);
                    }
                    else
                    {
                        node.Binding = relationship;
                    }

                    this.Children.Add(node);
                }

                this.Children.NotifyListChanged = true;
            } */
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Item != null)
            {
                // Stop watching current Item
                this.Item.Superceded -= Item_Superceded;
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if (this.Binding is Model.Relationship)
                {
                    this.Name = this.RelationshipTree.RelationshipFormatter.DisplayName(this.Relationship);
                    this.OpenIcon = this.Item.ItemType.Name.Replace(" ", "");
                    this.ClosedIcon = this.Item.ItemType.Name.Replace(" ", "");
                }
                else if (this.Binding is Model.Item)
                {
                    this.Name = this.RelationshipTree.ItemFormatter.DisplayName(this.Item);
                    this.OpenIcon = this.Item.ItemType.Name.Replace(" ", "");
                    this.ClosedIcon = this.Item.ItemType.Name.Replace(" ", "");
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Model.Item");
                }

                // Create Store
                //this.Store = new Model.Stores.Relationship<Model.Relationship>(this.Item, this.RelationshipTree.RelationshipType);

                // Watch for versioning on Item
                this.Item.Superceded += Item_Superceded;
            }
            else
            {
                this.Name = "";
                this.Store = null;
            }
        }

        void Item_Superceded(object sender, Model.SupercededEventArgs e)
        {
            if (this.Binding != null && this.Binding is Model.Item)
            {
                this.Binding = e.NewGeneration;
            }

            this.LoadChildren();
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            // Update Name
            if (this.Relationship != null)
            {
                this.Name = this.RelationshipTree.RelationshipFormatter.DisplayName(this.Relationship);
                this.OpenIcon = this.Item.ItemType.Name.Replace(" ", "");
                this.ClosedIcon = this.Item.ItemType.Name.Replace(" ", "");
            }
            else if (this.Item != null)
            {
                this.Name = this.RelationshipTree.ItemFormatter.DisplayName(this.Item);
                this.OpenIcon = this.Item.ItemType.Name.Replace(" ", "");
                this.ClosedIcon = this.Item.ItemType.Name.Replace(" ", "");
            }
            else
            {
                this.Name = "";
            }

        }

        public RelationshipTreeNode(RelationshipTree RelationshipTree, RelationshipTreeNode Parent)
            : base(RelationshipTree, Parent)
        {

        }
    }
}
