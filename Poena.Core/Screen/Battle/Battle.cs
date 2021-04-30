using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Screens;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Managers;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Entities;
using Poena.Core.Screen.Battle.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle
{
    public class Battle : GameScreen
    {
        private Poena _game => (Poena)base.Game;

        private World _world;
        private BoardGrid _boardGrid;
        private OrthographicCamera _camera;
        private readonly AssetManager _assetManager;

        public Battle(Poena game) : base(game)
        {
            _assetManager = new AssetManager(game.Content);
        }

        public override void Initialize()
        {
            _camera = new OrthographicCamera(_game.GraphicsDevice);
            _camera.Position = Coordinates.BoardToWorld(new Coordinates(9, 9, 0)).AsVector2();
            _camera.Zoom = .9f;
            _boardGrid = new BoardGrid(_assetManager, _game.SpriteBatch, _camera, BoardSize.Medium);
            _world = new WorldBuilder()
                .AddSystem(new PositionSystem(_boardGrid))
                .AddSystem(new SelectionSystem(_boardGrid, _camera))
                .AddSystem(new TurnSystem())
                .AddSystem(new TurnRenderSystem(_game.SpriteBatch, _assetManager))
                .AddSystem(new TileHighlightSystem(_game.SpriteBatch, _boardGrid))
                .AddSystem(new AttackingSystem())
                .Build();

            EntityFactory.GenerateEntity(_world);
            EntityFactory.GenerateNPC(_world);

            base.Initialize();
        }

        public override void LoadContent()
        {
            _assetManager.LoadTexture(Assets.GetUIElement(UIElements.EmptyActionBar));
            _assetManager.LoadTexture(Assets.GetUIElement(UIElements.BlueActionBar));
            _assetManager.LoadTexture(Assets.GetTile(TileType.Debug));
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.White);

            Matrix transformMatrix = _camera.GetViewMatrix();
            _game.SpriteBatch.Begin(transformMatrix: transformMatrix);

            _boardGrid.Draw(gameTime);
            _world.Draw(gameTime);

            _game.SpriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            _world.Update(gameTime);
        }

        /*public override bool HandleMouseClicked(MouseEvent mouseEvent)
        {
            mouseEvent.SetUnprojectedPosition(this.Camera);
            this.LayerNodeObjects.ForEach(obj => {
                bool handled = obj.HandleMouseClicked(mouseEvent);
                if (handled)
                {
                    return;
                }
            });

            return false;
        }

        public override void HandleMouseMoved(MouseEvent mouseEvent)
        {
            mouseEvent.SetUnprojectedPosition(this.Camera);
            this.LayerNodeObjects.ForEach(obj => obj.HandleMouseMoved(mouseEvent));
        }

        public override void HandleMouseDragged(MouseEvent mouseEvent)
        {
            Vector2 mouseMoved = mouseEvent.MouseEventArgs.DistanceMoved.Invert();
            this.Camera.Translate(mouseMoved);
        }

        public override void HandleMouseWheeled(MouseEvent mouseEvent)
        {
            int zoom = mouseEvent.MouseEventArgs.ScrollWheelDelta * 3;
            this.Camera.Zoom(zoom);
        }*/
    }
}
