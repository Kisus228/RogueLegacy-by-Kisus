using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace RogueLegacy
{
    public class Player : ICreature
    {
        private int AttackInterval { get; set; }

        public int HP { get; private set; }
        public int Damage { get; }
        public int Armor { get; }
        public Point Location { get; private set; }
        public Stopwatch AttackTimer { get; }
        public bool IsDead => HP <= 0;

        public bool CanAttack => true;
        private bool IsBlocking { get; set; }
        public Look LookDirection { get; private set; }

        public Player(Point startLocation)
        {
            HP = 100;
            Damage = 10;
            Armor = 5;
            Location = startLocation;
            AttackTimer = new Stopwatch();
        }

        public bool CanAttackFromPoint(Point p)
        {
            return true;
        }

        public void MakeMove(Point move)
        {
            var newLocation = Location + (Size) move;
            if (!Game.InBounds(Location + (Size)move) || Game.Map[newLocation.Y, newLocation.X] != State.Empty) return;
            Game.Map[Location.Y, Location.X] = State.Empty;
            Location = newLocation;
            Game.Map[Location.Y, Location.X] = State.Player;
            if (move == new Point(1, 0))
                LookDirection = Look.Right;
            else if (move == new Point(-1, 0))
                LookDirection = Look.Left;
        }

        public void Attack()
        {
            if (AttackTimer.ElapsedMilliseconds != 0 && AttackTimer.ElapsedMilliseconds < AttackInterval) return;
            AttackTimer.Restart();
            var enemy = Game.Enemies.FirstOrDefault(x => x.Location == Location +
                                                         (Size) (LookDirection == Look.Right
                                                             ? new Point(1, 0)
                                                             : new Point(-1, 0)));
            enemy?.GetDamage(Damage);
        }

        public void GetDamage(int damage)
        {
            if (IsBlocking)
                return;
            HP -= (int) Math.Ceiling((1 - Armor / 100d) * damage);
        }

        public void ChangeAttackInterval(int newValue)
        {
            AttackInterval = newValue;
        }
    }
}