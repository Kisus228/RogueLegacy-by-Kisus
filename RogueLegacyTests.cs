using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace RogueLegacy
{
    [TestFixture]
    internal class RogueLegacyTests
    {
        [TestCase("DefaultMap", 16, TestName = "DefaultMap")]
        public void PathFinderLengthTest(string mapName, int expectedLength)
        {
            Game.CreateGame(mapName);
            Assert.IsTrue(PathFinder.GetShortestPath(Game.Enemies).Count() == expectedLength);
        }

        [TestCase("DefaultMap", 10000)]
        [Timeout(100)]
        public void MonstersKillPlayer(string mapName, int monsterAttackInterval)
        {
            Game.CreateGame(mapName);
            foreach (var enemy in Game.Enemies)
            {
                enemy.ChangeAttackInterval(monsterAttackInterval);
            }
            StartGameTillPlayerIsAlive();
            Assert.IsTrue(Game.Player.IsDead, "Игрок остался живым.");
        }

        private static void StartGameTillPlayerIsAlive()
        {
            while (!Game.Player.IsDead)
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
            }
        }
    }
}