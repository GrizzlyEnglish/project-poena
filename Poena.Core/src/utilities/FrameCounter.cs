using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project_Poena.Common.Interfaces;
using Project_Poena.Extensions;
using Project_Poena.Common.Variables;
using Project_Poena.Common.Enums;
using Project_Poena.Common.Rectangle;

namespace Project_Poena.Utilities
{
    public class FrameCounter : IRenderable
    {
        public FrameCounter()
        {
        }

        public long total_frames { get; private set; }
        public float total_seconds { get; private set; }
        public float average_fps { get; private set; }
        public float current_fps { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> _sampleBuffer = new Queue<float>();

        public void LoadContent(ContentManager contentManager){}

        public StateEnum Update(double deltaTime)
        {
            var dt = (float)deltaTime;
            current_fps = (float)(1.0 / dt);

            _sampleBuffer.Enqueue(current_fps);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                average_fps = _sampleBuffer.Average(i => i);
            }
            else
            {
                average_fps = current_fps;
            }

            total_frames++;
            total_seconds += dt;

            return StateEnum.InProgress;
        }

        public void Render(SpriteBatch spriteBatch, RectangleF camera_bounds)
        {
            var fps = string.Format("FPS: {0}", this.average_fps);
            spriteBatch.Begin();
            spriteBatch.DrawDebugString(fps, new Vector2(1, 1), Color.Black);
            spriteBatch.End();
        }
    }
}