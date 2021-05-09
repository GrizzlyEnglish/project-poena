using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Poena.Core.Common;
using Poena.Core.Managers;
using Poena.Core.Screen.Battle.Board;
using Poena.Core.Screen.Battle.Components;

namespace Poena.Core.Screen.Battle.Systems
{
    public class TileHighlightSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;
        private ComponentMapper<TileHighlightComponent> _tileHighlightMapper;

        public TileHighlightSystem(SpriteBatch batch, AssetManager assetManager) 
            : base(Aspect.One(typeof(TileHighlightComponent)))
        {
            _assetManager = assetManager;
            _spriteBatch = batch;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tileHighlightMapper = mapperService.GetMapper<TileHighlightComponent>();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (int entityId in ActiveEntities)
            {
                // Loop the positions and tag tiles as movement
                TileHighlightComponent highlight = _tileHighlightMapper.Get(entityId);
                if (highlight != null)
                {
                    List<Vector2> highlights = highlight.HighlightCheck ? highlight.HighlightPositions : highlight.PossiblePositions;
                    foreach (Vector2 path_spot in highlights)
                    {
                        Point p = Coordinates.WorldToBoard(path_spot);
                        Coordinates coordinates = Coordinates.BoardToWorld(p);
                        _spriteBatch.Draw(_assetManager.GetTexture(Assets.GetTileHighlight(highlight.TileHighlight)), coordinates.AsVector2(), Color.White);
                    }
                }
            }
        }
    }
}
