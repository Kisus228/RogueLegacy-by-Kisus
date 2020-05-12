using System.Diagnostics;
using System.Drawing;

namespace RogueLegacy.Creatures
{
    public interface IMonster : ICreature
    {
        Stopwatch MoveTimer { get; }
        int MoveInterval { get; }
        int Range { get; }
        bool CanAttackFromPoint(Point p);
        void SetLookDirectionToPlayer();
    }
}