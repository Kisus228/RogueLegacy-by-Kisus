using System.Diagnostics;
using System.Drawing;
using RogueLegacy.Logic;

namespace RogueLegacy.Creatures
{
    public interface ICreature
    {
        int HP { get; }
        int Damage { get; }
        int Armor { get; }
        Point Location { get; }
        Stopwatch AttackTimer { get; }
        bool IsDead { get; }
        bool CanAttack { get; }
        Look LookDirection { get; }

        void MakeMove(Point move);
        void Attack();
        void GetDamage(int damage);
        void ChangeAttackInterval(int newValue);
        bool CanMove(Point move);
    }
}