using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Scene;

namespace Poena.Core.Screen
{
    /*
     *  Screens are the boxes in which the renderable objects exist
     *  Screens main job is to pause updating and hide and show the scenes
     */

    public class Screen
    {
        private AbstractScene scene;
        
        private string screen_name;

        public bool updating { get; private set; }
        public bool rendering { get; private set; }

        public Screen(string screenName, AbstractScene scene)
        {
            this.screen_name = screenName;
            this.scene = scene;
            this.rendering = true;
            this.updating = true;
        }

        public void Initialize()
        {
            this.scene.Initialize();
        }

        public void LoadContent(ContentManager contentManager)
        {
            this.scene.LoadContent(contentManager);

            //Content is loaded enter scene
            this.scene.EnterScene();
        }

        //Render the screen
        public void Render(SpriteBatch spriteBatch)
        {
            if (this.rendering)
            {
                this.scene.Render(spriteBatch, null);
            }
        }

        //Update the screen
        public StateEnum Update(double delta)
        {
            if (this.updating)
            {
                return this.scene.Update(delta);
            }

            return StateEnum.Paused;
        }
        
        //Pause stops updating
        public void Pause()
        {
            this.updating = false;
        }

        //Resume updating
        public void Resume()
        {
            this.updating = true;
        }

        //Hide screen
        public void Hide()
        {
            this.rendering = false;
        }

        //Resume rendering
        public void Show()
        {
            this.rendering = true;
        }

        // Load up input data and the scene
        public void Load(string path)
        {
            //TODO: Add in logic to load current screens input mappings

            //Load scene
            this.scene.Load(this.screen_name);
        }

        // Save the current scenes data
        public void Save(string path)
        {
            this.scene.Save(this.screen_name);
        }

        //Update the sprites and cameras viewport
        public void WindowResizeEvent()
        {
            this.scene.WindowResizeEvent();
        }
    }

}