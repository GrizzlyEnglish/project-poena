using System;
using Poena.OpenGL.Input;

namespace Poena.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Core.Poena())
            {
                game.inputHandler.AddGather(new MouseInputGather());
                game.inputHandler.AddGather(new KeyboardInputGather());
                game.Run();
            }
        }
    }
}
