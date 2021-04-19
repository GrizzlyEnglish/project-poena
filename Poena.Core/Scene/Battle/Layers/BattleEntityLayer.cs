using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Scene.Battle.Board;
using Poena.Core.Common;
using Poena.Core.Scene.Battle.Entities;

namespace Poena.Core.Scene.Battle.Layers
{
    public class BattleEntityLayer : SceneLayer
    {
        public BattleEntityLayer()
        {
            
        }

        public override void Entry()
        {

        }

        public override void Destroy()
        {
            
        }

        /*
         *  The initiaalization of the Battle Scene Layer
         *  Determines how to craft the board and what entities
         *  need to be loaded
         */
        public override void Initialize()
        {
            //TODO: rce - This needs to pass to a generator that will create the board and the entities
            this.EntityManager = new Entity.Managers.EntityManager();
            this.SystemManager = new Entity.Managers.SystemManager(this);

            //Build the board
            BattleBoard board = new BattleBoard(BoardSize.Medium);
            board.Initialize();
            this.LayerNodes.Add(board);
    
            //Get the board bounds
            Rectangle bounds = board.GetBounds();
            //Set the camera to the bounds of the game board
            this.Camera.ClampCamera(bounds, true);

            //For testing
            this.EntityManager.AddEntity(EntityFactory.GenerateEntity());
            this.EntityManager.AddEntity(EntityFactory.GenerateNPC());
            this.Camera.SetPosition(Coordinates.BoardToWorld(new Coordinates(9, 9, 0)).AsVector2());
            this.Camera.SetZoom(.9f);

            //Initialize systems last for any properties that need to be set
            this.SystemManager.Initialize();
        }

        public override void Exit()
        {
            
        }

        public override void HandleEvents()
        {
            
        }

        public override void Load(string path)
        {
            
        }

        public override void LoadContent(ContentManager contentManager)
        {
            this.LayerNodes.ForEach(obj => {
                obj.LoadContent(contentManager);
            });
            this.SystemManager.LoadContent(contentManager);
        }

        public override void RenderLayer(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            RectangleF bounds = this.Camera.GetViewBounds();
            
            //TODO: rce - Consider ordering this for rendering purposes?
            this.LayerNodes.ForEach(obj => {
                obj.Render(spriteBatch, camera_bounds);
            });
            this.SystemManager.Render(spriteBatch, camera_bounds);
        }

        public override void Save(string path)
        {
            
        }

        public override StateEnum UpdateLayer(double delta)
        {
            this.LayerNodes.ForEach(obj => {
                obj.Update(delta);
            });

            this.SystemManager.Update(delta);

            return StateEnum.InProgress;
        }

        public override bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            mouseEvent.SetUnprojectedPosition(this.Camera);
            this.LayerNodes.ForEach(obj => {
                bool handled = obj.HandleMouseClicked(mouseEvent);
                if (handled)
                {
                    return;
                }
            });

            return false;
        }
    }
}
