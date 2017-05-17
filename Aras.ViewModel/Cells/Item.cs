using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class Item : Cell
    {
        public override void SetValue(object Value)
        {
            base.SetValue(Value);

            if (Value == null)
            {
                this.Value = null;
            }
            else
            {
                if (Value is Aras.Model.Item)
                {
                    this.Value = (System.String)((Aras.Model.Item)Value).Property("keyed_name").Value;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be Aras.Model.Item");
                }
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {
            if (Value == null)
            {
                this.SetValue(null);
            }
            else
            {
                Model.Item thisitem = ((Columns.Item)this.Column).Query.Store.Get(Value);
                this.SetValue(thisitem);
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Item))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Item");
            }
        }

        internal Item(Column Column, Row Row)
            :base(Column, Row)
        {
       
        }
    }
}
