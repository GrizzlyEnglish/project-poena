using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Screen.Battle.Components;
using System.Linq;

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
            var entityComponents = ActiveEntities.Select(e => new
            {
                sprite = _spriteMapper.Get(e),
                pos = _positionMapper.Get(e)
            })
            .OrderBy(e => e.pos.TilePosition.Y);

            foreach (var entityComponent in entityComponents)
            {
                if (entityComponent.sprite.IsVisible)
                {
                    _spriteBatch.Draw(entityComponent.sprite.Texture, entityComponent.pos.TilePosition, null, Color.White,
                            0, entityComponent.sprite.Anchor, entityComponent.sprite.Scale, SpriteEffects.None, 0);

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
