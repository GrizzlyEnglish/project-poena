using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.UI
{
    public class HotBar
    {
        private readonly int ICON_LENGTH = 5;

        private bool _isVisible;
        
        private readonly Battle _battle;
        private string[] _iconPaths;

        private Texture2D _background;

        private Vector2 _backgroundPosition;

        public HotBar(Battle battle)
        {
            this._battle = battle;
            this._iconPaths = new string[ICON_LENGTH];
        }

        public void LoadContent()
        {
            this._background = this._battle.Content.Load<Texture2D>(Assets.GetUIElement(UIElements.HotBar));

            RectangleF cameraBounds = _battle.Camera.BoundingRectangle;
            this._backgroundPosition = new Vector2((cameraBounds.Width / 2), cameraBounds.Height - (_background.Height / 2) - 15);
        }

        public void Draw(GameTime gameTime)
        {
            if (this._isVisible)
            {
                this._battle.Game.SpriteBatch.Draw(_background, _backgroundPosition, null, Color.White,
                    0, new Vector2(.5f, .5f), 1f, SpriteEffects.None, 0);
                int distance = 100;
                for (int i = 0; i < ICON_LENGTH; i++)
                {
                    Texture2D icon = this._battle.AssetManager.GetTexture(Assets.UI_PATH + _iconPaths[i]);
                    if (icon != null)
                    {
                        this._battle.Game.SpriteBatch.Draw(icon, new Vector2(_backgroundPosition.X + distance, _backgroundPosition.Y), null, Color.White,
                            0, new Vector2(.5f, .5f), .17f, SpriteEffects.None, 0);
                    }
                }
            }
        }

        public void GenerateIcons(Entity ent)
        {
            // Skill icon
            SkillComponent skill = ent.Get<SkillComponent>();
            this._iconPaths[0] = skill.HotBarTexturePath;

            // Weapon skill icon

            // Items

            // Block

            // Stall
        }

        public void Update(GameTime gameTime)
        {
            //Check the event channel if someone new was selected
            Entity selectedEntity = this._battle.SelectionSystem.GetSelectedEntity();
            
            if (selectedEntity != null && !this._isVisible)
            {
                // Show the hot bar
                this.GenerateIcons(selectedEntity);
                this._isVisible = true;
            }
            else if (selectedEntity == null && this._isVisible)
            {
                this._isVisible = false;
            }
        }

        public bool HandleMouseClicked(Vector2 screenCoords)
        {
            if (this._isVisible && IsWithinBounds(screenCoords))
            {
            }

            return false;
        }

        public bool IsWithinBounds(Vector2 pos)
        {
            float left = this._backgroundPosition.X - (this._background.Width / 2);
            float right = left + this._background.Width;
            
            float top = this._backgroundPosition.Y - (this._background.Height / 2);
            float bottom = top + this._background.Height;
            
            return pos.X >= left && pos.X <= right && pos.Y >= top && pos.Y <= bottom;
        }
    }
}
