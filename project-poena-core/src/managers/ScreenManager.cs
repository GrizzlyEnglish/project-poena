using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Scene;
using Project_Poena.Common.Enums;
using Project_Poena.Screens;
using Project_Poena.Utilities;

namespace Project_Poena.Managers {

    /*
        Screen Managers manages the change between different screens

        For Example:
            Moving from Main Menu -> Loading -> Play
     */

    public class ScreenManager
    {
        private Stack<Screen> screens;
        private InputHandler inputHandler;
        private FrameCounter frameCounter;

        public ScreenManager(InputHandler inputHandler)
        {
            this.screens = new Stack<Screen>();
            this.inputHandler = inputHandler;
            
            //Default EntryPoint of Splash
            this.AddScreen(new Screen("debug", new BattleScene()));

#if DEBUG
            this.frameCounter = new FrameCounter();
#endif
        }

        public void AddScreen(Screen screen)
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

        public void RemoveScreen(Screen screen)
        {
                
        }

        public void GetScreen(string screen_name)
        {
            
        }

        public void LoadContent(ContentManager contentManager)
        {
            foreach (Screen screen in screens)
            {
                screen.LoadContent(contentManager);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (Screen screen in screens) 
            {
                screen.Render(spriteBatch);
            }

#if DEBUG
            //Render on the very time
            frameCounter.Render(spriteBatch, null);
#endif
        }

        public void Update(double delta)
        {
#if DEBUG
            //Update fps counter
            frameCounter.Update((float)delta);
#endif

            //Gather the input mappings
            this.inputHandler.Update();

            //Handle the inputs
            foreach (Screen screen in screens)
            {
                screen.HandleInput(this.inputHandler);
            }

            foreach (Screen screen in screens) 
            {
                StateEnum screenState = screen.Update(delta);

                if (screenState == StateEnum.Completed) {
                    //Flag the screen for removal
                }
            }

            //Remove any necessary screen
        }

        public void WindowResizeEvent()
        {
            foreach (Screen screen in screens)
            {
                screen.WindowResizeEvent();
            }
        }

    }
}