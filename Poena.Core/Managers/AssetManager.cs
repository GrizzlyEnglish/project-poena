using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Managers
{
    public class AssetManager
    {
        private readonly ContentManager _contentManager;
        private readonly Dictionary<string, Texture2D> _textures;

        public AssetManager(ContentManager content)
        {
            _contentManager = content;
            _textures = new Dictionary<string, Texture2D>();
        }

        public void LoadTexture(string name)
        {
            Texture2D texture = _contentManager.Load<Texture2D>(name);
            _textures.Add(name, texture);
        }

        public Texture2D GetTexture(string name)
        {
            return _textures[name];
        }
    }
}
