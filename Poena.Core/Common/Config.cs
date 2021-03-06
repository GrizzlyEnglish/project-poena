﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common
{
    public static class Config
    {
        public static readonly bool DEBUG_RENDER = true;
        public static Texture2D DEBUG_TEXTURE { get; set; }

        public static int VIEWPORT_WIDTH { get; set; } = 960;
        public static int VIEWPORT_HEIGHT { get; set; } = 640;
        
        public static readonly int TILE_WIDTH = 216;
        public static readonly int TILE_HEIGHT = 216;
    }
}
