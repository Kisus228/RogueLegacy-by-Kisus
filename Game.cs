using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RogueLegacy
{
    public static class Game
    {
        public const int ElementSize = 32;
        public static State[,] Map;
        public static Queue<Movement> MovementQueue;

        public static readonly Dictionary<State, Brush> StateToColor = new Dictionary<State, Brush>
        {
            {State.Empty, Brushes.LightSlateGray},
            {State.UnderAttack, Brushes.Orange},
            {State.Attacked, Brushes.Red},
        };

        public static readonly Dictionary<string, Bitmap> CreatureToPicture = new Dictionary<string, Bitmap>();

        public static Player Player { get; private set; }
        public static List<IMonster> Enemies { get; private set; }

        private static int MapHeight => Map.GetLength(0);
        private static int MapWidth => Map.GetLength(1);

        public static bool InBounds(Point p)
        {
            return p.X >= 0 && p.X < MapWidth && p.Y >= 0 && p.Y < MapHeight;
        }


        public static void CreateGame(string mapName)
        {
            InitializeCreaturesSprites();
            Enemies = new List<IMonster>();
            Map = InitializeMap(mapName);
            MovementQueue = new Queue<Movement>();
            UpdateMovements();
        }

        private static void InitializeCreaturesSprites()
        {
            CreatureToPicture.Add("player", new Bitmap(Path.Combine(RogueLegacyWindow.ProjectPath, "assets", "player.bmp")));
            CreatureToPicture.Add("guardian",
                new Bitmap(Path.Combine(RogueLegacyWindow.ProjectPath, "assets", "guardian.bmp")));
            CreatureToPicture.Add("skeleton",
                new Bitmap(Path.Combine(RogueLegacyWindow.ProjectPath, "assets", "skeleton.bmp")));
            CreatureToPicture.Add("necromancer",
                new Bitmap(Path.Combine(RogueLegacyWindow.ProjectPath, "assets", "necromancer.bmp")));
        }

        public static void UpdateMovements()
        {
            foreach (var move in PathFinder.GetShortestPath(Enemies.Where(x =>
                    {
                        var distance = Player.Location - (Size) x.Location;
                        return Math.Abs(distance.X) <= x.Range && Math.Abs(distance.Y) <= x.Range;
                    })
                    .ToList())
                .Where(x => x.Creature.CanMove(x.DeltaPoint)))
                MovementQueue.Enqueue(move);
            foreach (var monster in Enemies.Where(x => x is Necromancer))
            {
                var necromancer = (Necromancer) monster;
                MovementQueue.Enqueue(new Movement(necromancer, necromancer.GetNewRandomPoint()));
            }
        }

        private static State[,] InitializeMap(string mapName)
        {
            var level = GetMapsFromText()
                .First(name => name.StartsWith(mapName))
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .ToArray();
            var result = new State[level.Length, level[0].Length];
            for (var row = 0; row < result.GetLength(0); row++)
            for (var column = 0; column < result.GetLength(1); column++)
                switch (level[row][column])
                {
                    case 'G':
                        result[row, column] = State.Enemy;
                        Enemies.Add(new Guardian(new Point(column, row)));
                        break;
                    case 'P':
                        result[row, column] = State.Player;
                        Player = new Player(new Point(column, row));
                        break;
                    case 'N':
                        result[row, column] = State.Enemy;
                        Enemies.Add(new Necromancer(new Point(column, row)));
                        break;
                    default:
                        result[row, column] = State.Empty;
                        break;
                }

            return result;
        }

        private static IEnumerable<string> GetMapsFromText()
        {
            var exeFile = AppDomain.CurrentDomain.BaseDirectory;
            return File.ReadAllText(Path.Combine(exeFile, @"Maps.txt")).Split('|');
        }
    }
}