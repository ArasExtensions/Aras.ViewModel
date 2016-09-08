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
    public class Order : Item
    {
        [ViewModel.Attributes.Command("Update")]
        public UpdateCommand Update { get; private set; }

        private ViewModel.Grid _bOM;
        [ViewModel.Attributes.Property("BOM", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid BOM
        {
            get
            {
                if (this._bOM == null)
                {
                    this._bOM = new Grid();
                    this.OnPropertyChanged("BOM");
                    this._bOM.AllowSelect = false;
                    this._bOM.AddColumn("number", "Number");
                    this._bOM.AddColumn("revision", "Revision");
                    this._bOM.AddColumn("name", "Name");
                    this._bOM.AddColumn("quantity", "Qty");
                }

                return this._bOM;
            }
        }

        private ViewModel.Grid _configuration;
        [ViewModel.Attributes.Property("Configuration", Aras.ViewModel.Attributes.PropertyTypes.Control, true)]
        public ViewModel.Grid Configuration
        {
            get
            {
                if (this._configuration == null)
                {
                    this._configuration = new Grid();
                    this.OnPropertyChanged("Configuration");
                    this._configuration.AllowSelect = false;
                    this._configuration.AddColumn("rule", "Item");
                    this._configuration.AddColumn("value", "Required");
                    this._configuration.AddColumn("quantity", "Qty");
                }

                return this._configuration;
            }
        }

        public override void SetBinding(Model.Session Sesison, String ID)
        {
            try
            {
                this.Binding = Sesison.Cache("v_Order").Get(ID);
            }
            catch (Exception e)
            {
                throw new Model.Exceptions.ArgumentException("Invalid Context Item: " + ID, e);
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Design.Order))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be of type Model.Design.Order");
            }
        }

        protected override void AfterBindingChanged()
        {
            base.AfterBindingChanged();

            // Ensure Required Properties are Selected
            if (this.ModelSession != null)
            {
                this.ModelSession.ItemType("v_Order Context").AddToSelect("quantity,value,locked_by_id");
                this.ModelSession.ItemType("Variant Context").AddToSelect("context_type,min_quantity,max_quantity,sort_order");
                this.ModelSession.ItemType("Part").AddToSelect("item_number,locked_by_id");
                this.ModelSession.ItemType("Part Variants").AddToSelect("quantity");
                this.ModelSession.ItemType("Part BOM").AddToSelect("quantity,locked_by_id,sort_order");
                this.ModelSession.ItemType("User").AddToSelect("keyed_name");
            }

            // Set Top Level Part
            this.SetTopLevelPart();

            // Set Configured Part
            this.SetConfiguredPart();

            // Lock Items
            this.LockItems();

            // Update Configuration Grid
            this.UpdateConfigurationGrid();

            // Update BOM Grid
            this.UpdateBOMGrid();

            // Update Commands
            this.SetCommandsCanExecute();
        }

        protected override void AfterSave()
        {
            base.AfterSave();
        }

        protected override void AfterEdit()
        {
            base.AfterEdit();

            // Lock Items
            this.LockItems();
        }

        private void LockItems()
        {
            // Lock all Variant Orders
            if ((this.OrderContexts != null) && (this.ModelTransaction != null))
            {
                foreach (Model.Design.OrderContext ordercontext in this.OrderContexts.CurrentItems())
                {
                    ordercontext.Update(this.ModelTransaction);
                }
            }

            // Lock Configured Part
            if (this.ConfiguredPart != null)
            {
                this.ConfiguredPart.Update(this.ModelTransaction);
            }
        }

        private Model.Design.Part ConfiguredPart;   
        private Model.Stores.Relationship<Model.Design.PartBOM> ConfiguredPartBOMS;

        private void SetConfiguredPart()
        {
            if (this.ModelItem != null)
            {
                Model.Design.Part configured_part = (Model.Design.Part)this.ModelItem.Property("configured_part").Value;

                if (configured_part != null)
                {
                    if (!configured_part.Equals(this.ConfiguredPart))
                    {
                        this.ConfiguredPart = configured_part;

                        // Store for Configured BOMS
                        this.ConfiguredPartBOMS = new Model.Stores.Relationship<Model.Design.PartBOM>(this.ConfiguredPart, "Part BOM");
                    }
                }
                else
                {
                    this.ConfiguredPart = null;
                    this.ConfiguredPartBOMS = null;
                }
            }
            else
            {
                this.ConfiguredPart = null;
                this.ConfiguredPartBOMS = null;
            }
        }

        private Model.Design.Part TopLevelPart;
        private Model.Stores.Relationship<Model.Design.OrderContext> OrderContexts;

        private void SetTopLevelPart()
        {
            if (this.ModelItem != null)
            {
                Model.Design.Part top_level_part = (Model.Design.Part)this.ModelItem.Property("part").Value;

                if (top_level_part != null)
                {
                    if (!top_level_part.Equals(this.TopLevelPart))
                    {
                        this.TopLevelPart = top_level_part;

                        // Store for Order Contexts
                        this.OrderContexts = new Model.Stores.Relationship<Model.Design.OrderContext>(this.ModelItem, "v_Order Context");

                        // Watch for changes in Top Level Part
                        this.TopLevelPart.PropertyChanged += TopLevelPart_PropertyChanged;
                    }
                }
                else
                {
                    if (this.TopLevelPart != null)
                    {
                        this.TopLevelPart.PropertyChanged -= TopLevelPart_PropertyChanged;
                    }

                    this.TopLevelPart = null;
                    this.OrderContexts = null;
                }
            }
            else
            {
                if (this.TopLevelPart != null)
                {
                    this.TopLevelPart.PropertyChanged -= TopLevelPart_PropertyChanged;
                }

                this.TopLevelPart = null;
                this.ConfiguredPart = null;
            }
        }

        private void TopLevelPart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Watch for changes in Top Level Part
            switch(e.PropertyName)
            {
                case "part":
                    this.SetTopLevelPart();
                    this.SetConfiguredPart();
                    this.UpdateConfigurationGrid();
                    this.UpdateBOMGrid();
                    break;
                default:
                    break;
            }
        }

        // PartBOM Control Caches
        private ControlCache<Model.Design.PartBOM, ViewModel.Properties.String> PartBOMNumberCache;
        private ControlCache<Model.Design.PartBOM, ViewModel.Properties.String> PartBOMRevisionCache;
        private ControlCache<Model.Design.PartBOM, ViewModel.Properties.String> PartBOMNameCache;
        private ControlCache<Model.Design.PartBOM, ViewModel.Properties.Float> PartBOMQuantityCache;

        private void UpdateBOMGrid()
        {
            if (this.ConfiguredPartBOMS != null)
            {
                IEnumerable<Model.Item> currentpartboms = this.ConfiguredPartBOMS.CurrentItems();

                // Set No of Rows
                this.BOM.NoRows = currentpartboms.Count();

                // Update BOM Grid
                int cnt = 0;

                foreach (Model.Design.PartBOM partbom in currentpartboms)
                {
                    Row row = this.BOM.Rows[cnt];

                    // Add Part Number
                    ViewModel.Properties.String numbercontrol = this.PartBOMNumberCache.Get(partbom);
                    numbercontrol.Binding = partbom.Related.Property("item_number");
                    row.Cells[0].Value = numbercontrol;
                    numbercontrol.Enabled = false;

                    // Add Part Revision
                    ViewModel.Properties.String revisioncontrol = this.PartBOMRevisionCache.Get(partbom);
                    revisioncontrol.Binding = partbom.Related.Property("major_rev");
                    row.Cells[1].Value = revisioncontrol;
                    revisioncontrol.Enabled = false;

                    // Add Part Name
                    ViewModel.Properties.String namecontrol = this.PartBOMNameCache.Get(partbom);
                    namecontrol.Binding = partbom.Related.Property("cmb_name");
                    row.Cells[2].Value = namecontrol;
                    namecontrol.Enabled = false;

                    // Add Quantity
                    ViewModel.Properties.Float quantitycontrol = this.PartBOMQuantityCache.Get(partbom);
                    quantitycontrol.Binding = partbom.Property("quantity");
                    row.Cells[3].Value = quantitycontrol;
                    quantitycontrol.Enabled = false;

                    cnt++;
                }
            }
            else
            {
                this.BOM.NoRows = 0;
            }

            this.OnPropertyChanged("BOM");
        }

        private ControlCache<Model.Design.OrderContext, ViewModel.Properties.String> ConfigQuestionCache;
        private ControlCache<Model.Design.OrderContext, Properties.OrderContextList> ConfigValueCache;
        private ControlCache<Model.Design.OrderContext, ViewModel.Properties.Float> ConfigQuantityCache;

        private void UpdateConfigurationGrid()
        {
            // Build List of OrderContext to Display
            List<Model.Design.OrderContext> ordercontexts = new List<Model.Design.OrderContext>();

            foreach (Model.Design.OrderContext ordercontext in this.OrderContexts.CurrentItems())
            {
                if (!ordercontext.VariantContext.IsMethod)
                {
                    ordercontexts.Add(ordercontext);
                }
            }

            // Order
            ordercontexts.Sort(
                delegate(Model.Design.OrderContext p1, Model.Design.OrderContext p2)
                {
                    return p1.VariantContext.SortOrder.CompareTo(p2.VariantContext.SortOrder);
                }
            );

            // Update number of Rows
            this.Configuration.NoRows = ordercontexts.Count();

            // Update Configuration Grid
            int cnt = 0;

            foreach (Model.Design.OrderContext ordercontext in ordercontexts)
            {
                Row row = this.Configuration.Rows[cnt];

                // Add Question
                ViewModel.Properties.String questioncontrol = this.ConfigQuestionCache.Get(ordercontext);

                if (!String.IsNullOrEmpty((String)ordercontext.VariantContext.Property("question").Value))
                {
                    questioncontrol.Binding = ordercontext.VariantContext.Property("question");
                }
                else
                {
                    questioncontrol.Binding = ordercontext.VariantContext.Property("name");
                }

                row.Cells[0].Value = questioncontrol;

                // Add Values
                Properties.OrderContextList valuecontrol = this.ConfigValueCache.Get(ordercontext);
                valuecontrol.Binding = ordercontext;
                row.Cells[1].Value = valuecontrol;

                if (this.ModelItem.Locked(false))
                {
                    valuecontrol.Enabled = true;
                }
                else
                {
                    valuecontrol.Enabled = false;
                }

                // Add Quantity
                ViewModel.Properties.Float quantitycontrol = this.ConfigQuantityCache.Get(ordercontext);
                quantitycontrol.Binding = ordercontext.Property("quantity");
                row.Cells[2].Value = quantitycontrol;

                // Add Min Max Quantity
                quantitycontrol.MinValue = (System.Double)ordercontext.VariantContext.MinQuantity;
                quantitycontrol.MaxValue = (System.Double)ordercontext.VariantContext.MaxQuantity;

                if (this.ModelItem.Locked(false))
                {
                    quantitycontrol.Enabled = true;
                }
                else
                {
                    quantitycontrol.Enabled = false;
                }

                cnt++;
            }

            this.OnPropertyChanged("Configuration");
        }

        // Caches to hold current configured state
        private Dictionary<Model.Design.Part, Double> PartCache;
        private Dictionary<Model.Design.Part, Model.Stores.Relationship<Model.Design.PartBOM>> BOMStoreCache;
        private Dictionary<Model.Design.Part, Model.Stores.Relationship<Model.Design.PartVariant>> VariantStoreCache;
        private Dictionary<Model.Design.PartVariant, Model.Stores.Relationship<Model.Design.PartVariantRule>> PartVariantRuleCache;

        private void RefeshPartCache()
        {
            this.PartCache.Clear();

            if (this.ModelItem != null)
            {
                this.RefeshPart(this.TopLevelPart, 1.0);
            }

        }

        private Model.Design.OrderContext OrderContext(Model.Design.VariantContext VariantContext)
        {
            Model.Design.OrderContext ret = null;

            foreach(Model.Design.OrderContext ordercontext in this.OrderContexts)
            {
                if (ordercontext.VariantContext.Equals(VariantContext))
                {
                    ret = ordercontext;
                    break;
                }
            }

            if (ret == null)
            {
                ret = this.OrderContexts.Create(VariantContext, this.ModelTransaction);
            }

            return ret;
        }

        private void RefeshPart(Model.Design.Part Part, Double Quantity)
        {
            // Process Part Variants
            if (Part.IsVariant)
            {
                if (!this.VariantStoreCache.ContainsKey(Part))
                {
                    this.VariantStoreCache[Part] = new Model.Stores.Relationship<Model.Design.PartVariant>(Part, "Part Variants");
                }

                foreach (Model.Design.PartVariant partvariant in this.VariantStoreCache[Part])
                {
                    if (!this.PartVariantRuleCache.ContainsKey(partvariant))
                    {
                        this.PartVariantRuleCache[partvariant] = new Model.Stores.Relationship<Model.Design.PartVariantRule>(partvariant, "Part Variant Rule");
                    }

                    Boolean selected = true;
                    Double variant_quantity = 0.0;

                    foreach (Model.Design.PartVariantRule partvariantrule in this.PartVariantRuleCache[partvariant])
                    {
                        if (partvariantrule.Related != null)
                        {
                            // Get Order Context
                            Model.Design.OrderContext ordercontext = this.OrderContext(partvariantrule.VariantContext);
                            String order_context_value = null;
                            Double order_context_quantity = 0.0;

                            switch (partvariantrule.VariantContext.ContextType.Value)
                            {
                                case "Boolean":
                                case "List":

                                    order_context_value = ordercontext.Value;
                                    order_context_quantity = ordercontext.Quantity;

                                    break;

                                case "Method":

                                    Model.IO.Item dborder = new Model.IO.Item(this.ModelItem.ItemType.Name, partvariantrule.VariantContext.Method);
                                    dborder.ID = this.ModelItem.ID;

                                    // Add this Order Context
                                    Model.IO.Item dbordercontext = ordercontext.GetIOItem();
                                    dborder.AddRelationship(dbordercontext);

                                    // Add all other order Context
                                    foreach (Model.Design.OrderContext otherordercontext in this.OrderContexts)
                                    {
                                        if (!otherordercontext.Equals(ordercontext))
                                        {
                                            Model.IO.Item dbotherordercontext = otherordercontext.GetIOItem();
                                            dborder.AddRelationship(dbotherordercontext);
                                        }
                                    }

                                    Model.IO.SOAPRequest request = new Model.IO.SOAPRequest(Model.IO.SOAPOperation.ApplyItem, this.ModelSession, dborder);
                                    Model.IO.SOAPResponse response = request.Execute();

                                    if (!response.IsError)
                                    {
                                        if (response.Items.Count() == 1)
                                        {
                                            order_context_value = response.Items.First().GetProperty("value");
                                            order_context_quantity = Double.Parse(response.Items.First().GetProperty("quantity"));
                                        }
                                        else
                                        {
                                            throw new Model.Exceptions.ServerException("Variant Method failed to return a result: " + partvariantrule.VariantContext.Method);
                                        }
                                    }
                                    else
                                    {
                                        throw new Model.Exceptions.ServerException(response);
                                    }

                                    break;

                                default:
                                    throw new Model.Exceptions.ArgumentException("Unsupported Variant Context Type: " + partvariantrule.VariantContext.ContextType.Value);
                            }

                            if (String.Compare(partvariantrule.Value, order_context_value) == 0)
                            {
                                Double calc_quantity = Quantity * partvariant.Quantity * order_context_quantity;

                                if (calc_quantity > variant_quantity)
                                {
                                    variant_quantity = calc_quantity;
                                }
                            }
                            else
                            {
                                selected = false;
                            }
                        }
                        else
                        {
                            selected = false;
                        }

                        if (!selected)
                        {
                            break;
                        }
                    }

                    if (selected)
                    {
                        this.RefeshPart((Model.Design.Part)partvariant.Related, variant_quantity);
                    }
                }
            }
            else
            {
                // Add this Part to Cache
                if (!this.PartCache.ContainsKey(Part))
                {
                    this.PartCache[Part] = Quantity;
                }
                else
                {
                    this.PartCache[Part] += Quantity;
                }
            }

            // Process Part BOM
            if (!this.BOMStoreCache.ContainsKey(Part))
            {
                this.BOMStoreCache[Part] = new Model.Stores.Relationship<Model.Design.PartBOM>(Part, "Part BOM");
            }

            foreach (Model.Design.PartBOM partbom in this.BOMStoreCache[Part])
            {
                if (partbom.Related != null)
                {
                    Double part_bom_quantity = Quantity * partbom.Quantity;
                    this.RefeshPart((Model.Design.Part)partbom.Related, part_bom_quantity);
                }
            }
        }

        private void UpdateItem()
        {
            // Ensure Configured Part exists
            if (this.ConfiguredPart == null)
            {
                Model.Stores.Item<Model.Design.Part> partsstore = new Model.Stores.Item<Model.Design.Part>(this.ModelSession, "Part", Aras.Conditions.Eq("item_number", ((Model.Design.Part)this.ModelItem).ItemNumber));

                if (partsstore.Count() == 0)
                {
                    // Create ConfiguredPart
                    this.ConfiguredPart = (Model.Design.Part)partsstore.Create(this.ModelTransaction);
                    this.ConfiguredPart.ItemNumber = ((Model.Design.Part)this.ModelItem).ItemNumber;
                }
                else
                {
                    // Use Existing Configured Part
                    this.ConfiguredPart = (Model.Design.Part)partsstore.First();
                    this.ConfiguredPart.Update(this.ModelTransaction);
                }
            }

            // Update Configured Part Properties
            this.ConfiguredPart.Class = this.ConfiguredPart.ItemType.GetClassName("TopLevel");
            this.ConfiguredPart.Property("name").Value = this.ModelItem.Property("name").Value;
            this.ConfiguredPart.Property("cmb_name").Value = this.ModelItem.Property("name").Value;

            // Refesh Part Cache
            this.RefeshPartCache();

            // Build List of Parts
            List<Model.Design.Part> requiredparts = new List<Model.Design.Part>();

            foreach (Model.Design.Part part in this.PartCache.Keys)
            {
                if (part.Class.Name == "BOM")
                {
                    requiredparts.Add(part);
                }
            }

            // Build list of current Parts and remove Part not need anymore
            List<Model.Design.Part> currentparts = new List<Model.Design.Part>();

            foreach (Model.Design.PartBOM currentpartbom in this.ConfiguredPartBOMS)
            {
                Model.Design.Part currentpart = (Model.Design.Part)currentpartbom.Related;

                if (requiredparts.Contains(currentpart))
                {
                    currentparts.Add(currentpart);

                    // Update Quanity
                    currentpartbom.Update(this.ModelTransaction);
                    currentpartbom.Quantity = this.PartCache[currentpart];
                }
                else
                {
                    // Remove PartBOM
                    currentpartbom.Delete(this.ModelTransaction, true);
                }
            }

            // Add any new Parts
            foreach (Model.Design.Part requiredpart in requiredparts)
            {
                if (!currentparts.Contains(requiredpart))
                {
                    Model.Design.PartBOM newpartbom = this.ConfiguredPartBOMS.Create(requiredpart, this.ModelTransaction);
                    newpartbom.Quantity = this.PartCache[requiredpart];
                }
            }

            // Set Sequence Number
            int cnt = 10;
            
            foreach(Model.Design.PartBOM partbom in this.ConfiguredPartBOMS.CurrentItems())
            {
                if (partbom.SortOrder != cnt)
                {
                    partbom.Update(this.ModelTransaction);
                    partbom.SortOrder = cnt;
                }

                cnt += 10;
            }

            // Update BOM Grid
            this.UpdateBOMGrid();

            // Update Commands
            this.SetCommandsCanExecute();
        }

        private void SetCommandsCanExecute()
        {
            if (this.ModelItem != null)
            {
                if (this.ModelTransaction == null)
                {
                    this.Update.UpdateCanExecute(false);
                }
                else
                {
                    this.Update.UpdateCanExecute(true);
                }
            }
            else
            {
                this.Update.UpdateCanExecute(false);
            }
        }

        public Order()
            : base()
        {
            // Create Caches
            this.PartCache = new Dictionary<Model.Design.Part, Double>();
            this.BOMStoreCache = new Dictionary<Model.Design.Part, Model.Stores.Relationship<Model.Design.PartBOM>>();
            this.VariantStoreCache = new Dictionary<Model.Design.Part, Model.Stores.Relationship<Model.Design.PartVariant>>();
            this.PartVariantRuleCache = new Dictionary<Model.Design.PartVariant, Model.Stores.Relationship<Model.Design.PartVariantRule>>();
            this.PartBOMNumberCache = new ControlCache<Model.Design.PartBOM, ViewModel.Properties.String>();
            this.PartBOMRevisionCache = new ControlCache<Model.Design.PartBOM, ViewModel.Properties.String>();
            this.PartBOMNameCache = new ControlCache<Model.Design.PartBOM, ViewModel.Properties.String>();
            this.PartBOMQuantityCache = new ControlCache<Model.Design.PartBOM, ViewModel.Properties.Float>();
            this.ConfigQuestionCache = new ControlCache<Model.Design.OrderContext, ViewModel.Properties.String>();
            this.ConfigValueCache = new ControlCache<Model.Design.OrderContext, Properties.OrderContextList>();
            this.ConfigQuantityCache = new ControlCache<Model.Design.OrderContext, ViewModel.Properties.Float>();

            // Create Commands
            this.Update = new UpdateCommand(this);
        }

        public class UpdateCommand : Aras.ViewModel.Command
        {
            public Order Order { get; private set; }

            internal void UpdateCanExecute(Boolean CanExecute)
            {
                this.CanExecute = CanExecute;
            }

            protected override void Run(IEnumerable<Control> Parameters)
            {
                this.Order.UpdateItem();
            }

            internal UpdateCommand(Order Order)
            {
                this.Order = Order;
                this.CanExecute = false;
            }
        }
    }
}
