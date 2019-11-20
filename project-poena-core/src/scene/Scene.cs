using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Entity.Managers;
using Project_Poena.Events;
using Project_Poena.Extensions;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Enums;
using Project_Poena.Input;
using Project_Poena.Managers;
using System.Collections.Generic;

namespace Project_Poena.Scene
{

    /*
     *  Scenes are the renderable objects of a screen
     *  
     *  They will have layers that need to be defined and placed positionally
     *      For example - Overworld
     *          Layer 1 - Game board
     *          Layer 2 - Characters
     *          Layer 3 - UI
     *          
     *   Scenes will be built via a SceneBuilder loading the attributes via xml
     *  
     */

    public abstract class AbstractScene : IRenderable, ISaveable
    {
        public LinkedList<ISceneLayer> scene_layers { get; protected set; }
        public List<InputMapping> scene_mappings { get; protected set; }

        public EntityManager entity_manager { get; protected set; }
        public SystemManager system_manager { get; protected set; }

        public AbstractScene()
        {
            this.scene_layers = new LinkedList<ISceneLayer>();
            this.entity_manager = new EntityManager();
            this.system_manager = new SystemManager(this);
        }

        public T GetSceneLayer<T>() where T : ISceneLayer
        {
            foreach (ISceneLayer layer in scene_layers)
            {
                if (layer.GetType() == typeof(T)) return (T)layer;
            }

            return default(T);
        }

        //0 = Background x = foreground
        public void AddLayer(ISceneLayer layer, int? position = null)
        {
            //Inject the scene
            layer.InjectScene(this);
            
            //We need to 'inject' the layer to the position wanted
            if (position != null && position <= 0)
            {
                scene_layers.AddFirst(layer);
            }
            else if (position == null || position > scene_layers.Count)
            {
                scene_layers.AddLast(layer);
            }
            else
            {
                LinkedListNode<ISceneLayer> node = scene_layers.First;
                for (int i = 0; i < position; i++)
                {
                    node = node.Next;
                }

                scene_layers.AddBefore(node, layer);
            }
        }
        
        public void Initialize()
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.Initialize();
            }
        }

        //Content is loaded enter the scene
        public void EnterScene()
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.Entry();
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.LoadContent(contentManager);
            }

            //TODO: Determine if this was the correct call
            this.EnterScene();
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.Render(spriteBatch, camera_bounds);
            }
        }

        public void HandleInput(InputHandler inputHandler)
        {
            //Convert the mappings we have into actions
            List<MappedInputAction> mappings = inputHandler.GetMappedInputs(this.scene_mappings);
            List<MappedInputAction> handled = new List<MappedInputAction>();

            //Handle these actions
            foreach (ISceneLayer layer in this.scene_layers)
            {
                layer.HandleInput(mappings);

                //Remove handled actions and add to list
                handled.AddRange(mappings.RemoveHandled());
            }
            
            //Remove the handled actions
            foreach (MappedInputAction mia in handled)
            {
                //Tell the handler to digest is
                inputHandler.DigestAction(mia.raw_action);
            }

            //TODO: rce - Consider an event channel
        }

        public StateEnum Update(double delta)
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.Update(delta);
            }

            //Clear any unused events in the scene
            EventQueueHandler.GetInstance().Clear();

            return this.GetState();
        }

        public void WindowResizeEvent()
        {
            foreach (ISceneLayer sl in scene_layers)
            {
                sl.WindowResizeEvent();
            }
        }

        public abstract StateEnum GetState();

        public abstract void Load(string path);

        public abstract void Save(string path);
    }

}