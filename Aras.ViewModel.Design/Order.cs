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

        protected override Model.Item GetContext(Model.Session Sesison, string ID)
        {
            Model.Queries.Item query = Sesison.Query("v_Order", Aras.Conditions.Eq("id", ID));
            
            if (query.Count() == 1)
            {
                return query.First();
            }
            else
            {
                return null;
            }
        }

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

        // Configuration Control Caches
        private ControlCache<Model.Design.OrderContext, Properties.String> ConfigQuestionCache;
        private ControlCache<Model.Design.OrderContext, Properties.List> ConfigValueCache;
        private ControlCache<Model.Design.OrderContext, Properties.Float> ConfigQuantityCache;

        // PartBOM Control Caches
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMNumberCache;
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMRevisionCache;
        private ControlCache<Model.Design.PartBOM, Properties.String> PartBOMNameCache;
        private ControlCache<Model.Design.PartBOM, Properties.Float> PartBOMQuantityCache;

        private Model.Design.Order OrderModel
        {
            get
            {
                return (Model.Design.Order)this.Binding;
            }
        }

        private void UpdateBOMGrid()
        {
            if (this.OrderModel != null && this.OrderModel.ConfiguredPart != null)
            {
                // Set No of Rows
                int cnt = 0;

                foreach (Model.Design.PartBOM partbom in this.OrderModel.ConfiguredPart.Relationships("Part BOM"))
                {
                    if (partbom.Action != Model.Item.Actions.Deleted)
                    {
                        cnt++;
                    }
                }

                this.BOM.NoRows = cnt;

                // Update BOM Grid
                cnt = 0;

                foreach (Model.Design.PartBOM partbom in this.OrderModel.ConfiguredPart.Relationships("Part BOM"))
                {
                    if (partbom.Action != Model.Item.Actions.Deleted)
                    {
                        Row row = this.BOM.Rows[cnt];

                        // Add Part Number
                        Properties.String numbercontrol = this.PartBOMNumberCache.Get(partbom);
                        numbercontrol.Binding = partbom.Related.Property("item_number");
                        row.Cells[0].Value = numbercontrol;
                        numbercontrol.Enabled = false;

                        // Add Part Revision
                        Properties.String revisioncontrol = this.PartBOMRevisionCache.Get(partbom);
                        revisioncontrol.Binding = partbom.Related.Property("major_rev");
                        row.Cells[1].Value = revisioncontrol;
                        revisioncontrol.Enabled = false;

                        // Add Part Name
                        Properties.String namecontrol = this.PartBOMNameCache.Get(partbom);
                        namecontrol.Binding = partbom.Related.Property("cmb_name");
                        row.Cells[2].Value = namecontrol;
                        namecontrol.Enabled = false;

                        // Add Quantity
                        Properties.Float quantitycontrol = this.PartBOMQuantityCache.Get(partbom);
                        quantitycontrol.Binding = partbom.Property("quantity");
                        row.Cells[3].Value = quantitycontrol;
                        quantitycontrol.Enabled = false;

                        cnt++;
                    }
                }
            }
            else
            {
                this.BOM.NoRows = 0;
            }

            this.OnPropertyChanged("BOM");
        }

        private void UpdateConfigurationGrid()
        {
            // Update number of Rows
            this.Configuration.NoRows = this.OrderModel.Relationships("v_Order Context").Count();

            // Update Configuration Grid
            int cnt = 0;

            foreach (Model.Design.OrderContext ordercontext in this.OrderModel.Relationships("v_Order Context"))
            {
                Row row = this.Configuration.Rows[cnt];

                // Add Question
                Properties.String questioncontrol = this.ConfigQuestionCache.Get(ordercontext);
                questioncontrol.Binding = ordercontext.VariantContext.Property("question");
                row.Cells[0].Value = questioncontrol;
                
                // Add Values
                Properties.List valuecontrol = this.ConfigValueCache.Get(ordercontext);
                valuecontrol.Binding = ordercontext.Property("value_list");
                row.Cells[1].Value = valuecontrol;

                if (ordercontext.VariantContext.IsMethod || !this.OrderModel.Locked(false))
                {
                    valuecontrol.Enabled = false;
                }
                else
                {
                    valuecontrol.Enabled = true;
                }

                // Add Quantity
                Properties.Float quantitycontrol = this.ConfigQuantityCache.Get(ordercontext);
                quantitycontrol.Binding = ordercontext.Property("quantity");
                row.Cells[2].Value = quantitycontrol;

                if (ordercontext.VariantContext.IsMethod || !this.OrderModel.Locked(false))
                {
                    quantitycontrol.Enabled = false;
                }
                else
                {
                    quantitycontrol.Enabled = true;
                }

                cnt++;
            }

            this.OnPropertyChanged("Configuration");
        }

        private Model.Transaction Transaction;

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            if (this.Binding != null)
            {
                this.Update();

                // Add Event Handlers
                this.OrderModel.PropertyChanged += OrderModel_PropertyChanged;
                this.OrderModel.Relationships("v_Order Context").QueryChanged += OrderContext_QueryChanged;
            }
        }

        void OrderModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ConfiguredPart":
                    this.UpdateBOMGrid();
                    break;
                default:
                    break;
            }
        }

        void OrderContext_QueryChanged(object sender, EventArgs e)
        {
            this.UpdateConfigurationGrid();
        }

        protected override void BeforeBindingChanged()
        {
            base.BeforeBindingChanged();

            if (this.Binding != null)
            {
                // Rollbck Transaction

                if (this.Transaction != null)
                {
                    this.Transaction.RollBack();
                    this.Transaction = null;
                }

                // Remove Event Handlers
                this.OrderModel.Relationships("v_Order Context").QueryChanged -= OrderContext_QueryChanged;
                this.OrderModel.PropertyChanged -= OrderModel_PropertyChanged;

                // Clear Grids
                this.Configuration.Rows.Clear();
                this.BOM.Rows.Clear();
            }
        }

        private void Update()
        {
            this.OrderModel.Refresh();

            // Create Transaction if Order Locked
            if (this.OrderModel.Locked(true))
            {
                if (this.OrderModel.Transaction == null)
                {
                    this.Transaction = this.OrderModel.Session.BeginTransaction();
                    this.OrderModel.Update(this.Transaction);
                }
                else
                {
                    this.Transaction = this.OrderModel.Transaction;
                }

                this.Save.UpdateCanExecute(true);
            }
            else
            {
                this.Save.UpdateCanExecute(false);
            }

            // Update Grids
            this.UpdateConfigurationGrid();
            this.UpdateBOMGrid();
        }

        public Order()
            :base()
        {
            this.ConfigQuestionCache = new ControlCache<Model.Design.OrderContext, Properties.String>();
            this.ConfigValueCache = new ControlCache<Model.Design.OrderContext, Properties.List>();
            this.ConfigQuantityCache = new ControlCache<Model.Design.OrderContext, Properties.Float>();

            this.PartBOMNumberCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMRevisionCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMNameCache = new ControlCache<Model.Design.PartBOM, Properties.String>();
            this.PartBOMQuantityCache = new ControlCache<Model.Design.PartBOM, Properties.Float>();

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
                this.Order.Update();
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

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.SetCanExecute(CanExecute);
            }

            protected override bool Run(object parameter)
            {
                if (this.Order.Transaction != null)
                {
                    // Committ current transaction
                    this.Order.OrderModel.Transaction.Commit();

                    // Create new Transaction
                    this.Order.Transaction = this.Order.OrderModel.Session.BeginTransaction();
                    this.Order.OrderModel.Update(this.Order.Transaction);
                }

                return true;
            }

            internal SaveCommand(Order Order)
            {
                this.Order = Order;
                this.SetCanExecute(false);
            }
        }
    }
}
