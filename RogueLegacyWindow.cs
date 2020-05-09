﻿using System;
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
using System.Runtime.Remoting.Channels;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace RogueLegacy
{
    public sealed class RogueLegacyWindow : Form
    {
        private Label pauseLabel;
        private ProgressBar progressBar;
        private SoundPlayer sp;
        private bool isSoundPlayerRunning;
        private readonly Timer timer = new Timer{Interval = 15};
        private bool IsPausing { get; set; }
        private static readonly string ExeFilePath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string ProjectPath = Directory.GetParent(ExeFilePath).Parent.Parent.FullName;
        public RogueLegacyWindow()
        {
            DoubleBuffered = true;
            ClientSize = new Size(Game.Map.GetLength(1) * Game.ElementSize, (Game.Map.GetLength(0) + 2) * Game.ElementSize);
            MaximizeBox = false;
            InitializeComponents();
            StartTimer();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TranslateTransform(0, Game.ElementSize);
            g.FillRectangle(Brushes.LightSlateGray, 0, 0, Game.MapWidth * Game.ElementSize,
                Game.MapHeight * Game.ElementSize);
            for (var y = 0; y < Game.MapHeight; y++)
            {
                for (var x = 0; x < Game.MapWidth; x++)
                {
                    if (Game.Map[y, x] == State.Player || Game.Map[y, x] == State.Enemy)
                    {
                        var creature = Game.Player.Location == new Point(x, y)
                            ? Game.Player
                            : (ICreature) Game.Enemies.FirstOrDefault(enemy => enemy.Location == new Point(x, y));
                        var pic = (Bitmap)Game.CreatureToPicture[creature.GetName()].Clone();
                        if (creature.LookDirection == Look.Left)
                            pic.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        g.DrawImage(pic, x * Game.ElementSize, y * Game.ElementSize);
                    }
                    else if (Game.Map[y, x] != State.Empty)
                        g.FillRectangle(Game.StateToColor[Game.Map[y, x]], x * Game.ElementSize, y * Game.ElementSize,
                        Game.ElementSize, Game.ElementSize);
                }
            }
            g.ResetTransform();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Game.Player.IsBlocking && !IsPausing)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, -1)));
                        break;
                    case Keys.Down:
                        Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(0, 1)));
                        break;
                    case Keys.Left:
                        Game.Player.LookDirection = Look.Left;
                        Game.MovementQueue.Enqueue(new Movement(Game.Player, new Point(-1, 0)));
                        break;
                    case Keys.Right:
                        Game.Player.LookDirection = Look.Right;
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

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    IsPausing = !IsPausing;
                    pauseLabel.Visible = !pauseLabel.Visible;
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
                if (Game.Player.IsDead || Game.Enemies.All(x => x.IsDead) || IsPausing) return;
                if (Game.MovementQueue.Count == 0)
                    Game.UpdateMovements();
                if (Game.MovementQueue.Count != 0)
                {
                    var movement = Game.MovementQueue.Dequeue();
                    var creature = movement.Creature;
                    if (creature.CanMove(movement.DeltaPoint))
                        creature.MakeMove(movement.DeltaPoint);
                }

                try
                {
                    foreach (var creature in Game.Enemies.Where(creature => creature.CanAttack))
                        creature.Attack();
                }
                catch
                {
                    // ignored
                }

                progressBar.Value = progressBar.Maximum - Game.Enemies.Where(x => !(x is Skeleton)).Sum(x => x.HP);
                Invalidate();
            };
            timer.Start();
        }

        private void InitializeMediaPlayer()
        {
            Shown += (sender, args) =>
            {
                sp = new SoundPlayer(Path.Combine(ProjectPath, @"backsound.wav"));
                sp.PlayLooping();
                isSoundPlayerRunning = true;
            };
            Closing += (sender, args) => sp.Dispose();
        }

        private void InitializeComponents()
        {
            InitializePauseLabel();
            InitializeMenuStrip();
            InitializeProgressBar();
            InitializeMediaPlayer();
            PerformLayout();
        }

        private void InitializeProgressBar()
        {
            progressBar = new ProgressBar
            {
                Location = new Point(0, (Game.Map.GetLength(0) + 1) * Game.ElementSize),
                Height = Game.ElementSize,
                Dock = DockStyle.Bottom,
                Maximum = Game.Enemies.Sum(enemy => enemy.HP),
                ForeColor = Color.Red
            };
            Controls.Add(progressBar);
        }

        private void InitializeMenuStrip()
        {
            var menuStrip = new MenuStrip
                {Location = Point.Empty, Size = new Size(Game.Map.GetLength(1) * Game.ElementSize, Game.ElementSize)};
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            var levelChooser = new ToolStripMenuItem("Выбрать уровень");
            foreach (var level in Game.Levels)
            {
                levelChooser.DropDownItems.Add(level
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .First());
            }
            levelChooser.DropDownItemClicked += (sender, args) =>
            {
                timer.Stop();
                Game.LoadLevel(args.ClickedItem.Text);
                ClientSize = new Size(Game.Map.GetLength(1) * Game.ElementSize,
                    (Game.Map.GetLength(0) + 2) * Game.ElementSize);
                progressBar.Maximum = Game.Enemies.Sum(x => x.HP);
                progressBar.Value = 0;
                timer.Start();
            };

                menuStrip.Items.Add(levelChooser);

            var musicMuter = new ToolStripMenuItem("Вкл/Выкл музыку");
            musicMuter.Click += (sender, args) =>
            {
                if (isSoundPlayerRunning)
                    sp.Stop();
                else
                    sp.PlayLooping();
                isSoundPlayerRunning = !isSoundPlayerRunning;
            };

            menuStrip.Items.Add(musicMuter);
        }

        private void InitializePauseLabel()
        {
            pauseLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point((Game.Map.GetLength(1) / 2 - 1) * Game.ElementSize, (Game.Map.GetLength(0) / 2) * Game.ElementSize),
                TextAlign = ContentAlignment.MiddleCenter,
                Name = "pauseLabel",
                Size = new System.Drawing.Size(Game.Map.GetLength(1) * Game.ElementSize, Game.ElementSize * 4),
                Text = "PAUSE",
                Visible = false
            };
            Controls.Add(pauseLabel);
        }
    }
}
