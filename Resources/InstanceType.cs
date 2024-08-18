using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfflictionComponent.Resources
{
    public enum InstanceType
    {
        Open, //basically the default/not having the interface at all
        Single, //affliction can only exist once
        SingleLocation //affliction can only exist once per body area
    }
}
