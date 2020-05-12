using System;
using System.Windows.Forms;
using RogueLegacy.Logic;

namespace RogueLegacy
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Game.CreateGame("DefaultMap");
            Application.Run(new RogueLegacyWindow());
        }
    }
}