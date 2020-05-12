using System.Drawing;
using RogueLegacy.Creatures;

namespace RogueLegacy.Logic
{
    public class Movement
    {
        public ICreature Creature { get; }
        public Point DeltaPoint { get; }

        public Movement(ICreature creature, Point deltaPoint)
        {
            Creature = creature;
            DeltaPoint = deltaPoint;
        }
    }
}