using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class Integer : Cell
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
                if (Value is System.Int32)
                {
                    this.Value = ((System.Int32)Value).ToString();
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Int32");
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
                System.Int32 thisvalue = 0;

                if (System.Int32.TryParse(Value, out thisvalue))
                {
                    this.SetValue(thisvalue);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Integer))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Integer");
            }
        }

        internal Integer(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
