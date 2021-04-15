using System;
using Microsoft.Xna.Framework;

namespace Project_Poena.Common.Coordinates
{
    public class Coordinates
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }

        public Coordinates(int x, int y, int z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public override bool Equals(object? obj) {
            if (obj == null) {
                return false;
            }
            Coordinates coordinates = (Coordinates)obj;
            return coordinates.x == this.x && coordinates.y == y && coordinates.z == this.z;
        }

        public Vector2 AsVector2()
        {
            return new Vector2(x, y);
        }

        public static Coordinates operator +(Coordinates coordinates, Point pos)
        {
            return new Coordinates(coordinates.x + pos.X, coordinates.y + pos.Y, coordinates.z);
        }
        
        public static Coordinates BoardToWorld(Coordinates coordinates)
        {
            return BoardToWorld(coordinates.x, coordinates.y, coordinates.z);
        }

        /// <summary>
        /// Returns the point in a Coordinates wrapper
        /// </summary>
        /// <param name="p">The point that is being converted</param>
        /// <returns>
        /// A world point coordinate
        /// </returns>
        public static Coordinates BoardToWorld(Point p) 
        {
            return BoardToWorld(p.X, p.Y);
        }

        public static Coordinates BoardToWorld(int x, int y, int z = 0)
        {
            float half_width = Variables.Variables.TILE_WIDTH / 2;
            float half_height = Variables.Variables.TILE_HEIGHT / 2;

            x -= z;
            y -= z;

            int wx = (int)(half_width + 25) * (x - y);
            int wy = (int)((half_height - 75) * (x + y) - (z * 25));

            return new Coordinates(wx, wy);
        }

        public static Point WorldToBoard(Vector2 pos)
        {
            float half_width = Variables.Variables.TILE_WIDTH / 2;
            float half_height = Variables.Variables.TILE_HEIGHT / 2;
            
            int wy = (int)Math.Floor(((pos.Y / (half_height - 75)) - (pos.X / (half_width + 25))) / 2);
            int wx = (int)Math.Floor((pos.X / (half_width + 25)) + wy);

            return new Point(wx, wy);
        }

        public static Enums.TileDirections PositionDirectional(Vector3 forPos, Vector3 toPos)
        {
            if (forPos.X < toPos.X)
            {
                if (forPos.Y < toPos.Y) return Enums.TileDirections.Top;
                else return Enums.TileDirections.TopLeft;
            }
            else if (forPos.X > toPos.X)
            {
                if (forPos.Y > toPos.Y) return Enums.TileDirections.Bottom;
                else return Enums.TileDirections.BottomRight;
            }
            else
            {
                if (forPos.Y < toPos.Y) return Enums.TileDirections.TopRight;
                else return Enums.TileDirections.BottomLeft;
            }
        }

        public static bool IsTraversable(Vector3 pos1, Vector3 pos2)
        {
            float x = pos1.X - pos2.X;
            float y = pos1.Y - pos2.Y;
            return x == 0 || y == 0 || SameSign((int)x, (int)y);
        }

        public static bool SameSign(int num1, int num2)
        {
            return num1 == 0 || num2 == 0 || (num1 ^ num2) >= 0;
        }

        public static double Distance(Vector2 p1, Vector2 p2)
        {
            return Math.Sqrt(
                Math.Pow(p1.X - p2.X, 2) +
                Math.Pow(p1.Y - p2.Y, 2)
                );
        }

    }
}
