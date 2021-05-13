using System;
using System.Diagnostics;
using Poena.Core.Common;
using Poena.Core.Common.Enums;

namespace Poena.Core.Utilities
{
    public static class Logger {

        public static void Log(string nameSpace, string entry) 
        {
            Debug.WriteLine($"{nameSpace} -- {DateTime.Now.ToString("hh:mm:ss")} -- {entry}");
        }
    }

}