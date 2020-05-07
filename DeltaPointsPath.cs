using System.Drawing;

namespace RogueLegacy
{
    internal class DeltaPointsPath
    {
        public Point Point { get; }
        public DeltaPointsPath PrevPoint { get; }
        public ICreature Creature { get; }

        public DeltaPointsPath(ICreature creature, Point currentPoint, DeltaPointsPath prevPoint = null)
        {
            Creature = creature;
            Point = currentPoint;
            PrevPoint = prevPoint;
        }
    }
}