using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RogueLegacy
{
    public static class SubscribeProvider
    {
        public static void InitializeForm(RogueLegacyWindow form)
        {
            //InitializeMediaPlayer(form);
            var timer = new Timer {Interval = 15};
            timer.Tick += (sender, args) =>
            {
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
                form.Invalidate();
            };
            timer.Start();
        }

        private static void InitializeMediaPlayer(Form form)
        {
            SoundPlayer sp = null;
            form.Shown += (sender, args) =>
            {
                sp = new SoundPlayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"backsound.wav"));
                sp.PlayLooping();
            };
            form.Closing += (sender, args) => sp.Dispose();
        }
    }
}
