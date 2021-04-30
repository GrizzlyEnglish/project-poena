using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Common.Interfaces;
using Poena.Core.Extensions;

namespace Poena.Core.Screen.Battle.Board
{
    public class BoardTile : IRouteable
    {
        public BoardGridPosition Position { get; set; }
        public BoardGrid BoardGrid { get; private set; }
        public bool IsVisible { get; private set; }
        public TileType TileType { get; private set; }

        public BoardTile(Coordinates boardCoordinates, bool isVisible = true)
        {
            this.Position = new BoardGridPosition(boardCoordinates.x, boardCoordinates.y, boardCoordinates.z);
            this.IsVisible = isVisible;
        }

        public void InjectBoard(BoardGrid bg)
        {
            this.BoardGrid = bg;
        }
        
        public void Initialize()
        {
            TileType = TileType.Debug;
        }

        /// <summary>
        /// Gets thes renderable position of the board tile
        /// For ease of use
        /// </summary>
        /// <returns>
        /// Returns world coordinates of the tile
        /// </returns>
        public Coordinates RenderCoordinates() 
        {
            Point p = Coordinates.WorldToBoard(this.Position.WorldPosition.TopLeft);
            Coordinates coordinates = Coordinates.BoardToWorld(p.X, p.Y);

            return coordinates;
        }

        public bool IsEqual(BoardTile bt)
        {
            Coordinates c1 = this.Position.GridSlot;
            Coordinates c2 = bt.Position.GridSlot;

            return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
        }

        public override string ToString()
        {
            Coordinates b = this.Position.GridSlot;
            RectangleF w = this.Position.WorldPosition;
            return 
                $"Grid Slot [{b.x}, {b.y}, {b.z}]  at ({w.X},{w.Y})";
        }

        public int GetMovementCost(IRouteable mover = null)
        {
            return 1;
        }
    }
}
