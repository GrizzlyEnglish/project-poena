using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poena.Core.Common;
using Poena.Core.Extensions;
using Poena.Core.Managers;
using Poena.Core.Utilities;

namespace Poena.Core
{
    public class Poena : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;

        public Poena()
        {
            Logger.GetInstance().LogLevel = LogLevel.Debug;
            graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = false;
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = Variables.VIEWPORT_HEIGHT;
            graphics.PreferredBackBufferWidth = Variables.VIEWPORT_WIDTH;

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.WindowResizeEvent);

            screenManager = new ScreenManager();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //TODO: Init screen manager with splash screen
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.LoadContent(GraphicsDevice, this.Content);
            screenManager.LoadContent(this.Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            screenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            screenManager.Render(spriteBatch);

            base.Draw(gameTime);
        }

        private void WindowResizeEvent(object sender, EventArgs e)
        {
            //Set the new vars
            Variables.VIEWPORT_HEIGHT = this.Window.ClientBounds.Height;
            Variables.VIEWPORT_WIDTH = this.Window.ClientBounds.Width;

            graphics.PreferredBackBufferHeight = Variables.VIEWPORT_HEIGHT;
            graphics.PreferredBackBufferWidth = Variables.VIEWPORT_WIDTH;

            screenManager.WindowResizeEvent();
        }
    }
}
