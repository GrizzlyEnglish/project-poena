using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Extensions;
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
        private EntityFactory _entityFactory;
        private readonly AssetManager _assetManager;
        private MouseListener _mouseListener;

        public Battle(Poena game) : base(game)
        {
            _assetManager = new AssetManager(game.Content);
            _entityFactory = new EntityFactory(_assetManager);

            _mouseListener = new MouseListener();
            _mouseListener.MouseDrag += HandleMouseDragged;
            _mouseListener.MouseWheelMoved += HandleMouseWheeled;

            Game.Components.Add(new InputListenerComponent(Game, _mouseListener));
        }

        public override void Initialize()
        {
            _camera = new OrthographicCamera(_game.GraphicsDevice);
            _camera.Position = Coordinates.BoardToWorld(new Coordinates(9, 9, 0)).AsVector2();
            _camera.Zoom = .9f;
            _boardGrid = new BoardGrid(_assetManager, _game.SpriteBatch, _camera, BoardSize.Medium);
            _world = new WorldBuilder()
                .AddSystem(new TileHighlightSystem(_game.SpriteBatch, _assetManager))
                .AddSystem(new PositionSystem(_boardGrid))
                .AddSystem(new SelectionSystem(_boardGrid, _camera))
                .AddSystem(new TurnSystem())
                .AddSystem(new TurnRenderSystem(_game.SpriteBatch, _assetManager))
                .AddSystem(new AttackingSystem())
                .AddSystem(new SpriteSystem(_game.SpriteBatch))
                .Build();

            base.Initialize();
        }

        public override void LoadContent()
        {
            _assetManager.LoadTexture(Assets.GetUIElement(UIElements.EmptyActionBar));
            _assetManager.LoadTexture(Assets.GetUIElement(UIElements.BlueActionBar));
            _assetManager.LoadTexture(Assets.GetTile(TileType.Debug));
            _assetManager.LoadTexture(Assets.GetEntity(EntityType.GiantRat));
            _assetManager.LoadTexture(Assets.GetEntity(EntityType.Adventurer));
            _assetManager.LoadTexture(Assets.GetTileHighlight(TileHighlight.Movement));
            _assetManager.LoadTexture(Assets.GetTileHighlight(TileHighlight.Attack));
            // TODO: Figure out how to organize this better
            _entityFactory.GenerateEntity(_world);
            _entityFactory.GenerateNPC(_world);
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

        public void HandleMouseClicked(object sender, MouseEventArgs mouseEvent)
        {
        }

        public void HandleMouseMoved(object sender, MouseEventArgs mouseEvent)
        {
        }

        public void HandleMouseDragged(object sender, MouseEventArgs mouseEvent)
        {
            this._camera.Move(mouseEvent.DistanceMoved.Invert());
        }

        public void HandleMouseWheeled(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.ScrollWheelDelta < 0) this._camera.ZoomOut(.03f);
            else this._camera.ZoomIn(.03f);
        }
    }
}
