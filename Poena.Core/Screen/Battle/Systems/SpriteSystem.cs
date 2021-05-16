using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Common.Enums;
using Poena.Core.Managers;
using Poena.Core.Screen.Battle.Components;
using System.Linq;

namespace Poena.Core.Screen.Battle.Systems
{
    public class SpriteSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;

        private ComponentMapper<SpriteComponent> _spriteMapper;
        private ComponentMapper<PositionComponent> _positionMapper;
        private ComponentMapper<TurnComponent> _turnMapper;

        public SpriteSystem(SpriteBatch spriteBatch, AssetManager assetManager)
            : base(Aspect.All(typeof(SpriteComponent), typeof(PositionComponent), typeof(TurnComponent)))
        {
            this._spriteBatch = spriteBatch;
            this._assetManager = assetManager;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _spriteMapper = mapperService.GetMapper<SpriteComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
            _turnMapper = mapperService.GetMapper<TurnComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            var entityComponents = ActiveEntities.Select(e => new
            {
                sprite = _spriteMapper.Get(e),
                pos = _positionMapper.Get(e),
                turn = _turnMapper.Get(e)
            })
            .OrderBy(e => e.pos.TilePosition.Y);

            Texture2D turn_bar_background = _assetManager.GetTexture(Assets.GetUIElement(UIElements.EmptyActionBar));
            Texture2D turn_bar_foreground = _assetManager.GetTexture(Assets.GetUIElement(UIElements.BlueActionBar));
            Vector2 sprite_center = new Vector2(turn_bar_foreground.Width / 2, turn_bar_foreground.Height / 2);

            foreach (var entityComponent in entityComponents)
            {
                if (entityComponent.sprite.IsVisible)
                {
                    // Render the entity
                    _spriteBatch.Draw(entityComponent.sprite.Texture, entityComponent.pos.TilePosition, null, Color.White,
                                0, entityComponent.sprite.Anchor, entityComponent.sprite.Scale, SpriteEffects.None, 0);

                    // Render the turn

                    // Shift it up a bit
                    Vector2 position = new Vector2((int)entityComponent.pos.TilePosition.X, (int)entityComponent.pos.TilePosition.Y - 100);

                    // Draw the fully background
                    _spriteBatch.Draw(turn_bar_background, position, null, Color.White,
                        0, sprite_center, 0.5f, SpriteEffects.None, 0);

                    // We need to calc the rectange to draw
                    int width = (int)(turn_bar_foreground.Width * (entityComponent.turn.CurrentTime / entityComponent.turn.TimeForTurn));
                    Rectangle clipping_rect = new Rectangle(0, 0, width, turn_bar_foreground.Height);
                    
                    // Draw the clipped foreground
                    _spriteBatch.Draw(turn_bar_foreground, position, clipping_rect, Color.White,
                        0, sprite_center, 0.5f, SpriteEffects.None, 0);

                    if (Config.DEBUG_RENDER)
                    {
                        RectangleF dim = GetDimensions(entityComponent.pos.TilePosition, entityComponent.sprite.Anchor, entityComponent.sprite.Width, entityComponent.sprite.Height);
                        _spriteBatch.DrawRectangle(dim, Color.White, 3);
                    }
                }
            }
        }

        public RectangleF GetDimensions(Vector2 center, Vector2 scale, float width, float height)
        {
            float x = center.X - scale.X;
            float y = center.Y - scale.Y;
            return new RectangleF(x, y, width, height);
        }
    }
}
