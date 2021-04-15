using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Builders;
using Project_Poena.Events;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Enums;
using Project_Poena.Common.Rectangle;
using Project_Poena.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Board
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

        public List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
        {
            actions = this.grid.HandleInput(actions);

            return actions;
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

    }
}
