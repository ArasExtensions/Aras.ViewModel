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
        static void Main(string[] args)
        {
            // Connect to Server
            Model.Server server = new Model.Server("http://localhost/11SP1");
            server.LoadAssembly("Aras.Model.Design");
            server.LoadAssembly("Aras.ViewModel.Design");
            Model.Database database = server.Database("Development11SP1");
            Model.Session session = database.Login("admin", Model.Server.PasswordHash("innovator"));

            session.ItemType("Document").AddToSelect("item_number,name,description");

            Model.Stores.Item<Model.Design.Document> store = new Model.Stores.Item<Model.Design.Document>(session, "Document", Aras.Conditions.Eq("item_number", "0000001"));
            Model.Design.Document sample = store.First();
            Model.Design.DocumentFile worddoc = sample.Files.First();

            Form form = new Form();
            form.AddPropertyNames("item_number,name,description");
            form.Binding = sample;

            form.Edit.Execute();
            ((ViewModel.Properties.String)form.Fields.Last()).Value = "New Description";
            form.Save.Execute();
        }
    }
}
