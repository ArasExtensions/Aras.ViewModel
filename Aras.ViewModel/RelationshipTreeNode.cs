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

        protected override void LoadChildren()
        {
            base.LoadChildren();

            if (this.Item != null)
            {
                // Build List of Relationships
                IEnumerable<Model.Relationship> relationships = this.Item.Relationships(this.RelationshipTree.RelationshipTypes);

                this.Children.NotifyListChanged = false;

                if (relationships.Count() == 0)
                {
                    this.Children.Clear();
                }
                else
                {
                    int childitemcount = relationships.Count();
                    int childrencount = this.Children.Count();

                    if (childitemcount > childrencount)
                    {            
                        // Add Children
                        for (int i = 0; i < (childitemcount - childrencount); i++)
                        {
                            this.Children.Add(new RelationshipTreeNode(this.RelationshipTree, this));
                        }
                    }
                    else if (relationships.Count() < this.Children.Count())
                    {
                        // Remove Children
                        this.Children.RemoveRange(childitemcount, (childrencount - childitemcount));
                    }

                    // Set Bindings
                    for(int i=0; i<childitemcount; i++)
                    {
                        this.Children[i].Binding = relationships.ElementAt(i);
                    }
                }

                this.Children.NotifyListChanged = true;
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                if ((this.Binding is Model.Item) || (this.Binding is Model.Relationship))
                {
                    this.Name = this.RelationshipTree.ItemFormatter.DisplayName(this.Item);
                    this.OpenIcon = this.Item.ItemType.Name.Replace(" ", "");
                    this.ClosedIcon = this.Item.ItemType.Name.Replace(" ", "");
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Binding must be of type Model.Item");
                }
            }
            else
            {
                this.Name = "";
            }
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            if (this.Item != null)
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
