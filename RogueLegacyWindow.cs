using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace RogueLegacy
{
    public sealed class RogueLegacyWindow : Form
    {
        public RogueLegacyWindow()
        {
            DoubleBuffered = true;
            ClientSize = new Size(Game.Map.GetLength(0) * Game.ElementSize, Game.Map.GetLength(1) * Game.ElementSize);
            MaximizeBox = false;
            SubscribeProvider.InitializeForm(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            for (var y = 0; y < Game.Map.GetLength(0); y++)
            {
                for (var x = 0; x < Game.Map.GetLength(1); x++)
                {
                    g.FillRectangle(Game.StateToColor[Game.Map[y, x]], x * Game.ElementSize, y * Game.ElementSize,
                        Game.ElementSize, Game.ElementSize);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, -1)));
                    break;
                case Keys.S:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, 1)));
                    break;
                case Keys.A:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(-1, 0)));
                    break;
                case Keys.D:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(1, 0)));
                    break;
            }
        }
    }
}
