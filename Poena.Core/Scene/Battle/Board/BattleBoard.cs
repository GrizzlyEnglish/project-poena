using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;

namespace Poena.Core.Scene.Battle.Board
{
    public class BattleBoard : INodeObject
    {
        public BoardGrid grid { get; private set; }

        public BattleBoard(BoardSize board_size)
        {
            grid = new BoardGridBuilder().SetSize(board_size).Build();
        }

        public void Initialize()
        {
            grid.Initialize();
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

        public bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            return this.grid.HandleMouseClicked(mouseEvent);
        }

        public void HandleMouseMoved(MouseEvent mouseEvent)
        {
            this.grid.HandleMouseMoved(mouseEvent);
        }

        public BoardTile GetHoveringTile()
        {
            return this.grid.HoveringTile;
        }

        public BoardTile GetClickedTile()
        {
            return this.grid.GetClickedTile();
        }
        
        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager contentManager)
        {
            grid.LoadContent(contentManager);
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            grid.Render(spriteBatch, camera_bounds);
        }
        
        public StateEnum Update(double delta)
        {
            return StateEnum.InProgress;
        }

        public Rectangle GetBounds()
        {
            return grid.GetBounds();
        }

        public void HandleMouseDragged(MouseEvent mouseEvent)
        {
            throw new NotImplementedException();
        }

        public void HandleMouseWheeled(MouseEvent mouseEvent)
        {
            throw new NotImplementedException();
        }
    }
}
