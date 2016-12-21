/*  
  Aras.Model provides a .NET cient library for Aras Innovator

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

namespace Aras.ViewModel.Design.Properties
{
    [Attributes.ClientControl("Aras.View.Properties.List")]
    public class OrderContextList : ViewModel.Properties.List
    {
        protected override void CheckBinding(object Binding)
        {
            if (!(Binding is Model.Design.OrderContext))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be of type Model.Design.OrderContext");
            }
        }

        protected override void OnAfterSetValue()
        {
            if (this.Binding != null)
            {
                ((Model.Design.OrderContext)this.Binding).Value = this.Value;
            }
        }

        protected override void AfterBindingChanged()
        {
            if (this.Binding != null)
            {
                switch (((Model.Design.OrderContext)this.Binding).VariantContext.ContextType.Value)
                {
                    case "List":

                        // Get list and selected value
                        Model.List list = ((Model.Design.OrderContext)this.Binding).VariantContext.List;
                        Model.ListValue selectedvalue = list.ListValue(((Model.Design.OrderContext)this.Binding).Value);

                        this.Values.Clear();

                        this.Values.NotifyListChanged = false;

                        foreach (Model.ListValue modellistvalue in list.Values)
                        {
                            ViewModel.Properties.ListValue listvalue = new ViewModel.Properties.ListValue();
                            listvalue.Binding = modellistvalue;
                            this.Values.Add(listvalue);

                            if (modellistvalue.Equals(selectedvalue))
                            {
                                this.Value = listvalue.Value;
                            }
                        }

                        break;

                    case "Quantity":

                        // Add True
                        ViewModel.Properties.ListValue quantityvalue = new ViewModel.Properties.ListValue();
                        quantityvalue.Label = "Yes";
                        quantityvalue.Value = "1";
                        this.Values.Add(quantityvalue);

                        // Set Value
                        this.Value = ((Model.Design.OrderContext)this.Binding).Value;

                        break;

                    case "Boolean":
                    case "Method":

                        // Add False
                        ViewModel.Properties.ListValue falsevalue = new ViewModel.Properties.ListValue();
                        falsevalue.Label = "No";
                        falsevalue.Value = "0";
                        this.Values.Add(falsevalue);

                        // Add True
                        ViewModel.Properties.ListValue truevalue = new ViewModel.Properties.ListValue();
                        truevalue.Label = "Yes";
                        truevalue.Value = "1";
                        this.Values.Add(truevalue);

                        // Set Value
                        this.Value = ((Model.Design.OrderContext)this.Binding).Value;

                        break;
                    default:
                        throw new Model.Exceptions.ArgumentException("Invalid Variant Context Type: " + ((Model.Design.OrderContext)this.Binding).VariantContext.ContextType.Value);
                }

                this.Values.NotifyListChanged = true;
            }
        }

        protected override void RefreshControl()
        {
            base.RefreshControl();

            // Set Value
            this.Value = ((Model.Design.OrderContext)this.Binding).Value;
        }

        public OrderContextList()
            : base()
        {

        }
    }
}
