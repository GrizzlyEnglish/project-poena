using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common
{
    public class MouseEvent
    {
        public MouseEventArgs MouseEventArgs { get; private set; }

        public Vector2 UnprojectedPosition { get; private set; }

        public MouseEvent(MouseEventArgs mouseEventArgs)
        {
            this.MouseEventArgs = mouseEventArgs;
        }

        public void SetUnprojectedPosition(Camera cam)
        {
            this.UnprojectedPosition = cam.UnProjectCoordinates(MouseEventArgs.Position.ToVector2());
        }

    }
}
