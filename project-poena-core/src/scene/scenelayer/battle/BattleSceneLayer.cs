using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Board;
using Project_Poena.Entity.Factories;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Coordinates;
using Project_Poena.Common.Enums;
using Project_Poena.Events;
using Project_Poena.Scene;
using Project_Poena.Cameras;
using Project_Poena.Input;
using System.Diagnostics;

namespace Project_Poena.Scene.SceneLayer
{
    public class BattleSceneLayer : SceneLayer
    {
        public BattleSceneLayer() : base()
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

            //Build the board
            BattleBoard board = new BattleBoard(BoardSize.Medium);
            board.Initialize();
            this.layer_nodes.Add(board);
    
            //Get the board bounds
            Rectangle bounds = board.GetBounds();
            //Set the camera to the bounds of the game board
            this.camera.ClampCamera(bounds, true);

            //For testing
            this.entity_manager.AddEntity(EntityFactory.GenerateEntity());
            this.entity_manager.AddEntity(EntityFactory.GenerateNPC());
            this.camera.SetPosition(Coordinates.BoardToWorld(new Coordinates(9, 9, 0)).AsVector2());
            this.camera.SetZoom(.9f);

            //Initialize systems last for any properties that need to be set
            this.system_manager.Initialize();
        }

        public override void Exit()
        {
            
        }

        public override void HandleEvents()
        {
            
        }

        public override List<MappedInputAction> HandleLayerInput(List<MappedInputAction> actions)
        {
            this.layer_nodes.ForEach(obj => {
                actions = obj.HandleInput(actions);
            });
            return actions;
        }

        public override void Load(string path)
        {
            
        }

        public override void LoadContent(ContentManager contentManager)
        {
            this.layer_nodes.ForEach(obj => {
                obj.LoadContent(contentManager);
            });
            this.system_manager.LoadContent(contentManager);
        }

        public override void RenderLayer(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            RectangleF bounds = this.camera.GetViewBounds();
            
            //TODO: rce - Consider ordering this for rendering purposes?
            this.layer_nodes.ForEach(obj => {
                obj.Render(spriteBatch, camera_bounds);
            });
            this.system_manager.Render(spriteBatch, camera_bounds);
        }

        public override void Save(string path)
        {
            
        }

        public override StateEnum UpdateLayer(double delta)
        {
            this.layer_nodes.ForEach(obj => {
                obj.Update(delta);
            });

            this.system_manager.Update(delta);


            return StateEnum.InProgress;
        }
        
    }
}
