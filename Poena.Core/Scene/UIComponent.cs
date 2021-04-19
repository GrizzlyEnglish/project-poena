using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Sprites;

namespace Poena.Core.Scene
{

    /*
     * The abstraction of a user interface component
     * A ui component is made of a backround "panel" and a foreground icon/text
     * 
     * In most cases the background rendering will be handled by the UI layer
     * But in special cases the Foreground can act as the background (ie hotbar)
     */

    public abstract class UIComponent : IUIComponent
    {

        // Background texture handled by layer
        public string background_texture_name { get; protected set; }

        protected Sprite foreground_sprite;
        
        protected bool is_visible { get; set; }

        public int background_width { get; protected set; }
        public int background_height { get; protected set; }

        protected bool is_unique { get; set; }

        public UISceneLayer ui_scene_layer { get; protected set; }

        protected Vector2 ui_position
        {
            get { return this.foreground_sprite.position.position; }
            set { this.foreground_sprite.position.SetPosition(value); }
        }

        public UIComponent(UISceneLayer sceneLayer, Vector2 pos)
        {
            this.foreground_sprite = new Sprite();
            this.foreground_sprite.SetPosition(pos);
            this.ui_scene_layer = sceneLayer;
            this.ui_position = pos;
        }

        public UIComponent(UISceneLayer sceneLayer)
        {
            this.foreground_sprite = new Sprite();
            this.ui_scene_layer = sceneLayer;
        }

        public UIComponent()
        {
            this.foreground_sprite = new Sprite();
        }

        //Most likely not needed, but if so can override
        public StateEnum Update(double delta)
        {
            return StateEnum.InProgress;
        }

        public void SetTextureDimensions(int width, int height)
        {
            this.background_height = height;
            this.background_width = width;
        }

        //Alias for readablity
        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null)
        {
            if (this.is_visible)
            {
                this.RenderForeground(spriteBatch);
            }
        }

        public void Show()
        {
            this.is_visible = true;
        }

        public void Hide()
        {
            this.is_visible = false;
        }

        public Vector2 GetPosition()
        {
            return this.ui_position;
        }

        public virtual void SetPosition(Vector2 position)
        {
            this.ui_position = position;
            if (this.foreground_sprite?.position != null) this.foreground_sprite?.position.SetPosition(position);
        }

        public virtual void RenderForeground(SpriteBatch spriteBatch)
        {
            this.foreground_sprite.Render(spriteBatch);
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            //Default load the foreground texture and set the dimensions
            this.foreground_sprite.LoadContent(contentManager);
            this.SetTextureDimensions(this.foreground_sprite.width, this.foreground_sprite.height);
        }
        
        public bool IsWithinBounds(Point? p)
        {
            if (this.is_visible && p.HasValue)
            {
                Point p2 = p.Value;

                float left = this.ui_position.X - (this.background_width / 2);
                float right = left + this.background_width;
                
                float top = this.ui_position.Y - (this.background_height / 2);
                float bottom = top + this.background_height;
                
                return p2.X >= left && p2.X <= right && p2.Y >= top && p2.Y <= bottom;
            }

            return false;
        }

        //Gets the uis background layer
        public string GetBackgroundTexturePath()
        {
            return Variables.AssetPaths.UI_PATH + "/" + background_texture_name;
        }

        public abstract bool HandleMouseClicked(MouseEvent mouseEvent);
    }
}
