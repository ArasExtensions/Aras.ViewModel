using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Debug
{
    public class Session
    {
        private const String URL = "http://localhost/11SP1";

        public Boolean Execute()
        {
            // Create Logging
            Common.Logging.ILog log = new Common.Logging.Console.Log();

            // Create Server
            ViewModel.Server server = new Server(URL, log);

            // Add Applications
            server.LoadAssembly("Aras.ViewModel.Test");

            // Get Database
            ViewModel.Database database = server.Database("Development11SP1");

            // Craete Session
            ViewModel.Session session = database.Login("admin", Model.Database.PasswordHash("innovator"));

            // Get Application
            Aras.ViewModel.Test.TestSearch testsearch = (Aras.ViewModel.Test.TestSearch)session.Application("Aras.ViewModel.Test.TestSearch");

            // Run Search
            while (true)
            {
                Boolean test = testsearch.Search.Refresh.Execute();

                System.Console.WriteLine("Item Count: " + testsearch.Search.Items.Count());

                foreach (Row row in testsearch.Search.Grid.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        System.Console.Write(cell.Object + " ");
                    }

                    System.Console.WriteLine("");
                }

                System.Threading.Thread.Sleep(2000);
            }

            return true;
        }

        public Session()
        {
        }
    }
}
