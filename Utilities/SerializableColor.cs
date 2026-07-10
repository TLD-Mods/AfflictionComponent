using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfflictionComponent.Utilities
{
    public class SerializableColor
    {

        public float r, g, b, a;

        public SerializableColor(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }
        public Color ToColor() => new Color(r, g, b, a);

    }
}
