using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Cameras;
using Project_Poena.Input;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Enums;
using System.Collections.Generic;

namespace Project_Poena.Scene.SceneLayer
{
    public abstract class BaseSceneLayer : ISceneLayer
    {
        protected Camera camera;
        public AbstractScene current_scene { get; protected set; }

        public virtual void WindowResizeEvent()
        {
            this.camera.Resize();
        }

        public virtual void InjectScene(AbstractScene scene)
        {
            this.current_scene = scene;
        }

        public virtual List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
        {
            if (camera != null)
            {
                actions = camera.HandleInput(actions);
            }

            return this.HandleLayerInput(actions);
        }

        public void MoveCamera(Vector2 pos)
        {
            this.camera.MoveToPosition(pos);
        }
        
        public abstract void Initialize();
        public abstract void Entry();
        public abstract void Exit();
        public abstract void Destroy();
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null);
        public abstract StateEnum Update(double delta);
        public abstract List<MappedInputAction> HandleLayerInput(List<MappedInputAction> actions);
        public abstract void HandleEvents();
        public abstract void Load(string path);
        public abstract void Save(string path);
    }
}
