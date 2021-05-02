using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Enums
{
    public enum TileDirections
    {
        Top, //x--, y--
        TopRight, //y--
        BottomRight, //x++
        Bottom, //x++, y++
        BottomLeft, //y++
        TopLeft //x--
    }
}
