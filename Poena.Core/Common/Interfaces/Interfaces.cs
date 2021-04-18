using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input.InputListeners;
using Poena.Core.Common.Interfaces;
using Poena.Core.Input.Actions;
using Poena.Core.Scene;

namespace Poena.Core.Common {

    public interface IRenderable
    {
        void LoadContent(ContentManager contentManager);
        void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null);
        StateEnum Update(double delta);
    }


    public interface INode
    {
        void Initialize();
        void Entry();
        void Exit();
        void Destroy();
    }

    public interface INodeObject : INode, IRenderable, IInputable, ISaveable
    {
        //TODO: Create a system to allow data passed. First thought is a builder
    }

    public interface ISaveable
    {
        void Load(string path);
        void Save(string path);
    }

    public interface IEntity : INodeObject
    {

    }

    public interface IBuilder<T>
    {
        T Build();
    }

    public interface IRemovable
    {
        bool IsFlagged();
    }

    public interface IRouteable : INode
    {
        int GetMovementCost(IRouteable mover = null);
    }

    public interface IUIComponent : IRenderable, IInputable
    {
        string GetBackgroundTexturePath();
        void SetTextureDimensions(int width, int height);
    }


}