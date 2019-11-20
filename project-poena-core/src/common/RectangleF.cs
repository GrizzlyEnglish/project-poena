using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Common.Rectangle
{
    public class RectangleF
    {

        public float x { get; private set; }
        public float y { get; private set; }

        public float width { get; private set; }
        public float height { get; private set; }

        public float top { get { return y; } }
        public float left { get { return x; } }
        public float right { get; private set; }
        public float bottom { get; private set; }
        
        public Vector2 center { get; set; }

        public RectangleF(float x, float y, float w, float h)
        {
            this.width = w;
            this.height = h;
            this.x = x;
            this.y = y;
            this.right = x + w;
            this.bottom = y + h;
            this.center = new Vector2(x + (w / 2), y + (h / 2));
        }

        public bool Overlaps(RectangleF rectangle)
        {
            return rectangle.left < this.right && rectangle.right > this.left &&
                rectangle.bottom > this.top && rectangle.top < this.bottom;
        }

        public Vector2 AsVector2()
        {
            return new Vector2(this.x, this.y);
        }

        public Microsoft.Xna.Framework.Rectangle AsRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle((int)x, (int)y, (int)width, (int)height);
        }

    }
}
