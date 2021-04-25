using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Poena.Core.Common;

namespace Poena.Core.Scene.Battle.Board
{

    /*
     *  Position of a board grid slot
     *      Capable of storing:
     *          Tile
     *          Character
     *          Scenery
     * 
     */

    public class BoardGridPosition
    {
        //Positin on the grid
        public Coordinates GridSlot { get; private set; }

        //Position in the world
        public RectangleF WorldPosition { get; private set; }
        
        public BoardGridPosition(int x, int y, int z)
        {
            this.GridSlot = new Coordinates(x, y, z);
            SetWorldRectangle();
        }
        
        public BoardGridPosition(Coordinates grid_slot)
        {
            this.GridSlot = grid_slot;
            SetWorldRectangle();
        }

        private void SetWorldRectangle()
        {
            Coordinates world_coords = Coordinates.BoardToWorld(this.GridSlot);
            this.WorldPosition = new RectangleF(world_coords.x, world_coords.y, Variables.TILE_WIDTH, Variables.TILE_HEIGHT);
        }
        
        private Point DirectionScale(TileDirections direction, int scale)
        {
            Point p = new Point(0, 0);

            switch (direction)
            {
                case TileDirections.Top:
                case TileDirections.TopRight:
                    p.Y -= scale;
                    break;
                case TileDirections.Bottom:
                case TileDirections.BottomLeft:
                    p.Y += scale;
                    break;
            }

            switch (direction)
            {
                case TileDirections.Top:
                case TileDirections.TopLeft:
                    p.X -= scale;
                    break;
                case TileDirections.Bottom:
                case TileDirections.BottomRight:
                    p.X += scale;
                    break;
            }

            return p;
        }


        //TODO: rce - Is this correct? Should this be elsewhere?
        public BoardGridPosition NextPosition(TileDirections direction, int scale = 1)
        {
            Point vt = DirectionScale(direction, scale);
            
            return new BoardGridPosition(this.GridSlot.x + vt.X, this.GridSlot.y + vt.Y, this.GridSlot.z);
        }


        public List<BoardGridPosition> CircleAroundTile(int radius,
            bool includeCenter = false, bool includePreviousCircles = false,
            bool addVariation = false, bool includeEdges = true)
        {
            TileDirections[] directions = includeEdges ?
                new TileDirections[] {
                    TileDirections.Top,
                    TileDirections.BottomRight,
                    TileDirections.BottomLeft,
                    TileDirections.TopLeft,
                    TileDirections.TopRight
                }
                : new TileDirections[] {
                    TileDirections.Top,
                    TileDirections.BottomRight,
                    TileDirections.Bottom,
                    TileDirections.BottomLeft,
                    TileDirections.TopLeft,
                    TileDirections.Top,
                    TileDirections.TopRight
            };

            List<BoardGridPosition> tiles = new List<BoardGridPosition>();
            BoardGridPosition tile = this;

            if (includeCenter) tiles.Add(this);

            BoardGridPosition next_tile = this;

            for (int i = 0; i < directions.Length; i++)
            {
                int c_r;
                if (includeEdges) c_r = (i == 0 ? (radius / 2) : radius) + 1;
                else c_r = radius;
                for (int r = 0; r < c_r; r++)
                {
                    TileDirections d = directions[i];
                    next_tile = next_tile.NextPosition(d);
                    if (i > 0)
                    {
                        tiles.Add(next_tile);
                    }
                    tile = next_tile;
                }
            }

            List<BoardGridPosition> subLayers = null;
            if (includePreviousCircles)
            {
                //TODO: Determine if this is too slow, if it is find a better way
                subLayers = new List<BoardGridPosition>();
                int subLayerCount = radius;
                while (--subLayerCount > 0)
                {
                    subLayers.AddRange(CircleAroundTile(subLayerCount, false, false, false, false));
                }
            }

            if (addVariation)
            {
                List<BoardGridPosition> new_range = new List<BoardGridPosition>();
                List<BoardGridPosition> next_ring = CircleAroundTile(radius + 1, false, false, false, includeEdges);
                next_ring.ForEach(bgp =>
                {
                    if (new Random(Guid.NewGuid().GetHashCode()).Next(0, 2) == 1)
                    {
                        new_range.Add(bgp);
                    }
                });
                new_range.AddRange(tiles);
                tiles = new_range;
            }

            if (subLayers != null) tiles.AddRange(subLayers);

            return tiles.Distinct().ToList();
        }

        public Vector2 GetWorldAnchorPosition()
        {
            Vector2 pos = this.WorldPosition.TopLeft;

            //Set the anchor point to center of tile face
            pos.X += this.WorldPosition.Width / 2;
            pos.Y += 50;

            return pos;
        }

    }
}
