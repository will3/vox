using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace FarmVox
{
    public class RoutingAgent
    {
        public Vector3 position;

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

        List<Vector3Int> currentPath = new List<Vector3Int>();

        public void Navigate(Vector3 to) {
            var routes = GetRoutes();
            var path = AStar(position, to);
            currentPath = path;
        }

        public void DrawGizmos() {
            if (currentPath.Count > 0) {
                Gizmos.color = Color.red;
                var offset = new Vector3Int(1, 1, 1);
                for (var i = 0; i < currentPath.Count - 1; i++) {
                    Gizmos.DrawLine(currentPath[i] + offset, currentPath[i + 1] + offset);
                }
            }
        }

        public void Update() {
            UpdateCurrentPath();
        }

        void UpdateCurrentPath() {
            if (currentPath.Count > 0)
            {
                var routes = GetRoutes();
                var currentNode = routes.GetNode(position);
                Vector3Int next = currentPath[currentPath.Count - 1];

                if (currentNode == next)
                {
                    currentPath.RemoveAt(currentPath.Count - 1);
                    UpdateCurrentPath();
                }
                else
                {
                    Move(next + new Vector3Int());
                }
            }
        }

        const int maxNodeSize = 32768;

        public void Move(Vector3 to) {
            var diff = (to - position);

            var maxMag = 0.4f;
            if (diff.magnitude > maxMag)
            {
                diff = diff.normalized * maxMag;
            }

            position += diff;
        }

        public List<Vector3Int> AStar(Vector3 now, Vector3 to, int maxSteps = 64)
        {
            var routes = GetRoutes();
            var start = routes.GetNode(now);
            var end = routes.GetNode(to);

            if (start == end)
            {
                return new List<Vector3Int>();
            }

            var currentNode = new RouteNode(start);
            var leads = new FastPriorityQueue<RouteNode>(maxNodeSize);
            leads.Enqueue(currentNode, 0.0f);
            var costs = new Dictionary<Vector3Int, float>();
            var distanceBias = 4.0f;
            costs[currentNode.coord] = 0;
            var previous = new Dictionary<Vector3Int, Vector3Int>();
            var closest = Mathf.Infinity;
            Vector3Int closestNode = new Vector3Int();

            int stepCount = 0;
            while (leads.Count > 0)
            {
                currentNode = leads.Dequeue();

                if (currentNode.coord == end)
                {
                    break;
                }

                var connections = routes.GetConnections(currentNode.coord);

                var nodeDis = (currentNode.coord - end).magnitude;

                foreach (var connection in connections)
                {
                    var existingCost = costs.ContainsKey(connection) ? costs[connection] : Mathf.Infinity;

                    var connectionDis = (connection - end).magnitude;

                    var alt = costs[currentNode.coord] + 1 + (connectionDis - nodeDis) * distanceBias;

                    if (alt < existingCost)
                    {
                        costs[connection] = alt;
                        previous[connection] = currentNode.coord;
                        leads.Enqueue(new RouteNode(connection), alt);
                    }

                    if (connectionDis < closest)
                    {
                        closest = connectionDis;
                        closestNode = connection;
                    }
                }

                if (stepCount >= maxSteps)
                {
                    break;
                }

                stepCount++;
            }

            var path = new List<Vector3Int>();
            var smallest = closestNode;
            while (previous.ContainsKey(smallest))
            {
                path.Add(smallest);
                smallest = previous[smallest];
            }

            return path;
        }

    }
}