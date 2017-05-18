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

namespace Aras.ViewModel.Design.RelationshipFormatters
{
    public class Part : IRelationshipFormatter
    {
        public String DisplayName(Model.Relationship Relationship)
        {
            if (Relationship != null)
            {
                if (Relationship.Related != null)
                {
                    if (Relationship.ItemType.Name.Equals("Part BOM"))
                    {
                        Double? quantity = (Double?)Relationship.Property("quantity").Value;
                        String item_number = (String)Relationship.Related.Property("item_number").Value;

                        if ((quantity != null) && (quantity > 1))
                        {
                            return item_number + "." + Relationship.Related.MajorRev + " " + " " + (String)Relationship.Related.Property("name").Value + " (" + quantity.ToString() + ")";
                        }
                        else
                        {
                            return item_number + "." + Relationship.Related.MajorRev + " " + " " + (String)Relationship.Related.Property("name").Value;
                        }
                    }
                    else
                    {
                        return Relationship.KeyedName;
                    }

                }
                else
                {
                    return Relationship.KeyedName;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
