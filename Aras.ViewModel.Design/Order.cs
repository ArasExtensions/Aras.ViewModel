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

namespace Aras.ViewModel.Design
{
    public class Order : Control
    {
        [ViewModel.Attributes.Command("Refresh")]
        public RefreshCommand Refresh { get; private set; }

        [ViewModel.Attributes.Command("Save")]
        public SaveCommand Save { get; private set; }

        [ViewModel.Attributes.Property("BOM", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid BOM { get; private set; }

        [ViewModel.Attributes.Property("Configuration", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid Configuration { get; private set; }

        public override object Binding
        {
            get
            {
                return base.Binding;
            }
            set
            {
                if (value == null)
                {
                    base.Binding = value;
                }
                else
                {
                    if (value is Model.Design.Order)
                    {
                        base.Binding = value;
                    }
                    else
                    {
                        throw new Model.Exceptions.ArgumentException("Binding must be of type Aras.Model.Design.Order");
                    }
                }
            }
        }

        private ControlCache<Model.Design.OrderContext, Properties.String> QuestionCache;

        private void UpdateGrids()
        {
            // Get Order
            Model.Design.Order ordermodel = (Model.Design.Order)this.Binding;

            // Update Configuration Grid
            int cnt = 0;

            foreach (Model.Design.OrderContext ordercontext in ordermodel.Relationships("v_Order Context"))
            {
                Row row = null;

                if (this.Configuration.Rows.Count() < (cnt + 1))
                {
                    row = this.Configuration.AddRow();
                }
                else
                {
                    row = this.Configuration.Rows[cnt];
                }

                // Add Question
                Properties.String questioncontrol = this.QuestionCache.Get(ordercontext);
                questioncontrol.Binding = ordercontext.VariantContext.Property("question");
                


                cnt++;
            }

            // Update BOM Grid
            cnt = 0;

            foreach (Model.Design.PartBOM partbom in ordermodel.ConfiguredPart.Relationships("Part BOM"))
            {
                Row row = null;

                if (this.BOM.Rows.Count() < (cnt + 1))
                {
                    row = this.BOM.AddRow();
                }
                else
                {
                    row = this.BOM.Rows[cnt];
                }




                cnt++;
            }

        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.UpdateGrids();
            }
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                // Clear Grids
                this.Configuration.Rows.Clear();
                this.BOM.Rows.Clear();
            }
        }

        public Order()
            :base()
        {
            this.QuestionCache = new ControlCache<Model.Design.OrderContext, Properties.String>();

            this.Refresh = new RefreshCommand(this);
            this.Save = new SaveCommand(this);

            this.BOM = new Grid();
            this.BOM.AddColumn("number", "Number");
            this.BOM.AddColumn("revision", "Revision");
            this.BOM.AddColumn("name", "Name");
            this.BOM.AddColumn("quanity", "Qty");

            this.Configuration = new Grid();
            this.Configuration.AddColumn("rule", "Rule");
            this.Configuration.AddColumn("value", "Value");
            this.Configuration.AddColumn("quantity", "Qty");
        }

        public class RefreshCommand : Aras.ViewModel.Command
        {
            public Order Order { get; private set; }

            protected override bool Run(object parameter)
            {
                return true;
            }

            internal RefreshCommand(Order Order)
            {
                this.Order = Order;
                this.SetCanExecute(true);
            }
        }

        public class SaveCommand : Aras.ViewModel.Command
        {
            public Order Order { get; private set; }

            protected override bool Run(object parameter)
            {
                return true;
            }

            internal SaveCommand(Order Order)
            {
                this.Order = Order;
            }



        }
    }
}
