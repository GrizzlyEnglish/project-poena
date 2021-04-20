using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface IInputable 
    {
        bool HandleMouseClicked(MouseEvent mouseEvent);
        void HandleMouseMoved(MouseEvent mouseEvent);
    }
}
