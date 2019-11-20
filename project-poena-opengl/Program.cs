using System;
using Project_Poena;

namespace Project_Poena_opengl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Poena())
                game.Run();
        }
    }
}
