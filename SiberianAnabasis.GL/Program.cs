﻿using System;

namespace SiberianAnabasis
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
            {
                Game1.CurrentPlatform = Enums.Platform.GL;
                game.Run();
            }

        }
    }
}
