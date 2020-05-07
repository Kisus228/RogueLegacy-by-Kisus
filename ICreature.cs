using System.Diagnostics;
using System.Drawing;

namespace RogueLegacy
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
    }
}