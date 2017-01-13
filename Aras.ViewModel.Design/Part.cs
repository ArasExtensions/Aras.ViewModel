using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.ViewModel.Design
{
    [ViewModel.Attributes.Application("Parts", "", "Design")]
    public class Part : ViewModel.Containers.Application
    {
        public Part()
            : base()
        {

        }
    }
}
