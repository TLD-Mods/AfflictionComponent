using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfflictionComponent.Utilities;

namespace AfflictionComponent.Interfaces
{
    public interface ICustomRightSidePanelObject
    {

        public string PanelLabel { get; set; }
        public bool PanelBackground { get; set; }
        public string PanelIcon { get; set; }
        public string PanelText { get; set; }
        public SerializableColor PanelTextColour { get; set; }

    }
}
