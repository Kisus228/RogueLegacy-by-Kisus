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
        public bool IsDead { get; private set; }

        public bool CanAttack =>
            AttackTimer.ElapsedMilliseconds == 0 || AttackTimer.ElapsedMilliseconds >= AttackInterval;

        public bool IsBlocking { get; private set; }
        public Look LookDirection { get; set; }

        public Player(Point startLocation)
        {
            HP = 100;
            Damage = 10;
            Armor = 5;
            Location = startLocation;
            AttackTimer = new Stopwatch();
            AttackInterval = 700;
        }

        public void MakeMove(Point move)
        {
            var newLocation = Location + (Size) move;
            Game.Map[Location.Y, Location.X] = State.Empty;
            Location = newLocation;
            Game.Map[Location.Y, Location.X] = State.Player;
        }

        public void Attack()
        {
            AttackTimer.Restart();
            var enemy = Game.Enemies.FirstOrDefault(x => x.Location == Location +
                                                         (Size) (LookDirection == Look.Right
                                                             ? new Point(1, 0)
                                                             : new Point(-1, 0)));
            enemy?.GetDamage(Damage);
        }

        public void GetDamage(int damage)
        {
            HP -= (int) Math.Ceiling((1 - Armor / 100d) * damage);
            if (HP > 0) return;
            Game.Map[Location.Y, Location.X] = State.Empty;
            IsDead = true;
        }

        public void ChangeAttackInterval(int newValue)
        {
            AttackInterval = newValue;
        }

        public bool CanMove(Point move)
        {
            var newLocation = Location + (Size) move;
            return Game.InBounds(Location + (Size) move) && Game.Map[newLocation.Y, newLocation.X] == State.Empty;
        }

        public string GetName()
        {
            return "player";
        }

        public void SwitchBlocking(bool value)
        {
            IsBlocking = value;
        }
    }
}