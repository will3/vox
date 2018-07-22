using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class RoutingAgent
    {
        public Vector3 position;
        private Stack<Vector3Int> path = new Stack<Vector3Int>();

        private Routes GetRoutes()
        {
            var terrian = Finder.FindTerrian();
            var routesMap = terrian.RoutesMap;
            var origin = routesMap.GetOrigin(position);
            return routesMap.GetRoutes(origin);
        }

        public void Drag(Vector3 to) {
            var routes = GetRoutes();
            var nextPosition = routes.Drag(position, to);

            if (nextPosition != null)
            {
                position = nextPosition.Value;
            }
        }

        public void Navigate(Vector3 to) {
            Drag(to);

            //if (path.Count > 0) {
            //    FollowPath();
            //    return;
            //}
            //var routes = GetRoutes();

            //var nextPosition = routes.Drag(position, to);

            //if (nextPosition == null) {
            //    var bestCoord = routes.AStar(position, to);
            //    if (bestCoord != null) {
            //        path.Clear();
            //        path.Push(bestCoord.Value);
            //    }
            //}

            //if (nextPosition != null) {
            //    position = nextPosition.Value;
            //}
        }

        private void FollowPath() {
            //var routes = GetRoutes();

            //var currentCoord = PosToCoord(position);
            //var nextCoord = path.Peek();

            //if (currentCoord == nextCoord) {
            //    path.Pop();
            //    if (path.Count == 0) {
            //        return;
            //    }
            //}

            //nextCoord = path.Peek();
            //var nextCoordPos = CoordToPos(nextCoord);
            //var disSq = (nextCoordPos - position).sqrMagnitude;
            //// Knocked off path
            //if (disSq > 3) {
            //    path.Clear();
            //    return;
            //}

            //var nextPosition = routes.ForceDrag(position, nextCoordPos);

            //position = nextPosition;
        }
    }
}