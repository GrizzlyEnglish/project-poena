using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;
using Poena.Core.Sprites;

namespace Poena.Core.Scene.UI
{

    /*
     * The abstraction of a user interface component
     * A ui component is made of a backround "panel" and a foreground icon/text
     * 
     * In most cases the background rendering will be handled by the UI layer
     * But in special cases the Foreground can act as the background (ie hotbar)
     */

    public abstract class UIComponent : IInputUIComponent
    {

        // Background texture handled by layer
        public string BackgroundTextureName { get; protected set; }
        public int BackgroundWidth { get; protected set; }
        public int BackgroundHeight { get; protected set; }
        public UISceneLayer UISceneLayer { get; protected set; }
        protected Sprite ForegroundTextureName { get; set; }
        protected bool IsVisible { get; set; }

        protected Vector2 ui_position
        {
            get { return this.ForegroundTextureName.position.position; }
            set { this.ForegroundTextureName.position.SetPosition(value); }
        }

        public UIComponent(UISceneLayer sceneLayer, Vector2 pos)
        {
            this.ForegroundTextureName = new Sprite();
            this.ForegroundTextureName.SetPosition(pos);
            this.UISceneLayer = sceneLayer;
            this.ui_position = pos;
        }

        public UIComponent(UISceneLayer sceneLayer)
        {
            this.ForegroundTextureName = new Sprite();
            this.UISceneLayer = sceneLayer;
        }

        public UIComponent()
        {
            this.ForegroundTextureName = new Sprite();
        }

        //Most likely not needed, but if so can override
        public virtual StateEnum Update(double delta)
        {
            return StateEnum.InProgress;
        }

        public void SetTextureDimensions(int width, int height)
        {
            this.BackgroundHeight = height;
            this.BackgroundWidth = width;
        }

        //Alias for readablity
        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null)
        {
            if (this.IsVisible)
            {
                this.RenderForeground(spriteBatch);
            }
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        public void Hide()
        {
            this.IsVisible = false;
        }

        public Vector2 GetPosition()
        {
            return this.ui_position;
        }

        public virtual void SetPosition(Vector2 position)
        {
            this.ui_position = position;
            if (this.ForegroundTextureName?.position != null) this.ForegroundTextureName?.position.SetPosition(position);
        }

        public virtual void RenderForeground(SpriteBatch spriteBatch)
        {
            this.ForegroundTextureName.Render(spriteBatch);
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            //Default load the foreground texture and set the dimensions
            this.ForegroundTextureName.LoadContent(contentManager);
            this.SetTextureDimensions(this.ForegroundTextureName.width, this.ForegroundTextureName.height);
        }
        
        public bool IsWithinBounds(Point? p)
        {
            if (this.IsVisible && p.HasValue)
            {
                Point p2 = p.Value;

                float left = this.ui_position.X - (this.BackgroundWidth / 2);
                float right = left + this.BackgroundWidth;
                
                float top = this.ui_position.Y - (this.BackgroundHeight / 2);
                float bottom = top + this.BackgroundHeight;
                
                return p2.X >= left && p2.X <= right && p2.Y >= top && p2.Y <= bottom;
            }

            return false;
        }

        //Gets the uis background layer
        public string GetBackgroundTexturePath()
        {
            return Variables.AssetPaths.UI_PATH + "/" + BackgroundTextureName;
        }

        public abstract bool HandleMouseClicked(MouseEvent mouseEvent);
        public abstract void HandleMouseMoved(MouseEvent mouseEvent);

        public void HandleMouseDragged(MouseEvent mouseEvent)
        {
            throw new System.NotImplementedException();
        }

        public void HandleMouseWheeled(MouseEvent mouseEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
