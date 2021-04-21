using System;
using System.Collections.Generic;
using System.Text;

namespace Poena.Core.Common.Interfaces
{
    public interface ISaveable
    {
        void Load(string path);
        void Save(string path);
    }
}
