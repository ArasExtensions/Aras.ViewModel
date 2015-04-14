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
using System.Threading;
using System.Threading.Tasks;

namespace Aras.ViewModel.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Manager
            String URL = "http://localhost/innovatorserver100sp4";
            Manager manager = new Manager(URL);
            
            // Add Applications
            manager.LoadAssembly("AESSiS.PartViewer");

            // Get Database
            Model.Database database = manager.Database("Development100SP4");

            // Craete Session
            Session session = manager.Login(database, "admin", "innovator");

            // Get Application
            AESSiS.PartViewer.Application partviewapp = (AESSiS.PartViewer.Application)session.Applications.First();

            // Click Refresh 
            ViewModel.Search searchcontrol = (ViewModel.Search)session.Control(partviewapp.Search.Value.ID);
            ViewModel.Command searchcommand = session.Command(searchcontrol.Command("Refresh").ID);

            searchcommand.Execute();
            IEnumerable<ViewModel.Command> commandqueue1 = session.GetCommandsFromQueue();

            Thread.Sleep(5000);

            IEnumerable<ViewModel.Command> commandqueue2 = session.GetCommandsFromQueue();
            
        }
    }
}
