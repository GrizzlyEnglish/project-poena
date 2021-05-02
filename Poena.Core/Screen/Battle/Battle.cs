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
using Poena.Core.Screen.Battle.Components;
using Poena.Core.Screen.Battle.Entities;
using Poena.Core.Screen.Battle.Systems;
using Poena.Core.Screen.Battle.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle
{
    public class Battle : GameScreen
    {
        private World _world;
        private BoardGrid _boardGrid;
        private EntityFactory _entityFactory;
        private MouseListener _mouseListener;

        private HotBar _hotBar;

        private Vector2? _cameraPanToPosition;
        private Vector2? _cameraPanFromPosition;
        private float _timePanningCamera;

        public OrthographicCamera Camera { get; private set; }
        public new Poena Game { get { return (Poena)base.Game; } }
        public SelectionSystem SelectionSystem { get; private set; }
        public AssetManager AssetManager { get; private set; }

        public Battle(Poena game) : base(game)
        {
            AssetManager = new AssetManager(game.Content);
            _entityFactory = new EntityFactory(AssetManager);

            _mouseListener = new MouseListener();
            _mouseListener.MouseDrag += HandleMouseDragged;
            _mouseListener.MouseWheelMoved += HandleMouseWheeled;
            _mouseListener.MouseClicked += HandleMouseClicked;

            base.Game.Components.Add(new InputListenerComponent(base.Game, _mouseListener));
        }

        public override void Initialize()
        {
            Camera = new OrthographicCamera(Game.GraphicsDevice);
            Camera.LookAt(Coordinates.BoardToWorld(new Coordinates(9, 9, 0)).AsVector2());
            Camera.Zoom = .9f;
            _boardGrid = new BoardGrid(AssetManager, Game.SpriteBatch, Camera, BoardSize.Medium);
            SelectionSystem = new SelectionSystem(_boardGrid, this);
            _world = new WorldBuilder()
                .AddSystem(new TileHighlightSystem(Game.SpriteBatch, AssetManager))
                .AddSystem(new PositionSystem(_boardGrid))
                .AddSystem(SelectionSystem)
                .AddSystem(new TurnSystem())
                .AddSystem(new TurnRenderSystem(Game.SpriteBatch, AssetManager))
                .AddSystem(new AttackingSystem())
                .AddSystem(new SpriteSystem(Game.SpriteBatch))
                .Build();

            _hotBar = new HotBar(this);

            base.Initialize();
        }

        public override void LoadContent()
        {
            AssetManager.LoadTexture(Assets.GetUIElement(UIElements.EmptyActionBar));
            AssetManager.LoadTexture(Assets.GetUIElement(UIElements.BlueActionBar));
            AssetManager.LoadTexture(Assets.GetTile(TileType.Debug));
            AssetManager.LoadTexture(Assets.GetEntity(EntityType.GiantRat));
            AssetManager.LoadTexture(Assets.GetEntity(EntityType.Adventurer));
            AssetManager.LoadTexture(Assets.GetTileHighlight(TileHighlight.Movement));
            AssetManager.LoadTexture(Assets.GetTileHighlight(TileHighlight.Attack));
            AssetManager.LoadTexture(Assets.GetUIElement(UIElements.HotBar));
            AssetManager.LoadTexture(Assets.UI_PATH + "active1");
            _hotBar.LoadContent();
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
            UpdateCamera(gameTime);

            Game.GraphicsDevice.Clear(Color.White);

            Matrix transformMatrix = Camera.GetViewMatrix();
            Game.SpriteBatch.Begin(transformMatrix: transformMatrix);

            _boardGrid.Draw(gameTime);
            _world.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin();
            _hotBar.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            _world.Update(gameTime);

            _hotBar.Update(gameTime);
        }

        public void PanCamera(Vector2 panTo)
        {
            this._cameraPanToPosition = panTo;
            this._cameraPanFromPosition = this.Camera.Center;
            this._timePanningCamera = 0;
        }

        private void UpdateCamera(GameTime gameTime)
        {
            if (this._cameraPanToPosition != null)
            {
                this._timePanningCamera += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
                this.Camera.LookAt(this._cameraPanFromPosition.Value.Lerp(this._cameraPanToPosition.Value, this._timePanningCamera));
                if (this._timePanningCamera >= 1)
                {
                    this.Camera.LookAt(this._cameraPanToPosition.Value);
                    this._cameraPanToPosition = null;
                    this._cameraPanFromPosition = null;
                }
            }
        }

        public void HandleMouseClicked(object sender, MouseEventArgs mouseEvent)
        {
            // TODO: Handle UI clicks before checking the board

            Vector2 worldPoint = Camera.ScreenToWorld(mouseEvent.Position.X, mouseEvent.Position.Y);
            Point p = Coordinates.WorldToBoard(worldPoint);
            this._boardGrid.SelectTile(p);
        }

        public void HandleMouseMoved(object sender, MouseEventArgs mouseEvent)
        {
        }

        public void HandleMouseDragged(object sender, MouseEventArgs mouseEvent)
        {
            this.Camera.Move(mouseEvent.DistanceMoved.Invert());
        }

        public void HandleMouseWheeled(object sender, MouseEventArgs mouseEvent)
        {
            if (mouseEvent.ScrollWheelDelta < 0) this.Camera.ZoomOut(.03f);
            else this.Camera.ZoomIn(.03f);
        }
    }
}
