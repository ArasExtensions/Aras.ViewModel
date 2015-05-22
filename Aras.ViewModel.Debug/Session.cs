using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Debug
{
    public class Session
    {
        private const String URL = "http://localhost/innovatorserver100sp4";

        public async Task<Boolean> Execute()
        {
            // Create Server
            ViewModel.Server server = new Server(URL);

            // Add Applications
            server.LoadAssembly("Aras.ViewModel.Test");

            // Get Database
            ViewModel.Database database = server.Database("Development100SP4");

            // Craete Session
            ViewModel.Session session = database.Login("admin", "innovator");

            // Get Application
            Aras.ViewModel.Test.TestSearch testsearch = (Aras.ViewModel.Test.TestSearch)session.Applications.First();

            int cnt = 0;

            while (cnt < 10)
            {
                Boolean test = await testsearch.Search.Refresh.ExecuteAsync();
                System.Threading.Thread.Sleep(2000);
                System.Console.WriteLine("Item Count: " + testsearch.Search.Items.Count());
                cnt++;
            }

            return true;
        }

        public Session()
        {
        }
    }
}
