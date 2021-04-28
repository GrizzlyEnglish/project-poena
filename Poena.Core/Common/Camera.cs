using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Poena.Core.Extensions;

namespace Poena.Core.Common
{
    public class Camera
    {
        //The center of the camera position
        private float X;
        private float Y;
        
        private float TranslationDifference;

        private Rectangle? Bounds;

        private readonly float _ZoomScale = 0.03f;
        private float _Zoom;
        private float _MaxZoom;
        private float _MinZoom;
        private bool IsStatic = false;

        private Vector2? move_to_position;
        private Vector2? start_position;
        private float movement_time = 0;

        public Matrix translation_matrix { get; set; }

        public Camera(float x = 0, float y = 0,
            bool is_static = false, float translation_difference = 10, float max_zoom = 1f, float min_zoom = 0.01f, float current_zoom = 0.03f)
        {
            this.X = x;
            this.Y = y;
            this.IsStatic = is_static;
            this.TranslationDifference = translation_difference;
            this._MaxZoom = max_zoom;
            this._MinZoom = min_zoom;
            this._Zoom = current_zoom;
            this.Resize();
        }
        
        public void ClampCamera(Rectangle bounds, bool centerOnBounds = false)
        {
            this.Bounds = bounds;
            
            if (centerOnBounds)
            {
                this.SetPosition(bounds.Center.ToVector2());
            }
        }

        private Vector2 viewport_center
        {
            get
            {
                return new Vector2(Config.VIEWPORT_WIDTH * .5f, Config.VIEWPORT_HEIGHT * .5f);
            }
        }

        public Vector2 top_left
        {
            get
            {
                float cx = this.X - (this.width / 2f);
                float cy = this.Y - (this.height / 2f);

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
                return Config.VIEWPORT_WIDTH / this._Zoom;
            }
        }

        public float height
        {
            //TODO: rce - determine if we need to set the height
            get
            {
                return Config.VIEWPORT_HEIGHT / this._Zoom;
            }
        }

        public Vector2 center
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.UpdateTranslationMatrix();
            }
        }

        public Vector2 UnProjectCoordinates(Vector2 coords, float? scale = null)
        {
            float zoom = scale ?? this._Zoom;

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
            if (!IsStatic)
            {
                //TODO: Vefify bounds
                this.X = x;
                this.Y = y;
                this.UpdateTranslationMatrix();
            }
        }

        public void Translate(Vector2 trans)
        {
            this.Translate(trans.X, trans.Y);
        }

        public void Translate(float x = 0, float y = 0, bool adjustForScale = true)
        {
            if (!IsStatic)
            {
                float x_diff = x;
                float y_diff = y;
                if (adjustForScale)
                {
                    x_diff /= this._Zoom;
                    y_diff /= this._Zoom;
                }
                this.X += x_diff;
                this.Y += y_diff;
                this.CheckTranslationClamp(x_diff, y_diff);
                this.UpdateTranslationMatrix();
            }
        }

        private void CheckTranslationClamp(float x_diff, float y_diff)
        {
            //Nothing to check return
            if (!this.Bounds.HasValue) return;

            Rectangle rect = this.Bounds.Value;
            
            //This gives the furthest x and y values
            Point top_left = new Point(rect.Left, rect.Top);
            Point bottom_right = new Point(rect.Right, rect.Bottom);
            
            //Check the horizontal position
            if (this.top_left.X < top_left.X || (this.top_left.X + this.width) > bottom_right.X)
            {
                //Remove the recently added difference
                this.X -= x_diff;
            }

            //Chec the vertical position
            if (this.top_left.Y < top_left.Y || (this.top_left.Y + this.height) > bottom_right.Y)
            {
                //Remove the recently added difference
                this.Y -= y_diff;
            }
        }

        public void SetZoom(float zoom)
        {
            this.Zoom(zoom - this._Zoom, null, zoom);
        }
        
        public void Zoom(float dt, Vector2? to_point = null, float? set_zoom = null)
        {
            float z;
            
            if (set_zoom.HasValue)
            {
                z = set_zoom.Value;
            }
            else
            {
                z = this._Zoom + (dt > 0 ? this._ZoomScale : -this._ZoomScale);
            }
            
            if (dt > 0)
            {
                //Zooming in
                if (z <= this._MaxZoom)
                {
                    //TODO: rce - Zoom to point if applicable
                    this._Zoom = z;
                }
                else
                {
                    this._Zoom = this._MaxZoom;
                }
            }
            else
            {
                //Zooming out
                if (this.CheckZoomClamp() && z > this._MinZoom)
                {
                    //TODO: rce - Zoom to center of clamp if applicable
                    this._Zoom = z;
                }
            }

            this.UpdateTranslationMatrix();
        }

        //Checks if the clamp is visible in the zoom
        private bool CheckZoomClamp()
        {
            if (!this.Bounds.HasValue) return true;

            float bounds_w = this.Bounds.Value.Width;
            float bounds_h = this.Bounds.Value.Height;
            
            return bounds_w >= this.width && bounds_h >= this.height;
        }
        
        public void Resize()
        {
            if (this.IsStatic)
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
                Matrix.CreateTranslation(-(int)X, -(int)Y, 0) *
                Matrix.CreateRotationZ(0) *
                Matrix.CreateScale(_Zoom) *
                Matrix.CreateTranslation(new Vector3(viewport_center, 0));
        }
        
        public void SetStatic(bool isStatic)
        {
            this.IsStatic = isStatic;
        }

        public Rectangle? GetBounds()
        {
            return this.Bounds;
        }

        public RectangleF GetViewBounds()
        {
            Vector2 topLeft = this.top_left;
            return new RectangleF(top_left.X, top_left.Y, this.width, this.height);
        }

        public void SetMaxZoom(float max_zoom)
        {
            this._MaxZoom = max_zoom;
        }

        public void SetMinZoom(float min_zoom)
        {
            this._MinZoom = min_zoom;
        }

        public void MoveToPosition(Vector2 position)
        {
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
                }
            }
        }
    }
}