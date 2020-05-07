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
using System.Runtime.CompilerServices;

namespace RogueLegacy
{
    public sealed class RogueLegacyWindow : Form
    {
        private readonly Timer timer = new Timer{Interval = 15};
        public RogueLegacyWindow()
        {
            DoubleBuffered = true;
            ClientSize = new Size(Game.Map.GetLength(0) * Game.ElementSize, Game.Map.GetLength(1) * Game.ElementSize);
            MaximizeBox = false;
            StartTimer();
            //InitializeMediaPlayer();
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
            if (Game.Player.IsBlocking) return;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, -1)));
                    break;
                case Keys.Down:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, 1)));
                    break;
                case Keys.Left:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(-1, 0)));
                    break;
                case Keys.Right:
                    Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(1, 0)));
                    break;
                case Keys.D:
                    if (Game.Player.CanAttack)
                        Game.Player.Attack();
                    break;
                case Keys.Space:
                    Game.Player.SwitchBlocking(true);
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    Game.Player.SwitchBlocking(false);
                    break;
            }
        }

        private void StartTimer()
        {
            timer.Tick += (sender, args) =>
            {
                if (Game.Player.IsDead) return;
                if (Game.MovementQueue.Count == 0)
                    Game.UpdateMovements();
                if (Game.MovementQueue.Count != 0)
                {
                    var movement = Game.MovementQueue.Dequeue();
                    var creature = movement.Creature;
                    creature.MakeMove(movement.DeltaPoint);
                }

                foreach (var creature in Game.Enemies.Where(creature => creature.CanAttack))
                    creature.Attack();
                Invalidate();
            };
            timer.Start();
        }

        private void InitializeMediaPlayer()
        {
            SoundPlayer sp = null;
            Shown += (sender, args) =>
            {
                sp = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"backsound.wav"));
                sp.PlayLooping();
            };
            Closing += (sender, args) => sp.Dispose();
        }
    }
}
