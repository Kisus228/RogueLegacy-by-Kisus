﻿using System;
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
        public static Player Player { get; private set; }
        public static List<ICreature> Enemies { get; private set; }

        public static readonly Dictionary<State, Brush> StateToColor = new Dictionary<State, Brush>
        {
            {State.Empty, Brushes.Black},
            {State.Player, Brushes.Green},
            {State.Enemy, Brushes.DarkOrchid},
            {State.UnderAttack, Brushes.Orange},
            {State.Attacked, Brushes.Red}
        };

        private static int MapHeight => Map.GetLength(0);
        private static int MapWidth => Map.GetLength(1);

        public static bool InBounds(Point p)
        {
            return p.X >= 0 && p.X < MapWidth && p.Y >= 0 && p.Y < MapHeight;
        }


        public static void CreateGame(string mapName)
        {
            Enemies = new List<ICreature>();
            Map = InitializeMap(mapName);
            MovementQueue = new Queue<Movement>();
            UpdateMovements();
        }

        public static void UpdateMovements()
        {
            foreach (var move in PathFinder.GetShortestPath(Enemies)) MovementQueue.Enqueue(move);
        }

        private static State[,] InitializeMap(string mapName)
        {
            var level = GetMapsFromText()
                .First(name => name.StartsWith(mapName))
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
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