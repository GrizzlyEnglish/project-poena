using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Managers;
using Poena.Core.Screen.Battle.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Screen.Battle.Systems
{
    public class TurnRenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<TurnComponent> _turnMapper;

        public TurnRenderSystem(SpriteBatch spriteBatch, AssetManager assetManager)
            : base(Aspect.All(typeof(TurnComponent)))
        {
            _spriteBatch = spriteBatch;
            _assetManager = assetManager;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            Texture2D turn_bar_background = _assetManager.GetTexture(Assets.GetUIElement(UIElements.EmptyActionBar));
            Texture2D turn_bar_foreground = _assetManager.GetTexture(Assets.GetUIElement(UIElements.BlueActionBar));
            Vector2 sprite_center = new Vector2(turn_bar_foreground.Width / 2, turn_bar_foreground.Height / 2);

            // Loop anybody that has a turn comp and render there bar
            foreach (int entityId in ActiveEntities)
            {
                PositionComponent pos = _positionMapper.Get(entityId);
                TurnComponent turn = _turnMapper.Get(entityId);
                
                // Shift it up a bit
                Vector2 position = new Vector2((int)pos.TilePosition.X, (int)pos.TilePosition.Y - 100);

                // Draw the fully background
                _spriteBatch.Draw(turn_bar_background, position, null, Color.White,
                    0, sprite_center, 0.5f, SpriteEffects.None, 0);

                // We need to calc the rectange to draw
                int width = (int)(turn_bar_foreground.Width * (turn.current_time / turn.time_for_turn));
                Rectangle clipping_rect = new Rectangle(0, 0, width, turn_bar_foreground.Height);
                
                // Draw the clipped foreground
                _spriteBatch.Draw(turn_bar_foreground, position, clipping_rect, Color.White,
                    0, sprite_center, 0.5f, SpriteEffects.None, 0);
            }
        }
    }
}
