using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfflictionComponent.Components
{
    public class AfflictionManagerSaveDataProxy
    {
        public List<CustomAffliction> AfflictionList { get; set; }

        public AfflictionManagerSaveDataProxy(List<CustomAffliction> list)
        {
            AfflictionList = list;
        }
        public AfflictionManagerSaveDataProxy()
        {
        }
    }
}

