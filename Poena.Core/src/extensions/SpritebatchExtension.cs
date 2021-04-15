using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Variables;

namespace Project_Poena.Extensions
{
    public static class SpritebatchExtension
    {
        public static Texture2D white_texture;
        public static SpriteFont sprite_font;

        public static void LoadContent(this SpriteBatch batch, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            white_texture = new Texture2D(graphicsDevice, 1, 1);
            white_texture.SetData(new Color[] { Color.White });

            sprite_font = contentManager.Load<SpriteFont>(Variables.AssetPaths.FONT_PATH + "BaseFont");
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rectangle, Color color, int scale)
        {
            batch.Draw(white_texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, scale), color);
            batch.Draw(white_texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, scale), color);
            batch.Draw(white_texture, new Rectangle(rectangle.Left, rectangle.Top, scale, rectangle.Height), color);
            batch.Draw(white_texture, new Rectangle(rectangle.Right, rectangle.Top, scale, rectangle.Height + scale), color);
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rectangle, int scale)
        {
            batch.DrawRectangle(rectangle, Color.Red, scale);
        }

        public static void DrawDebugString(this SpriteBatch batch, string message, Vector2 position, Color color)
        {
            batch.DrawString(sprite_font, message, position, color);
        }

        public static void DrawDebugString(this SpriteBatch batch, string message, Vector2 position)
        {
            batch.DrawString(sprite_font, message, position, Color.White);
        }

    }
}
