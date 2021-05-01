using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Poena.Core.Common;
using Poena.Core.Common.Interfaces;
using Poena.Core.Extensions;

namespace Poena.Core.Utilities
{
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AvgFPS { get; private set; }
        public float CurrentFPS { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private readonly Queue<float> _sampleBuffer = new Queue<float>();
        private readonly Poena _poena;

        public FrameCounter(Poena poena)
        {
            _poena = poena;
        }

        public StateEnum Update(GameTime gameTime)
        {
            var dt = gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFPS = (float)(1.0 / dt);

            _sampleBuffer.Enqueue(CurrentFPS);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AvgFPS = _sampleBuffer.Average(i => i);
            }
            else
            {
                AvgFPS = CurrentFPS;
            }

            TotalFrames++;
            TotalSeconds += (float)dt;

            return StateEnum.InProgress;
        }

        public void Draw(GameTime gameTime)
        {
            string fps = string.Format("FPS: {0}", this.AvgFPS);
            _poena.SpriteBatch.Begin();
            _poena.SpriteBatch.DrawDebugString(fps, new Vector2(1, 1), Color.Black);
            _poena.SpriteBatch.End();
        }
    }
}