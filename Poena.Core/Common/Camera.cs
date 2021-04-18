using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poena.Core.Common;
using Poena.Core.Extensions;
using Poena.Core.Input.Actions;
using Poena.Core.Input.Extensions;

namespace Poena.Core.Common
{

    public class Camera
    {
        //The center of the camera position
        private float x_position;
        private float y_position;
        
        private float translation_diff;

        private Rectangle? camera_bounds;

        private float zoom = 0.03f;
        private float max_zoom = 1f;
        private float min_zoom = 0.01f;
        private float zoom_scale = 0.015f;
        private bool is_static = false;

        private bool ignore_inputs;
        
        private Vector2? move_to_position;
        private Vector2? start_position;
        private float movement_time = 0;

        private List<InputWatcher> actionWatchers { get; set; }
        
        public Matrix translation_matrix { get; set; }

        public Camera(float x = 0, float y = 0,
            bool is_static = false, float translation_difference = 10, float max_zoom = 1f, float min_zoom = 0.01f, float current_zoom = 0.03f)
        {
            this.x_position = x;
            this.y_position = y;
            this.is_static = is_static;
            this.translation_diff = translation_difference;
            this.max_zoom = max_zoom;
            this.min_zoom = min_zoom;
            this.zoom = current_zoom;
            actionWatchers = new List<InputWatcher>()
            {
                new InputWatcher("left", mia => this.Translate(new Vector2(-translation_diff, 0))),
                new InputWatcher("right", mia => this.Translate(new Vector2(translation_diff, 0))),
                new InputWatcher("up", mia => this.Translate(new Vector2(0, -translation_diff))),
                new InputWatcher("down", mia => this.Translate(new Vector2(0, translation_diff))),
                new InputWatcher("left_button", ActionType.Held, mia => this.Translate(-1 * mia.raw_action.distance)),
                new InputWatcher("zoom", mia => this.Zoom(mia.raw_action.distance.Y, mia.raw_action.position?.ToVector2())),
            };
            
            this.Resize();
        }
        
        public List<MappedInputAction> HandleInput(List<MappedInputAction> actions)
        {
            //Find any mouse positions
            actions.UnprojectCoordinates(pos => this.UnProjectCoordinates(pos));

            if (!this.is_static && !this.ignore_inputs)
            {
                actions.FireAvailableWatchers(actionWatchers);
            }

            return actions;
        }

        public void ClampCamera(Rectangle bounds, bool centerOnBounds = false)
        {
            this.camera_bounds = bounds;
            
            if (centerOnBounds)
            {
                this.SetPosition(bounds.Center.ToVector2());
            }
        }

        private Vector2 viewport_center
        {
            get
            {
                return new Vector2(Variables.VIEWPORT_WIDTH * .5f, Variables.VIEWPORT_HEIGHT * .5f);
            }
        }

        public Vector2 top_left
        {
            get
            {
                float cx = this.x_position - (this.width / 2f);
                float cy = this.y_position - (this.height / 2f);

                return new Vector2(cx, cy);
            }
            set
            {
                value.X += (this.width / 2f);
                value.Y += (this.height / 2f);

                this.center = value;
            }
        }

        public float width
        {
            //TODO: rce - determine if we need to set the width
            get
            {
                return Variables.VIEWPORT_WIDTH / this.zoom;
            }
        }

        public float height
        {
            //TODO: rce - determine if we need to set the height
            get
            {
                return Variables.VIEWPORT_HEIGHT / this.zoom;
            }
        }

        public Vector2 center
        {
            get
            {
                return new Vector2(this.x_position, this.y_position);
            }
            set
            {
                this.x_position = value.X;
                this.y_position = value.Y;
                this.UpdateTranslationMatrix();
            }
        }

        public Vector2 UnProjectCoordinates(Vector2 coords, float? scale = null)
        {
            float zoom = scale ?? this.zoom;

            Vector2 cam_pos = this.top_left;

            float c_x = cam_pos.X + (coords.X / zoom);
            float c_y = cam_pos.Y + (coords.Y / zoom);

            return new Vector2(c_x, c_y);
        }

        public Vector2 UnProjectCoordinates(Point coords, float? scale = null)
        {
            return this.UnProjectCoordinates(coords.ToVector2());
        }

        public void SetPosition(Vector2 pos)
        {
            this.SetPosition(pos.X, pos.Y);
        }

        public void SetPosition(float x, float y)
        {
            if (!is_static)
            {
                //TODO: Vefify bounds
                this.x_position = x;
                this.y_position = y;
                this.UpdateTranslationMatrix();
            }
        }

