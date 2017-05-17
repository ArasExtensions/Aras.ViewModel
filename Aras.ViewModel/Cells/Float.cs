using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class Float : Cell
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
                if (Value is System.Double)
                {
                    this.Value = ((System.Double)Value).ToString();
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Double");
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
                System.Double thisval = 0;

                if (System.Double.TryParse(Value, out thisval))
                {
                    this.SetValue(thisval);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Float))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Float");
            }
        }

        internal Float(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
