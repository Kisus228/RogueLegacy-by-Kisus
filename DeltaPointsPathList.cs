using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RogueLegacy
{
    internal class DeltaPointsPathList : IEnumerable<Movement>
    {
        private readonly List<DeltaPointsPath> endPoints;
        public int Count => endPoints.Count;

        public DeltaPointsPathList()
        {
            endPoints = new List<DeltaPointsPath>();
        }

        public IEnumerator<Movement> GetEnumerator()
        {
            var hashSet = new HashSet<DeltaPointsPath>();
            var stack = new Stack<Movement>();
            foreach (var p in endPoints.Where(p => p.PrevPoint != null)) hashSet.Add(p);

            while (hashSet.Count != 0)
            {
                var pointsToAdd = new HashSet<DeltaPointsPath>();
                var pointsToDelete = new HashSet<DeltaPointsPath>();
                foreach (var deltaPath in hashSet)
                {
                    stack.Push(new Movement(deltaPath.Creature, deltaPath.Point - (Size) deltaPath.PrevPoint.Point));
                    if (deltaPath.PrevPoint.PrevPoint != null)
                        pointsToAdd.Add(deltaPath.PrevPoint);
                    pointsToDelete.Add(deltaPath);
                }

                foreach (var point in pointsToAdd)
                    hashSet.Add(point);

                foreach (var point in pointsToDelete)
                    hashSet.Remove(point);
            }

            foreach (var move in stack.Reverse()) yield return move;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(DeltaPointsPath p)
        {
            endPoints.Add(p);
        }

        public bool ContainsPoint(Point p)
        {
            return endPoints.Select(x => x.Point).Contains(p);
        }
    }
}