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
            Model.Server server = new Model.Server("http://localhost/11SP1");
            server.LoadAssembly("Aras.Model.Design");
            Model.Database database = server.Database("VariantsDemo11SP1");
            Model.Session session = database.Login("admin", Model.Server.PasswordHash("innovator"));

            session.ItemType("v_Order").AddToSelect("keyed_name,item_number,name,part,locked_by_id,configured_part");
            session.ItemType("v_Order Context").AddToSelect("quantity");
            session.ItemType("Variant Context").AddToSelect("name,keyed_name,context_type,list,method,question");
            session.ItemType("Part Variants").AddToSelect("quantity");
            session.ItemType("Part Variant Rule").AddToSelect("value");
            session.ItemType("Part").AddToSelect("item_number,major_rev,name");
            session.ItemType("Part BOM").AddToSelect("quantity");

            Model.Design.Order order = (Model.Design.Order)session.Query("v_Order", Aras.Conditions.Eq("item_number", "0002")).First();

            Order ordercontrol = new Order();
            ordercontrol.Binding = order;
            ((Properties.List)ordercontrol.Configuration.Rows[0].Cells[1].Value).Value = "Gas";
            ordercontrol.Save.Execute();
   
        }
    }
}
