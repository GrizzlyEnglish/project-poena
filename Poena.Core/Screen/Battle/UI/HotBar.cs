using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Screen.Battle.Components;
using Poena.Core.Utilities;
using System;

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
            SetPosition();
        }

        public void SetPosition()
        {
            RectangleF cameraBounds = _battle.Camera.BoundingRectangle;
            this._backgroundPosition = new Vector2((cameraBounds.Width / 2) - (_background.Width / 2), (cameraBounds.Height * .87f) - _background.Height);
        }

        public void Draw(GameTime gameTime)
        {
            if (this._isVisible)
            {
                this._battle.Game.SpriteBatch.Draw(_background, _backgroundPosition, null, Color.White,
                    0, new Vector2(.5f, .5f), 1f, SpriteEffects.None, 0);

                if (Config.DEBUG_RENDER)
                {
                    this._battle.Game.SpriteBatch.DrawRectangle(GetDimensions(), Color.White, 3);
                }
                for (int i = 0; i < ICON_LENGTH; i++)
                {
                    Texture2D icon = this._battle.AssetManager.GetTexture(Assets.UI_PATH + _iconPaths[i]);
                    if (icon != null)
                    {
                        this._battle.Game.SpriteBatch.Draw(icon, new Vector2(_backgroundPosition.X + (i * 100) + 8, _backgroundPosition.Y + 7), null, Color.White,
                            0, new Vector2(.5f, .5f), .17f, SpriteEffects.None, 0);
                    }
                }
            }
        }

        public void GenerateIcons(Entity ent)
        {
            // Weapon skill icon

            // Skill icon
            SkillComponent skill = ent.Get<SkillComponent>();
            this._iconPaths[1] = skill.HotBarTexturePath;

            // Items

            // Block

            // Stall
        }

        public void Update(GameTime gameTime)
        {
            //Check the event channel if someone new was selected
            Entity selectedEntity = this._battle.BoardSystem.GetSelectedEntity();
            
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
            if (this._isVisible && GetDimensions().Contains(screenCoords.ToPoint()))
            {
                float slotDistance = _background.Width / 5f;
                int slot = (int)Math.Floor((screenCoords.X - _backgroundPosition.X) / slotDistance);
                Logger.Log("Hotbar", $"Clicked slot {slot}");
                this._battle.BoardSystem.HandleHotBarInteraction((AttackType)slot);
                return true;
            }

            return false;
        }

        public RectangleF GetDimensions()
        {
            return new RectangleF
            {
                Height = _background.Height,
                X = _backgroundPosition.X,
                Y = _backgroundPosition.Y,
                Width = _background.Width
            };
        }
    }
}
