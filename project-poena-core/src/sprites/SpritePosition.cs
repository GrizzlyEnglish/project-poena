using Microsoft.Xna.Framework;
using Project_Poena.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_Poena.Sprites
{
    public class SpritePosition
    {
        /*
         * Current position of the animation
         */
        public Vector2 position { get; private set; }

        /*
         * Destination of moving the animation
         */
        private Vector2? destination;

        /*
         * Start position of the animation when moving
         */
        private Vector2? start_position;

        /*
         * When set the destination is set we are moving
         */
        public bool in_motion { get { return this.destination != null;  } }

        /*
         * Current time of update
         */
        private float time = 0;

        /*
         * Action to fire once movement is complete
         */
        private Action completionAction;

        /*
         * Sets the speed of the movement 
         */
        private float? _speed;
        public float speed { get { return _speed ?? 1; } set { _speed = value; } }

        public SpritePosition(Vector2 position)
        {
            this.SetPosition(position);
        }

        public void MoveTo(Vector2 destination, Action onCompletion = null)
        {
            this.start_position = this.position;
            this.destination = destination;
            this.completionAction = onCompletion;
        }

        public void MoveDistance(Vector2 distance, Action onCompletion = null)
        {
            this.MoveTo(this.position + distance, onCompletion);
        }

        public void SetPosition(Vector2 vector2)
        {
            this.position = vector2;
        }

        public void Update(double delta)
        {
            if (this.destination.HasValue)
            {
                //Lerp to the position
                this.time += (float)(delta * this.speed);
                this.position = this.start_position.Value.Lerp(this.destination.Value, time);
                if (this.time > 1)
                {
                    //We are now at the destination clean up
                    this.position = this.destination.Value;
                    this.completionAction?.Invoke();

                    //Clear the vars
                    this.time = 0;
                    this.destination = null;
                    this.start_position = null;
                    this.completionAction = null;
                }
            }
        }
    }
}
