using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Entity.Managers;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Enums;
using Project_Poena.Scene;
using Project_Poena.Cameras;
using Project_Poena.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Scene.SceneLayer
{
    public abstract class SceneLayer : BaseSceneLayer
    {
        protected List<INodeObject> layer_nodes;
        
        protected EntityManager entity_manager { get { return this.current_scene.entity_manager;  } }
        protected SystemManager system_manager { get { return this.current_scene.system_manager; } }

        public SceneLayer()
        {
            //Default objects
            this.layer_nodes = new List<INodeObject>();
            this.camera = new Camera();
        }
        
        public override void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            spriteBatch.Begin(transformMatrix: camera?.translation_matrix);

            this.RenderLayer(spriteBatch, camera.GetViewBounds());

            spriteBatch.End();
        }
        
        public T GetLayerNode<T>() where T : INodeObject
        {
            foreach (INodeObject obj in this.layer_nodes)
            {
                if (obj.GetType() == typeof(T)) return (T)obj;
            }

            return default(T);
        }
        
        public override StateEnum Update(double dt)
        {
            this.camera.Update(dt);
            return this.UpdateLayer(dt);
        }
        
        public abstract void RenderLayer(SpriteBatch spriteBatch, RectangleF camera_bounds);
        public abstract StateEnum UpdateLayer(double delta);
    }
}
