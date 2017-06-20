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

namespace Aras.ViewModel.Design.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Logging
            Logging.Console log = new Logging.Console();

            // Connect to Server
            ViewModel.Manager.Server server = new ViewModel.Manager.Server("http://localhost/11SP9", log);
            
            // Load Assemblies
            server.LoadAssembly("Aras.Model.Design");
            server.LoadAssembly("Aras.ViewModel.Design");
            
            // Login
            ViewModel.Manager.Database database = server.Database("Development");
            ViewModel.Manager.Session session = database.Login("admin", IO.Server.PasswordHash("innovator"));

            Model.Design.Queries.Searches.Part partquery = new Model.Design.Queries.Searches.Part(session.Model);
            Model.Design.Items.Part part = (Model.Design.Items.Part)partquery.Store.First();

            Model.Design.Queries.Trees.Part treequery = new Model.Design.Queries.Trees.Part(session.Model);
            treequery.Root = part;

            Model.Design.Items.Part treeroot = (Model.Design.Items.Part)treequery.Store.First();

            ViewModel.Trees.Relationship reltree = new Trees.Relationship(session, typeof(Aras.ViewModel.Design.NodeFormatters.Part));
            reltree.Binding = treeroot;

            TreeNode rootnode = reltree.Root;
            rootnode.Load.Execute();

            String test = rootnode.Label;
        }
    }
}
