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

namespace Aras.ViewModel.Manager.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            Logging.Console log = new Logging.Console();
            Server server = new Server("http://localhost/InnovatorServer10SP4", log);
            server.LoadAssembly("Aras.Model.Design");
            server.LoadAssembly("Aras.ViewModel.Design");
            Database database = server.Database("CMB");
            Session session = database.Login("admin", Model.Server.PasswordHash("innovator"));

            ViewModel.Design.Order order = (ViewModel.Design.Order)session.Plugin("Aras.ViewModel.Design.Order", "B4F27233C77C4E768D85984CCD5C4CBA");
            order.Refresh.Execute();
            //String test1 = ((ViewModel.Properties.String)order.BOM.Rows[6].Cells[2].Value).Value;
            //Cell cell = order.Configuration.Rows[0].Cells[1];
            //ViewModel.Properties.List list = (ViewModel.Properties.List)cell.Value;
            //list.Value = list.Values[2].Value;
            //String test2 = ((ViewModel.Properties.String)order.BOM.Rows[6].Cells[2].Value).Value;


        }
    }
}
