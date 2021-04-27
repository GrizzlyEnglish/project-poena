using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common;
using Poena.Core.Scene.UI;

namespace Poena.Core.Scene
{
    public abstract class UISceneLayer : SceneLayer
    {
        public bool Loaded { get; protected set; }
        protected class UITexture
        {
            public Texture2D texture;
            public Vector2 center;

            public UITexture(Texture2D text, Vector2 center)
            {
                this.texture = text;
                this.center = center;
            }
        }
        
        protected List<UIComponent> LayerComponents;
        protected Dictionary<string, UITexture> ComponentTextures;

        public UISceneLayer()
        {
            this.Camera = new Camera(is_static: true, current_zoom: 1f);
            this.LayerComponents = new List<UIComponent>();
            this.ComponentTextures = new Dictionary<string, UITexture>();
        }

        public void AddComponent(UIComponent component)
        {
            this.LayerComponents.Add(component);
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            //Load component backgrounds
            foreach (UIComponent comp in this.LayerComponents)
            {
                string name = comp.BackgroundTextureName;
                
                if (name != null && !this.ComponentTextures.ContainsKey(name))
                {
                    Texture2D texture = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + "/" + name);
                    Vector2 center = new Vector2(texture.Width / 2, texture.Height / 2);
                    this.ComponentTextures[name] = new UITexture(texture, center);
                }

                comp.LoadContent(contentManager);
            }

            Loaded = true;
        }

        public override void RenderLayer(SpriteBatch spriteBatch, RectangleF cameraBounds)
        {
            foreach (UIComponent comp in LayerComponents)
            {
                if (comp.BackgroundTextureName != null)
                {
                    //Render the backgrounds
                    Vector2 pos = comp.GetPosition();
                    UITexture ui_texture = this.ComponentTextures[comp.BackgroundTextureName];
                    Texture2D texture = ui_texture.texture;
                    Vector2 center = ui_texture.center;

                    spriteBatch.Draw(texture, pos, null, Color.White,
                        0, center, 1f, SpriteEffects.None, 0);
                }
                
                //Render the foreground
                comp.Render(spriteBatch, cameraBounds);
            }
        }

        public T GetComponent<T>() where T : UIComponent
        {
            foreach (UIComponent comp in this.LayerComponents)
            {
                if (comp.GetType() == typeof(T))
                {
                    return (T)comp;
                }
            }

            return default(T);
        }

        public override StateEnum UpdateLayer(double delta)
        {
            foreach(UIComponent comp in LayerComponents)
            {
                comp.Update(delta);
            }

            return StateEnum.InProgress;
        }
    }
}
