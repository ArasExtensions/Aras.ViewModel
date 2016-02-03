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

namespace Aras.ViewModel.Design.Debug
{
    class Program
    {
        static void OutputOrder(Order Order)
        {
            foreach(Row row in Order.BOM.Rows)
            {
                System.Console.WriteLine(((Properties.String)row.Cells[2].Value).Value);
            }

            System.Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Model.Server server = new Model.Server("http://localhost/11SP1");
            server.LoadAssembly("Aras.Model.Design");
            server.LoadAssembly("Aras.ViewModel.Design");
            Model.Database database = server.Database("VariantsDemo11SP1");
            Model.Session session = database.Login("admin", Model.Server.PasswordHash("innovator"));

            session.ItemType("Part").AddToSelect("item_number,major_rev,name,keyed_name");

            Model.Stores.Item storeitem = session.Store("Part");

            Searches.Item partsearch = new Searches.Item();
            partsearch.AddToPropertyNames("item_number,major_rev,name");

            partsearch.Binding = storeitem;

            /*
            session.ItemType("Part").AddToSelect("item_number,major_rev,name,keyed_name");
            session.ItemType("Part BOM").AddToSelect("quantity");

            Model.Design.Part toplevel = (Model.Design.Part)session.Store("Part").Query(Aras.Conditions.Eq("item_number", "DX-0000000001")).First();
            ViewModel.RelationshipTree tree = new RelationshipTree();
            tree.AddRelationshipType(session.ItemType("Part").RelationshipType("Part BOM"));
            tree.LazyLoad = false;
            tree.Binding = toplevel;
            IEnumerable<TreeNode> children = tree.Node.Children;
            */
        }
    }
}
