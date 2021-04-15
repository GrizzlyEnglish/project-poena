using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Interfaces;
using Project_Poena.Common.Rectangle;
using Project_Poena.Common.Enums;

namespace Project_Poena.Sprites
{
    public class Sprite : IRenderable
    {

        private Animation animation;

        private Texture2D texture;
        private string texture_path;

        private float scale = 1f;

        public SpritePosition position { get; private set; }

        public int width { get { return this.texture?.Width ?? 0;  } }

        public int height { get { return this.texture?.Height ?? 0; } }
        
        public bool is_visible { get; set; }
        
        private Vector2 anchor { get { return new Vector2(this.texture.Width * anchor_offset.X, this.texture.Height * anchor_offset.Y); } }

        private Vector2? _anchor_offset { get; set; }
        public Vector2 anchor_offset
        {
            get
            {
                return _anchor_offset ?? new Vector2(.5f, .5f);
            }
            set
            {
                this._anchor_offset = value;
            }
        }

        public Sprite(params string[] paths)
        {
            this.SetTexturePath(paths);
            this.position = new SpritePosition(new Vector2(0, 0));
            this.is_visible = true;
        }

        public Sprite()
        {
            this.position = new SpritePosition(new Vector2(0, 0));
            this.is_visible = true;
        }

        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        public void SetPosition(Vector2 pos)
        {
            this.position.SetPosition(pos);
        }
        
        public void SetTexturePath(params string[] paths)
        {
            if (paths.Length == 1) this.texture_path = paths[0];
            else this.animation = new Animation(paths);
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        public void LoadContent(ContentManager contentManager)
        {
            if (this.animation != null) this.animation.LoadContent(contentManager);
            else
            {
                this.texture = contentManager.Load<Texture2D>(this.texture_path);
            }
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds = null)
        {
            if (this.is_visible)
            {
                Texture2D texture;

                if (this.animation != null) texture = this.animation.current_frame;
                else texture = this.texture;

                Vector2 position = this.position.position;

                spriteBatch.Draw(texture, position, null, Color.White,
                        0, this.anchor, this.scale, SpriteEffects.None, 0);
            }
        }

        public StateEnum Update(double delta)
        {
            if (this.animation != null) this.animation.Update(delta);
            this.position.Update(delta);

            return StateEnum.InProgress;
        }
    }
}
