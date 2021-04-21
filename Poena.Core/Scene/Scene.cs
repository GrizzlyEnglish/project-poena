using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;

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
namespace Poena.Core.Scene
{
    public abstract class AbstractScene : IRenderable, ISaveable
    {
        public LinkedList<ISceneLayer> SceneLayers { get; protected set; }
        /// <summary>
        /// The namespace of the scene for the event handler
        /// </summary>
        /// <value>Public get witha protected set to allow the intertinace to set</value>
        public string SceneNamespace { get; protected set; }
        public MouseListener MouseListener { get; protected set; }
        public TouchListener TouchListener { get; protected set; }

        public AbstractScene(MouseListener mouseListener, TouchListener touchListener)
        {
            this.SceneLayers = new LinkedList<ISceneLayer>();
            this.MouseListener = mouseListener;
            this.TouchListener = touchListener;

            this.MouseListener.MouseClicked += MouseClicked;
            this.MouseListener.MouseMoved += MouseMoved;
            this.MouseListener.MouseDrag += MouseDragged;
            this.MouseListener.MouseWheelMoved += MouseWheeled;
        }

        public T GetSceneLayer<T>() where T : ISceneLayer
        {
            foreach (ISceneLayer layer in this.SceneLayers)
            {
                if (layer.GetType() == typeof(T)) return (T)layer;
            }

            throw new System.Exception("Layer not found");
        }

        //0 = Background x = foreground
        public void AddLayer(ISceneLayer layer, int? position = null)
        {
            //Inject the scene
            layer.InjectScene(this);

            //We need to 'inject' the layer to the position wanted
            if (position != null && position <= 0)
            {
                SceneLayers.AddFirst(layer);
            }
            else if (position == null || position > SceneLayers.Count)
            {
                SceneLayers.AddLast(layer);
            }
            else
            {
                LinkedListNode<ISceneLayer> node = SceneLayers.First;
                for (int i = 0; i < position; i++)
                {
                    node = node.Next;
                }

                SceneLayers.AddBefore(node, layer);
            }
        }

        public void Initialize()
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.Initialize();
            }
        }

        //Content is loaded enter the scene
        public void EnterScene()
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.Entry();
            }
        }

        public void LoadContent(ContentManager contentManager)
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.LoadContent(contentManager);
            }

            //TODO: Determine if this was the correct call
            this.EnterScene();
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.Render(spriteBatch, camera_bounds);
            }
        }

        public StateEnum Update(double delta)
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.Update(delta);
            }

            return this.GetState();
        }

        public void WindowResizeEvent()
        {
            foreach (ISceneLayer sl in SceneLayers)
            {
                sl.WindowResizeEvent();
            }
        }

        public void MouseDragged(object sender, MouseEventArgs mouseEventArgs)
        {
            // Wrap the mouse event
            MouseEvent mouseEvent = new MouseEvent(mouseEventArgs);
            foreach (ISceneLayer layer in this.SceneLayers)
            {
                layer.HandleMouseDragged(mouseEvent);
            }
        }

        public void MouseClicked(object sender, MouseEventArgs mouseEventArgs)
        {
            // Wrap the mouse event
            MouseEvent mouseEvent = new MouseEvent(mouseEventArgs);
            bool handled = false;
            foreach (ISceneLayer layer in this.SceneLayers)
            {
                if (!handled)
                {
                    handled = layer.HandleMouseClicked(mouseEvent);
                }
            }
        }

        public void MouseMoved(object sender, MouseEventArgs mouseEventArgs)
        {
            // Wrap the mouse event
            MouseEvent mouseEvent = new MouseEvent(mouseEventArgs);
            foreach (ISceneLayer layer in this.SceneLayers)
            {
                layer.HandleMouseMoved(mouseEvent);
            }
        }

        public void MouseWheeled(object sender, MouseEventArgs mouseEventArgs)
        {
            // Wrap the mouse event
            MouseEvent mouseEvent = new MouseEvent(mouseEventArgs);
            foreach (ISceneLayer layer in this.SceneLayers)
            {
                layer.HandleMouseWheeled(mouseEvent);
            }
        }

        public abstract StateEnum GetState();

        public abstract void Load(string path);

        public abstract void Save(string path);
    }

}