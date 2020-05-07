﻿using System;
using System.Diagnostics;
using System.Drawing;

namespace RogueLegacy
{
    internal class Guardian : ICreature, IMonster
    {
        private int AttackInterval { get; set; }
        public Stopwatch AttackTimer { get; }

        public int HP { get; private set; }
        public int Damage { get; }
        public int Armor { get; }
        public Point Location { get; private set; }
        public bool IsDead { get; private set; }

        public bool CanAttack =>
            (AttackTimer.ElapsedMilliseconds == 0 || AttackTimer.ElapsedMilliseconds >= AttackInterval)
            && CanAttackFromPoint(Location);

        public Look LookDirection { get; private set; }

        public Guardian(Point startLocation)
        {
            AttackTimer = new Stopwatch();
            HP = 40;
            Damage = 10;
            Armor = 20;
            Location = startLocation;
            LookDirection = Look.Right;
            AttackInterval = 700;
        }

        public bool CanAttackFromPoint(Point p)
        {
            return Game.Player.Location.Y == p.Y
                   && Math.Abs(Game.Player.Location.X - p.X) == 1;
        }

        public void MakeMove(Point move)
        {
            var relativeX = Game.Player.Location.X -  Location.X;
            if (relativeX > 0)
                LookDirection = Look.Right;
            else if (relativeX < 0)
                LookDirection = Look.Left;
            var newLocation = Location + (Size) move;
            if (!Game.InBounds(Location + (Size)move) || Game.Map[newLocation.Y, newLocation.X] != State.Empty) return;
            Game.Map[Location.Y, Location.X] = State.Empty;
            Location = newLocation;
            Game.Map[Location.Y, Location.X] = State.Enemy;
        }

        public void Attack()
        {
            AttackTimer.Restart();
            if (Game.Player.IsBlocking && Game.Player.LookDirection != LookDirection) return;
            Game.Player.GetDamage(Damage);
        }

        public void GetDamage(int damage)
        {
            HP -= (int) Math.Ceiling((1 - Armor / 100d) * damage);
            if (HP > 0) return;
            IsDead = true;
            Game.Enemies.Remove(this);
            Game.Map[Location.Y, Location.X] = State.Empty;
        }

        public void ChangeAttackInterval(int newValue)
        {
            AttackInterval = newValue;
        }
    }
}