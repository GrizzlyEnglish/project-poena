using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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
        public UISceneLayer UISceneLayer { get; protected set; }
        protected Sprite ForegroundSprite { get; set; }
        protected bool IsVisible { get; set; }

        public float Width { get { return ForegroundSprite.Width; } }
        public float Height { get { return ForegroundSprite.Height; } }

        protected Vector2 UIPosition
        {
            get { return this.ForegroundSprite.Position.position; }
            set { this.ForegroundSprite.Position.SetPosition(value); }
        }

        public UIComponent(UISceneLayer sceneLayer, Vector2 pos)
        {
            this.ForegroundSprite = new Sprite();
            this.ForegroundSprite.SetPosition(pos);
            this.UISceneLayer = sceneLayer;
            this.UIPosition = pos;
        }

        public UIComponent(UISceneLayer sceneLayer)
        {
            this.ForegroundSprite = new Sprite();
            this.UISceneLayer = sceneLayer;
        }

        protected UIComponent()
        {
            this.ForegroundSprite = new Sprite();
        }

        //Most likely not needed, but if so can override
        public virtual StateEnum Update(double delta)
        {
            return StateEnum.InProgress;
        }

        public void Render(SpriteBatch spriteBatch, RectangleF cameraBounds)
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
            return this.UIPosition;
        }

        public virtual void SetPosition(Vector2 position)
        {
            this.UIPosition = position;
            if (this.ForegroundSprite?.Position != null) this.ForegroundSprite?.Position.SetPosition(position);
        }

        public virtual void RenderForeground(SpriteBatch spriteBatch)
        {
            this.ForegroundSprite.Render(spriteBatch, default(RectangleF));
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            //Default load the foreground texture and set the dimensions
            this.ForegroundSprite.LoadContent(contentManager);
        }
        
        //Gets the uis background layer
        public string GetBackgroundTexturePath()
        {
            return Assets.UI_PATH + "/" + BackgroundTextureName;
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
