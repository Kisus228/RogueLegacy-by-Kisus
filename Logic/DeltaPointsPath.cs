using System.Drawing;
using RogueLegacy.Creatures;

namespace RogueLegacy.Logic
{
    internal class DeltaPointsPath
    {
        public Point Point { get; }
        public DeltaPointsPath PrevPoint { get; }
        public IMonster Creature { get; }

        public DeltaPointsPath(IMonster creature, Point currentPoint, DeltaPointsPath prevPoint = null)
        {
            Creature = creature;
            Point = currentPoint;
            PrevPoint = prevPoint;
        }
    }
}