using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Cells
{
    public class List : Cell
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
                if (Value is Aras.Model.Relationships.Value)
                {
                    this.Value = (System.String)((Aras.Model.Relationships.Value)Value).Property("label").Value;
                }
                else
                {
                    throw new Model.Exceptions.ArgumentException("Value must be Aras.Model.Relationships.Value");
                }
            }
        }

        protected override void ProcessUpdateValue(string Value)
        {

        }

        protected override void CheckBinding(object Binding)
        {
            base.CheckBinding(Binding);

            if (!(Binding is Model.Properties.List))
            {
                throw new Model.Exceptions.ArgumentException("Binding must be Aras.Model.Properties.List");
            }
        }

        internal List(Column Column, Row Row)
            :base(Column, Row)
        {

        }
    }
}
