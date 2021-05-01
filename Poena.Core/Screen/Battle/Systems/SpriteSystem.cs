using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class SpriteSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<SpriteComponent> _spriteMapper;
        private ComponentMapper<PositionComponent> _positionMapper;

        public SpriteSystem(SpriteBatch spriteBatch)
            : base(Aspect.All(typeof(SpriteComponent)))
        {
            this._spriteBatch = spriteBatch;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _spriteMapper = mapperService.GetMapper<SpriteComponent>();
            _positionMapper = mapperService.GetMapper<PositionComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {
                SpriteComponent sprite = _spriteMapper.Get(entityId);
                PositionComponent pos = _positionMapper.Get(entityId);
                if (sprite.IsVisible)
                {
                    _spriteBatch.Draw(sprite.Texture, pos.tile_position, null, Color.White,
                            0, sprite.Anchor, sprite.Scale, SpriteEffects.None, 0);

                    if (Config.DEBUG_RENDER)
                    {
                        RectangleF dim = GetDimensions(pos.tile_position, sprite.Anchor, sprite.Width, sprite.Height);
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
