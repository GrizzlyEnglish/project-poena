using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private bool DebugRender = false;

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

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            if (this.isVisible && camera_bounds.Overlaps(this.Position.world_position))
            {
                spriteBatch.Draw(this.TileTexture, this.Position.world_position.AsVector2(), Color.White);
#if DEBUG
                if (this.DebugRender)
                {
                    Vector2 pos = this.Position.GetWorldAnchorPosition();
                    
                    spriteBatch.DrawDebugString(this.Position.grid_slot.ToString(), pos);
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
            Point p = Coordinates.WorldToBoard(this.Position.world_position.AsVector2());
            Coordinates coordinates = Coordinates.BoardToWorld(p.X, p.Y);

            return coordinates;
        }

        public bool IsEqual(BoardTile bt)
        {
            Coordinates c1 = this.Position.grid_slot;
            Coordinates c2 = bt.Position.grid_slot;

            return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
        }

        public override string ToString()
        {
            Coordinates b = this.Position.grid_slot;
            RectangleF w = this.Position.world_position;
            return 
                $"Grid Slot [{b.x}, {b.y}, {b.z}]  at ({w.x},{w.y})";
        }

        public int GetMovementCost(IRouteable mover = null)
        {
            return 1;
        }
    }
}
