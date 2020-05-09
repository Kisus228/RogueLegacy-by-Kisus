using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLegacy
{
    class Skeleton : IMonster
    {
        public int HP { get; private set; }
        public int Damage { get; }
        public int Armor { get; }
        public Point Location { get; private set; }
        public Stopwatch AttackTimer { get; }
        public bool IsDead { get; private set; }
        public bool CanAttack => (AttackTimer.ElapsedMilliseconds == 0 || AttackTimer.ElapsedMilliseconds >= AttackInterval)
                                 && CanAttackFromPoint(Location);
        public Look LookDirection { get; private set; }
        private int AttackInterval { get; set; }

        public Skeleton(Point startLocation)
        {
            AttackTimer = new Stopwatch();
            HP = 30;
            Damage = 13;
            Armor = 0;
            Location = startLocation;
            LookDirection = Look.Right;
            AttackInterval = 400;
            MoveTimer = new Stopwatch();
            MoveInterval = 700;
            Range = 2;
        }
        public void MakeMove(Point move)
        {
            SetLookDirectionToPlayer();
            MoveTimer.Restart();
            var newLocation = Location + (Size)move;
            Game.Map[Location.Y, Location.X] = State.Empty;
            Location = newLocation;
            Game.Map[Location.Y, Location.X] = State.Enemy;
        }

        public void Attack()
        {
            SetLookDirectionToPlayer();
            AttackTimer.Restart();
            if (Game.Player.IsBlocking && Game.Player.LookDirection != LookDirection) return;
            Game.Player.GetDamage(Damage);
        }

        public void GetDamage(int damage)
        {
            HP -= (int)Math.Ceiling((1 - Armor / 100d) * damage);
            if (HP > 0) return;
            IsDead = true;
            Game.Enemies.Remove(this);
            Game.Map[Location.Y, Location.X] = State.Empty;
        }

        public void ChangeAttackInterval(int newValue)
        {
            AttackInterval = newValue;
        }

        public bool CanMove(Point move)
        {
            var newLocation = Location + (Size)move;
            return (MoveTimer.ElapsedMilliseconds == 0 || MoveTimer.ElapsedMilliseconds >= MoveInterval)
                   && Game.InBounds(Location + (Size)move) && Game.Map[newLocation.Y, newLocation.X] == State.Empty;
        }

        public string GetName()
        {
            return "skeleton";
        }

        public Stopwatch MoveTimer { get; }
        public int MoveInterval { get; }
        public int Range { get; }
        public bool CanAttackFromPoint(Point p)
        {
            return Game.Player.Location.Y == p.Y
                   && Math.Abs(Game.Player.Location.X - p.X) == 1;
        }

        public void SetLookDirectionToPlayer()
        {
            var relativeX = Game.Player.Location.X - Location.X;
            if (relativeX > 0)
                LookDirection = Look.Right;
            else if (relativeX < 0)
                LookDirection = Look.Left;
        }
    }
}
