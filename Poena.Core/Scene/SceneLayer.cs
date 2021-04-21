using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Poena.Core.Common;
using Poena.Core.Entity.Managers;

namespace Poena.Core.Scene
{
    public abstract class SceneLayer : ISceneLayer
    {
        public AbstractScene CurrentScene { get; protected set; }
        public EntityManager EntityManager { get; set; }
        public SystemManager SystemManager { get; set; }

        protected Camera Camera { get; set; }
        protected List<INodeObject> LayerNodes { get; set; }

        public SceneLayer()
        {
            this.LayerNodes = new List<INodeObject>();
            this.Camera = new Camera();
        }
        
        public virtual void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            spriteBatch.Begin(transformMatrix: this.Camera?.translation_matrix);

            this.RenderLayer(spriteBatch, this.Camera.GetViewBounds());

            spriteBatch.End();
        }
        
        public T GetLayerNode<T>() where T : INodeObject
        {
            foreach (INodeObject obj in this.LayerNodes)
            {
                if (obj.GetType() == typeof(T)) return (T)obj;
            }

            return default(T);
        }
        
        public StateEnum Update(double dt)
        {
            this.Camera.Update(dt);
            return this.UpdateLayer(dt);
        }

        public virtual void WindowResizeEvent()
        {
            this.Camera.Resize();
        }

        public virtual void InjectScene(AbstractScene scene)
        {
            this.CurrentScene = scene;
        }

        public void MoveCamera(Vector2 pos)
        {
            this.Camera.MoveToPosition(pos);
        }
        
        public abstract void RenderLayer(SpriteBatch spriteBatch, RectangleF camera_bounds);
        public abstract StateEnum UpdateLayer(double delta);
        public abstract void Initialize();
        public abstract void Entry();
        public abstract void Exit();
        public abstract void Destroy();
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void HandleEvents();
        public abstract void Load(string path);
        public abstract void Save(string path);
        public abstract bool HandleMouseClicked(MouseEvent mouseEvent);
        public abstract void HandleMouseMoved(MouseEvent mouseEvent);
        public abstract void HandleMouseDragged(MouseEvent mouseEvent);
        public abstract void HandleMouseWheeled(MouseEvent mouseEvent);
    }
}
