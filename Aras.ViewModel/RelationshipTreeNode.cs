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

        protected override void LoadChildren()
        {
            base.LoadChildren();

            if ((this.Binding != null) && (this.Binding is Model.Item))
            {
                // Build List of Children
                IEnumerable<Model.Item> childitems = ((Model.Item)this.Binding).RelatedItems(this.RelationshipTree.RelationshipTypes);

                this.Children.NotifyListChanged = false;

                if (childitems.Count() == 0)
                {
                    this.Children.Clear();
                }
                else
                {
                    int childitemcount = childitems.Count();
                    int childrencount = this.Children.Count();

                    if (childitemcount > childrencount)
                    {            
                        // Add Children
                        for (int i = 0; i < (childitemcount - childrencount); i++)
                        {
                            this.Children.Add(new RelationshipTreeNode(this.RelationshipTree));
                        }
                    }
                    else if (childitems.Count() < this.Children.Count())
                    {
                        // Remove Children
                        this.Children.RemoveRange(childitemcount, (childrencount - childitemcount));
                    }

                    // Set Bindings
                    for(int i=0; i<childitemcount; i++)
                    {
                        this.Children[i].Binding = childitems.ElementAt(i);
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
                if (this.Binding is Model.Item)
                {
                    this.Name = this.RelationshipTree.ItemFormatter.DisplayName((Model.Item)this.Binding);
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

            this.ChildrenLoaded = false;
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            if (this.Binding != null && this.Binding is Model.Item)
            {
                this.Name = this.RelationshipTree.ItemFormatter.DisplayName((Model.Item)this.Binding);
            }
            else
            {
                this.Name = "";
            }
        }

        public RelationshipTreeNode(RelationshipTree RelationshipTree)
            :base(RelationshipTree)
        {

        }
    }
}
