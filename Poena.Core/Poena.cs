using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Extensions;
using Poena.Core.Screen.Battle;
using Poena.Core.Utilities;

namespace Poena.Core
{
    public class Poena : Game
    {
        readonly GraphicsDeviceManager _graphics;
        readonly ScreenManager _screenManager;

#if DEBUG
        readonly FrameCounter _frameCounter;
#endif

        public SpriteBatch SpriteBatch { get; private set; }

        public Poena()
        {
            Logger.GetInstance().LogLevel = LogLevel.Debug;
            _graphics = new GraphicsDeviceManager(this);

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = Config.VIEWPORT_HEIGHT;
            _graphics.PreferredBackBufferWidth = Config.VIEWPORT_WIDTH;

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.WindowResizeEvent);

            _screenManager = new ScreenManager();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

#if DEBUG
            _frameCounter = new FrameCounter(this);
#endif
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch.LoadContent(GraphicsDevice, this.Content);

            // Setup debug render dexture
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });

            _screenManager.LoadScreen(new Battle(this));

            Config.DEBUG_TEXTURE = texture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _screenManager.Update(gameTime);

#if DEBUG
            _frameCounter.Update(gameTime);
#endif

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _screenManager.Draw(gameTime);

#if DEBUG
            _frameCounter.Draw(gameTime);
#endif

            base.Draw(gameTime);
        }

        private void WindowResizeEvent(object sender, EventArgs e)
        {
            //Set the new vars
            Config.VIEWPORT_HEIGHT = this.Window.ClientBounds.Height;
            Config.VIEWPORT_WIDTH = this.Window.ClientBounds.Width;

            _graphics.PreferredBackBufferHeight = Config.VIEWPORT_HEIGHT;
            _graphics.PreferredBackBufferWidth = Config.VIEWPORT_WIDTH;
        }
    }
}