        public void Translate(Vector2 trans)
        {
            this.Translate(trans.X, trans.Y);
        }

        public void Translate(float x = 0, float y = 0, bool adjustForScale = true)
        {
            if (!is_static)
            {
                float x_diff = x;
                float y_diff = y;
                if (adjustForScale)
                {
                    x_diff /= this.zoom;
                    y_diff /= this.zoom;
                }
                this.x_position += x_diff;
                this.y_position += y_diff;
                this.CheckTranslationClamp(x_diff, y_diff);
                this.UpdateTranslationMatrix();
            }
        }

        private void CheckTranslationClamp(float x_diff, float y_diff)
        {
            //Nothing to check return
            if (!this.camera_bounds.HasValue) return;

            Rectangle rect = this.camera_bounds.Value;
            
            //This gives the furthest x and y values
            Point top_left = new Point(rect.Left, rect.Top);
            Point bottom_right = new Point(rect.Right, rect.Bottom);
            
            //Check the horizontal position
            if (this.top_left.X < top_left.X || (this.top_left.X + this.width) > bottom_right.X)
            {
                //Remove the recently added difference
                this.x_position -= x_diff;
            }

            //Chec the vertical position
            if (this.top_left.Y < top_left.Y || (this.top_left.Y + this.height) > bottom_right.Y)
            {
                //Remove the recently added difference
                this.y_position -= y_diff;
            }
        }

        public void SetZoom(float zoom)
        {
            this.Zoom(zoom - this.zoom, null, zoom);
        }
        
        public void Zoom(float dt, Vector2? to_point = null, float? set_zoom = null)
        {
            float z;
            
            if (set_zoom.HasValue)
            {
                z = set_zoom.Value;
            } else
            {
                z = zoom + (dt > 0 ? this.zoom_scale : -this.zoom_scale);
            }
            
            if (dt > 0)
            {
                //Zooming in
                if (z <= this.max_zoom)
                {
                    //TODO: rce - Zoom to point if applicable
                    this.zoom = z;
                }
            }
            else
            {
                //Zooming out
                if (this.CheckZoomClamp() && z > this.min_zoom)
                {
                    //TODO: rce - Zoom to center of clamp if applicable
                    this.zoom = z;
                }
            }

            this.UpdateTranslationMatrix();
        }

        //Checks if the clamp is visible in the zoom
        private bool CheckZoomClamp()
        {
            if (!this.camera_bounds.HasValue) return true;

            float bounds_w = this.camera_bounds.Value.Width;
            float bounds_h = this.camera_bounds.Value.Height;
            
            return bounds_w >= this.width && bounds_h >= this.height;
        }
        
        public void Resize()
        {
            if (this.is_static)
            {
                //Update 0,0 to be the top left which in turn updates the matrix
                this.top_left = new Vector2(0, 0);
            }
            else
            {
                //Just update matrix
                this.UpdateTranslationMatrix();
            }
        }

        private void UpdateTranslationMatrix()
        {
            //Update the matrix
            this.translation_matrix =
                Matrix.CreateTranslation(-(int)x_position, -(int)y_position, 0) *
                Matrix.CreateRotationZ(0) *
                Matrix.CreateScale(zoom) *
                Matrix.CreateTranslation(new Vector3(viewport_center, 0));
        }
        
        public void SetStatic(bool isStatic)
        {
            this.is_static = isStatic;
        }

        public Rectangle? GetBounds()
        {
            return this.camera_bounds;
        }

        public RectangleF GetViewBounds()
        {
            Vector2 top_left = this.top_left;
            return new RectangleF(top_left.X, top_left.Y, this.width, this.height);
        }

        public void SetMaxZoom(float max_zoom)
        {
            this.max_zoom = max_zoom;
        }

        public void SetMinZoom(float min_zoom)
        {
            this.min_zoom = min_zoom;
        }

        public void MoveToPosition(Vector2 position)
        {
            this.ignore_inputs = true;
            this.start_position = this.center;
            this.move_to_position = position;
        }

        public void Update(double dt)
        {
            if (this.move_to_position != null)
            {
                this.movement_time += (float)dt * 3;
                this.center = this.start_position.Value.Lerp(this.move_to_position.Value, this.movement_time);
                if (this.movement_time >= 1)
                {
                    this.center = this.move_to_position.Value;
                    this.start_position = null;
                    this.movement_time = 0;
                    this.move_to_position = null;
                    this.ignore_inputs = false;
                }
            }
        }
    }
}