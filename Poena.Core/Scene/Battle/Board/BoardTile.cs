using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;
using Poena.Core.Extensions;

namespace Poena.Core.Scene.Battle.Board
{
    public class BoardTile : IRouteable
    {

        public BoardGridPosition Position { get; set; }
        public BoardGrid BoardGrid { get; private set; }
        
        private string TileName;
        private Texture2D TileTexture;
        private bool isVisible;

        public BoardTile(Coordinates boardCoordinates, bool isVisible = true)
        {
            this.Position = new BoardGridPosition(boardCoordinates.x, boardCoordinates.y, boardCoordinates.z);
            this.isVisible = isVisible;
        }

        public BoardTile(int x, int y, int z, bool isVisible = true)
        {
            this.Position = new BoardGridPosition(x, y, z);
            this.isVisible = isVisible;
        }

        public void InjectBoard(BoardGrid bg)
        {
            this.BoardGrid = bg;
        }
        
        public void Initialize()
        {
            this.TileName = "HEX_Dirt_01";
        }

        public void LoadContent(ContentManager contentManager)
        {
            //TODO: Create a handler that takes params and generates the actual file name
            this.TileTexture = contentManager.Load<Texture2D>(Variables.AssetPaths.TILE_PATH + this.TileName);
        }

        public void Render(SpriteBatch spriteBatch, RectangleF cameraBounds)
        {
            if (this.isVisible && cameraBounds.Intersects(this.Position.WorldPosition))
            {
                spriteBatch.Draw(this.TileTexture, this.Position.WorldPosition.TopLeft, Color.White);
#if DEBUG
                if (Config.DEBUG_RENDER)
                {
                    Vector2 pos = this.Position.GetWorldAnchorPosition();
                    
                    spriteBatch.DrawDebugString(this.Position.GridSlot.ToString(), pos);
                }
#endif
            }
        }
        
        public void Show()
        {
            this.isVisible = true;
        }

        public void Hide()
        {
            this.isVisible = false;
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
