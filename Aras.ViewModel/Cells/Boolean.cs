using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class Boolean : Cell
    {
        public override void SetValue(object Value)
        {
            base.SetValue(Value);

            if (Value == null)
            {
                this.Value = "0";
            }
            else
            {
                if (Value is System.Boolean)
                {
                    if ((System.Boolean)Value)
                    {
                        this.Value = "1";
                    }
                    else
                    {
                        this.Value = "0";
                    }
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.Boolean");
                }
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {
            if (Value == null)
            {
                this.SetValue(false);
            }
            else
            {
                if (Value.Equals("1"))
                {
                    this.SetValue(true);
                }
                else
                {
                    this.SetValue(false);
                }
            }
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.Boolean))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.Boolean");
            }
        }

        internal Boolean(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
