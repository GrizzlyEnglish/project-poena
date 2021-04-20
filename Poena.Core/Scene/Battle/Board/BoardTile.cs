using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Extensions;

namespace Poena.Core.Scene.Battle.Board
{
    public class BoardTile : INodeObject, IRouteable
    {
        private Texture2D tile_texture;
        private bool isVisible;

        public BoardGridPosition position { get; set; }
        public BoardGrid board_grid { get; private set; }
        
        private string tile_name;
            
        //Debug
        private bool debug_render = false;

        public BoardTile(Coordinates boardCoordinates, bool isVisible = true)
        {
            this.position = new BoardGridPosition(boardCoordinates.x, boardCoordinates.y, boardCoordinates.z);
            this.isVisible = isVisible;
        }

        public BoardTile(int x, int y, int z, bool isVisible = true)
        {
            this.position = new BoardGridPosition(x, y, z);
            this.isVisible = isVisible;
        }

        public void InjectBoard(BoardGrid bg)
        {
            this.board_grid = bg;
        }
        
        public void Initialize()
        {
            this.tile_name = "HEX_Dirt_01";
        }

        public void Entry()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager contentManager)
        {
            //TODO: Create a handler that takes params and generates the actual file name
            this.tile_texture = contentManager.Load<Texture2D>(Variables.AssetPaths.TILE_PATH + this.tile_name);
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            if (this.isVisible && camera_bounds.Overlaps(this.position.world_position))
            {
                spriteBatch.Draw(this.tile_texture, this.position.world_position.AsVector2(), Color.White);
#if DEBUG
                if (this.debug_render)
                {
                    Vector2 pos = this.position.GetWorldAnchorPosition();
                    
                    spriteBatch.DrawDebugString(this.position.grid_slot.ToString(), pos);
                }
#endif
            }
        }
        
        public StateEnum Update(double delta)
        {
            throw new NotImplementedException();
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
            Point p = Coordinates.WorldToBoard(this.position.world_position.AsVector2());
            Coordinates coordinates = Coordinates.BoardToWorld(p.X, p.Y);

            return coordinates;
        }

        public bool IsEqual(BoardTile bt)
        {
            Coordinates c1 = this.position.grid_slot;
            Coordinates c2 = bt.position.grid_slot;

            return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
        }

        public override string ToString()
        {
            Coordinates b = this.position.grid_slot;
            RectangleF w = this.position.world_position;
            return 
                $"Grid Slot [{b.x}, {b.y}, {b.z}]  at ({w.x},{w.y})";
        }

        public int GetMovementCost(IRouteable mover = null)
        {
            return 1;
        }

        public bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            throw new NotImplementedException();
        }

        public void HandleMouseMoved(MouseEvent mouseEvent)
        {
            throw new NotImplementedException();
        }
    }
}
