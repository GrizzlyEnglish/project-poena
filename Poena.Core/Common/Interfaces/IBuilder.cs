using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface IBuilder<T>
    {
        T Build();
    }
}
