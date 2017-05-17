using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class Decimal : Cell
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
                if (Value is System.Decimal)
                {
                    this.Value = ((System.Decimal)Value).ToString();
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Decimal");
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
                System.Decimal thisval = 0;

                if (System.Decimal.TryParse(Value, out thisval))
                {
                    this.SetValue(thisval);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Decimal))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Decimal");
            }
        }

        internal Decimal(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
