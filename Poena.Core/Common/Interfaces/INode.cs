using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface INode : IRenderable, ISaveable
    {
        void Initialize();
        void Entry();
        void Exit();
        void Destroy();
    }
}
