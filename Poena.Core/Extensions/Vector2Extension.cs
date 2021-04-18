using System;
using Microsoft.Xna.Framework;

namespace Poena.Core.Extensions
{
    public static class Vector2Extension
    {
        public static double Distance(this Vector2 a, Vector2 b)
        {
            return Math.Sqrt(
                Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2)
                );
        }

        public static Vector2 Lerp(this Vector2 startPosition, Vector2 endPosition, float time)
        {
            return new Vector2(
                LerpF(startPosition.X, endPosition.X, time),
                LerpF(startPosition.Y, endPosition.Y, time)
                );
        }

        private static float LerpF(float position, float destination, float by)
        {
            return position * (1 - by) + destination * by;
        }

    }
}
