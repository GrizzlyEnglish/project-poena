using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Input;
using Project_Poena.Cameras;
using Project_Poena.Common.Variables;
using Project_Poena.Common.Enums;
using Project_Poena.Common.Rectangle;

namespace Project_Poena.Scene.SceneLayer
{
    public abstract class UISceneLayer : BaseSceneLayer
    {
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
        
        protected List<UIComponent> layer_components;
        protected Dictionary<string, UITexture> component_textures;

        public UISceneLayer()
        {
            this.camera = new Camera(is_static: true, current_zoom: 1f);
            this.layer_components = new List<UIComponent>();
            this.component_textures = new Dictionary<string, UITexture>();
        }

        public void AddComponent(UIComponent component)
        {
            layer_components.Add(component);
        }
        
        public override void LoadContent(ContentManager contentManager)
        {
            //Load component backgrounds
            foreach (UIComponent comp in layer_components)
            {
                string name = comp.background_texture_name;
                
                if (name != null && !this.component_textures.ContainsKey(name))
                {
                    Texture2D texture = contentManager.Load<Texture2D>(Variables.AssetPaths.UI_PATH + "/" + name);
                    Vector2 center = new Vector2(texture.Width / 2, texture.Height / 2);
                    this.component_textures[name] = new UITexture(texture, center);
                }

                comp.LoadContent(contentManager);
            }
        }

        public override void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null)
        {
            spriteBatch.Begin(transformMatrix: camera?.translation_matrix);
            
            foreach (UIComponent comp in layer_components)
            {
                if (comp.background_texture_name != null)
                {
                    //Render the backgrounds
                    Vector2 pos = comp.GetPosition();
                    UITexture ui_texture = this.component_textures[comp.background_texture_name];
                    Texture2D texture = ui_texture.texture;
                    Vector2 center = ui_texture.center;

                    spriteBatch.Draw(texture, pos, null, Color.White,
                        0, center, 1f, SpriteEffects.None, 0);
                }
                
                //Render the foreground
                comp.Render(spriteBatch);
            }

            spriteBatch.End();
        }

        public Camera GetCamera()
        {
            return this.camera;
        }

        public T GetComponent<T>() where T : UIComponent
        {
            foreach (UIComponent comp in this.layer_components)
            {
                if (comp.GetType() == typeof(T))
                {
                    return (T)comp;
                }
            }

            return default(T);
        }

        public override StateEnum Update(double delta)
        {
            foreach(UIComponent comp in layer_components)
            {
                comp.Update(delta);
            }

            return StateEnum.InProgress;
        }

        public override List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
        {
            //actions.UnprojectCoordinates(this.camera.UnProjectCoordinatesToPoint);
            foreach (UIComponent comp in layer_components)
            {
                actions = comp.HandleInput(actions);
            }

            return actions;
        }

        public override void WindowResizeEvent()
        {
            base.WindowResizeEvent();

            //Reset the components positions
            this.Entry();
        }
        
        public abstract StateEnum UpdateLayer(double delta);
    }
}
