using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace RogueLegacy
{
    [TestFixture]
    internal class RogueLegacyTests
    {

        [TestCase("NormalMap", 1)]
        [TestCase("HardMap", 2)]
        public void DoesNecromancerSpawnSkeleton(string mapName, int expectedDifference)
        {
            Game.CreateGame(mapName);
            var enemiesCountWhenCreated = Game.Enemies.Count;
            MakeTurn();
            Assert.IsTrue(Game.Enemies.Count - enemiesCountWhenCreated == expectedDifference,
                $"Некромансер не заспавнил скелетов. Стало: {Game.Enemies.Count}. Должно было быть: {enemiesCountWhenCreated + expectedDifference}");
        }

        [TestCase("DefaultMap", 4)]
        [TestCase("NormalMap", 14)]
        [TestCase("HardMap", 0)]
        public void PathFinderLengthTest(string mapName, int expectedLength)
        {
            Game.CreateGame(mapName);
            var actualLength = PathFinder.GetShortestPath(new List<IMonster> {Game.Enemies.First()}).Count();
            Assert.IsTrue(actualLength == expectedLength, $"Получившийся результат: {actualLength}. Ожидаемый результат: {expectedLength}");
        }

        [TestCase("DefaultMap", 10)]
        [Timeout(100)]
        public void MonstersKillPlayerAtSpawn(string mapName, int monsterAttackInterval)
        {
            Game.CreateGame(mapName);
            foreach (var enemy in Game.Enemies)
            {
                enemy.ChangeAttackInterval(monsterAttackInterval);
            }
            StartGameTillPlayerIsAlive();
            Assert.IsTrue(Game.Player.IsDead, "Игрок остался живым.");
        }

        [TestCase("NormalMap", 15, 150)]
        [TestCase("HardMap", 15, 150)]
        [Timeout(4000)]
        public void MonstersKillNotPlayerAtSpawn(string mapName, int monsterAttackInterval, int timeout)
        {
            Game.CreateGame(mapName);
            foreach (var enemy in Game.Enemies)
            {
                enemy.ChangeAttackInterval(monsterAttackInterval);
            }

            var sw = new Stopwatch();
            while(sw.ElapsedMilliseconds < timeout)
            {
                sw.Start();
                MakeTurn();
                sw.Stop();
            }
            Assert.IsFalse(Game.Player.IsDead, "Игрок не должен был умереть.");
        }

        private static void StartGameTillPlayerIsAlive()
        {
            while (!Game.Player.IsDead)
                MakeTurn();
        }

        private static void MakeTurn()
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
            if (Game.QueueToAddEnemy.Count != 0)
            {
                foreach (var summonedMonster in Game.QueueToAddEnemy)
                {
                    Game.Enemies.Add(summonedMonster);
                    Game.Map[summonedMonster.Location.Y, summonedMonster.Location.X] = State.Enemy;
                }
            }
        }
    }
}