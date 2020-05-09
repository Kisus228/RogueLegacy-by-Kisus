using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RogueLegacy
{
    public static class PathFinder
    {
        public static IEnumerable<Movement> GetShortestPath(List<IMonster> creatures)
        {
            var queue = new Queue<DeltaPointsPath>();
            var visited = new Dictionary<IMonster, HashSet<Point>>();
            var endPointsList = new DeltaPointsPathList();
            var foundPathCreatures = new HashSet<ICreature>();
            InitializeStartLocation(creatures, queue, visited);
            while (queue.Count > 0)
            {
                var pointWithPath = queue.Dequeue();
                if (foundPathCreatures.Contains(pointWithPath.Creature))
                    continue;

                if (creatures.Any(enemy => enemy.CanAttackFromPoint(pointWithPath.Point)) 
                    || GetNeighbours(pointWithPath.Point).Any(p => endPointsList.ContainsPoint(p)))
                {
                    if (endPointsList.ContainsPoint(pointWithPath.Point))
                        pointWithPath = pointWithPath.PrevPoint;
                    endPointsList.Add(pointWithPath);
                    foundPathCreatures.Add(pointWithPath.Creature);
                    if (endPointsList.Count == creatures.Count)
                    {
                        foreach (var move in endPointsList)
                            yield return move;
                    }
                }

                foreach (var newPoint in GetNeighbours(pointWithPath.Point).Where(p => !visited[pointWithPath.Creature].Contains(p)))
                {
                    AddNewPointsToQueue(newPoint, visited, pointWithPath, queue);
                }
            }
        }

        private static void InitializeStartLocation(List<IMonster> creatures, Queue<DeltaPointsPath> queue,
            Dictionary<IMonster, HashSet<Point>> visited)
        {
            foreach (var enemy in creatures)
            {
                queue.Enqueue(new DeltaPointsPath(enemy, enemy.Location));
                visited[enemy] = new HashSet<Point> {enemy.Location};
            }
        }

        private static void AddNewPointsToQueue(Point newPoint, Dictionary<IMonster, HashSet<Point>> visited,
            DeltaPointsPath pointWithPath, Queue<DeltaPointsPath> queue)
        {
            if (!Game.InBounds(newPoint) || Game.Map[newPoint.Y, newPoint.X] != State.Empty) return;
            visited[pointWithPath.Creature].Add(newPoint);
            queue.Enqueue(new DeltaPointsPath(pointWithPath.Creature, newPoint, pointWithPath));
        }

        private static IEnumerable<Point> GetNeighbours(Point p)
        {
            for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
                if ((dx == 0) ^ (dy == 0))
                    yield return new Point(p.X + dx, p.Y + dy);
        }
    }
}