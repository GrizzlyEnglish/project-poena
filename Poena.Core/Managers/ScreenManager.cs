using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common;
using Poena.Core.Scene.Battle;
using Poena.Core.Utilities;

namespace Poena.Core.Managers {

    /*
        Screen Managers manages the change between different screens

        For Example:
            Moving from Main Menu -> Loading -> Play
     */

    public class ScreenManager
    {
        private Stack<Screen.Screen> screens;
        private FrameCounter frameCounter;

        private readonly TouchListener TouchListener;
        private readonly MouseListener MouseListener;

        public ScreenManager()
        {
            this.screens = new Stack<Screen.Screen>();
            this.TouchListener = new TouchListener();
            this.MouseListener = new MouseListener();
            
            //Default EntryPoint of Splash
            this.AddScreen(new Screen.Screen("debug", new BattleScene(MouseListener, TouchListener)));

#if DEBUG
            this.frameCounter = new FrameCounter();
#endif
        }

        public void AddScreen(Screen.Screen screen)
        {
            //Look below if we have something pause it
            if (this.screens.Count > 0) {
                this.screens.Peek().Pause();
            }

            //Do any entry logic
            screen.Initialize();

            //Add it
            this.screens.Push(screen);
        }

        public void RemoveScreen(Screen.Screen screen)
        {
                
        }

        public void GetScreen(string screen_name)
        {
            
        }

        public void LoadContent(ContentManager contentManager)
        {
            foreach (Screen.Screen screen in screens)
            {
                screen.LoadContent(contentManager);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (Screen.Screen screen in screens) 
            {
                screen.Render(spriteBatch);
            }

#if DEBUG
            //Render on the very time
            frameCounter.Render(spriteBatch, null);
#endif
        }

        public void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
#if DEBUG
            //Update fps counter
            frameCounter.Update((float)deltaTime);
#endif

            //Gather the input mappings
            this.TouchListener.Update(gameTime);
            this.MouseListener.Update(gameTime);

            foreach (Screen.Screen screen in screens) 
            {
                StateEnum screenState = screen.Update(deltaTime);

                if (screenState == StateEnum.Completed) {
                    //Flag the screen for removal
                }
            }
        }

        public void WindowResizeEvent()
        {
            foreach (Screen.Screen screen in screens)
            {
                screen.WindowResizeEvent();
            }
        }

    }
}