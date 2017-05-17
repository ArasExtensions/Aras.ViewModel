using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class String : Cell
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
                if (Value is System.String)
                {
                    this.Value = (System.String)Value;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be System.String");
                }
            }
        }

        protected override void ProcessUpdateValue(System.String Value)
        {
            this.SetValue(Value);
        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);
        
            if (!(Binding is Model.Properties.String))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.String");
            }
        }

        internal String(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
