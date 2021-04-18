using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Poena.Core.Sprites
{
    public class Animation
    {
        /*
         * Current frame textures of the animation
         */
        private Texture2D[] textures;

        /*
         * Holder for the sprite names for loading
         */
        private string[] textures_paths;

        /*
         * Position of the current frame
         */
        private float animation_time;

        /*
         * Position of the current frame
         */
        private float current_time;

        /*
         * Gets the current frame of the animation
         */
        public Texture2D current_frame
        {
            get
            {
                int frame_slot = (int)(current_time / (textures.Length / animation_time));
                return textures[frame_slot];
            }
        }

        public Animation(params string[] textures)
        {
            
        }
        
        public void LoadContent(ContentManager contentManager)
        {
            this.textures = new Texture2D[this.textures_paths.Length];

            for (int i = 0; i < this.textures_paths.Length - 1; i++)
            {
                this.textures[i] = contentManager.Load<Texture2D>(this.textures_paths[i]);
            }
        }
        
        public void Update(double delta)
        {
            throw new NotImplementedException();
        }
    }
}
