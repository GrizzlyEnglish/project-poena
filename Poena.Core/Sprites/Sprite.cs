using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;

namespace Poena.Core.Sprites
{
    public class Sprite : IRenderable
    {

        private Animation Animation;
        private Texture2D Texture;
        private string TexturePath;
        private float Scale = 1f;

        public SpritePosition Position { get; private set; }
        public bool IsVisible { get; set; }
        public RectangleF Dimensions
        {
            get
            {
                float width = this.Texture.Width * this.Scale;
                float height = this.Texture.Height * this.Scale;
                float x = this.Position.position.X - (width / 2);
                float y = this.Position.position.Y - (height / 2);
                return new RectangleF(x, y, width, height);
            }
        }
        
        private Vector2 Anchor { get { return new Vector2(this.Texture.Width * AnchorOffset.X, this.Texture.Height * AnchorOffset.Y); } }
        private Vector2? _AnchorOffset { get; set; }
        public Vector2 AnchorOffset
        {
            get
            {
                return _AnchorOffset ?? new Vector2(.5f, .5f);
            }
            set
            {
                this._AnchorOffset = value;
            }
        }

        public Sprite(params string[] paths)
        {
            this.SetTexturePath(paths);
            this.Position = new SpritePosition(new Vector2(0, 0));
            this.IsVisible = true;
        }

        public Sprite()
        {
            this.Position = new SpritePosition(new Vector2(0, 0));
            this.IsVisible = true;
        }

        public void SetTexture(Texture2D texture)
        {
            this.Texture = texture;
        }

        public void SetPosition(Vector2 pos)
        {
            this.Position.SetPosition(pos);
        }
        
        public void SetTexturePath(params string[] paths)
        {
            if (paths.Length == 1) this.TexturePath = paths[0];
            else this.Animation = new Animation(paths);
        }

        public void SetScale(float scale)
        {
            this.Scale = scale;
        }

        public void LoadContent(ContentManager contentManager)
        {
            if (this.Animation != null) this.Animation.LoadContent(contentManager);
            else
            {
                this.Texture = contentManager.Load<Texture2D>(this.TexturePath);
            }
        }

        public void Render(SpriteBatch spriteBatch, RectangleF cameraBounds)
        {
            if (this.IsVisible)
            {
                Texture2D texture;

                if (this.Animation != null) texture = this.Animation.current_frame;
                else texture = this.Texture;

                Vector2 position = this.Position.position;

                spriteBatch.Draw(texture, position, null, Color.White,
                        0, this.Anchor, this.Scale, SpriteEffects.None, 0);

                if (Config.DEBUG_RENDER)
                {
                    RectangleF dim = this.Dimensions;
                    spriteBatch.Draw(Config.DEBUG_TEXTURE, new Rectangle((int)dim.Left, (int)dim.Top, (int)dim.Width, 3), Color.White);
                    spriteBatch.Draw(Config.DEBUG_TEXTURE, new Rectangle((int)dim.Right, (int)dim.Top, 3, (int)dim.Height), Color.White);
                    spriteBatch.Draw(Config.DEBUG_TEXTURE, new Rectangle((int)dim.Left, (int)dim.Bottom, (int)dim.Width, 3), Color.White);
                    spriteBatch.Draw(Config.DEBUG_TEXTURE, new Rectangle((int)dim.Left, (int)dim.Top, 3, (int)dim.Height), Color.White);
                }
            }
        }

        public StateEnum Update(double delta)
        {
            if (this.Animation != null) this.Animation.Update(delta);
            this.Position.Update(delta);

            return StateEnum.InProgress;
        }

        public bool IsWithinBounds(Point? p)
        {
            if (this.IsVisible && p.HasValue)
            {
                Point p2 = p.Value;
                RectangleF bounds = this.Dimensions;
                
                return p2.X >= bounds.Left && p2.X <= bounds.Right && p2.Y >= bounds.Top && p2.Y <= bounds.Bottom;
            }

            return false;
        }
    }
}
