using System;
using Project_Poena;
using Project_Poena.OpenGL.Input;

namespace Project_Poena_opengl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Poena())
            {
                game.inputHandler.AddGather(new MouseInputGather());
                game.inputHandler.AddGather(new KeyboardInputGather());
                game.Run();
            }
        }
    }
}
