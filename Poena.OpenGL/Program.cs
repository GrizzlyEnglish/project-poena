using System;

namespace Poena.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Core.Poena())
            {
                game.Run();
            }
        }
    }
}
