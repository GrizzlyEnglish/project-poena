using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface IRouteable
    {
        int GetMovementCost(IRouteable mover = null);
    }
}
