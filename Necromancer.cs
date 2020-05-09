using System;
using System.Diagnostics;
using System.Drawing;

namespace RogueLegacy
{
    internal class Necromancer : IMonster
    {
        public int HP { get; private set; }
        public int Damage { get; }
        public int Armor { get; }
        public Point Location { get; private set; }
        public Stopwatch AttackTimer { get; }
        public bool IsDead { get; private set; }

        public bool CanAttack =>
            AttackTimer.ElapsedMilliseconds == 0 || AttackTimer.ElapsedMilliseconds >= AttackInterval;
        public Look LookDirection { get; private set; }

        public Stopwatch MoveTimer { get; }
        public int MoveInterval { get; }
        public int Range { get; }
        private int AttackInterval { get; set; }

        public Necromancer(Point startLocation)
        {
            AttackTimer = new Stopwatch();
            HP = 150;
            Damage = 0;
            Armor = 0;
            Location = startLocation;
            LookDirection = Look.Right;
            AttackInterval = 13000;
            MoveTimer = new Stopwatch();
            MoveInterval = 3000;
            Range = 0;
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
            SpawnSkeleton();
        }

        private void SpawnSkeleton()
        {
            var skeleton = new Skeleton(Location + (Size) GetNewRandomPoint());
            Game.Map[skeleton.Location.Y, skeleton.Location.X] = State.Enemy;
            Game.Enemies.Add(skeleton);
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
            return "necromancer";
        }

        public bool CanAttackFromPoint(Point p)
        {
            return true;
        }

        public void SetLookDirectionToPlayer()
        {
            var relativeX = Game.Player.Location.X - Location.X;
            if (relativeX > 0)
                LookDirection = Look.Right;
            else if (relativeX < 0)
                LookDirection = Look.Left;
        }

        public Point GetNewRandomPoint()
        {
            var rnd = new Random();
            var newPoint = new Point(-1 + rnd.Next(3), -1 + rnd.Next(3));
            while (!Game.InBounds(Location + (Size) newPoint) && Game.Map[newPoint.Y, newPoint.X] == State.Empty)
                newPoint = new Point(-1 + rnd.Next(3), -1 + rnd.Next(3));
            return newPoint;
        }
    }
}