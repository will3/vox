using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace FarmVox
{
    public class RoutingAgent : MonoBehaviour
    {
        public Vector3 position;

        List<Vector3Int> currentPath = new List<Vector3Int>();
        RoutesMap routesMap;
        bool moved = false;
        const int maxNodeSize = 32768;
        float speed = 0.4f;

        public Vector3Int goalRead;
        public Vector3Int reachedGoalRead;
        public Vector3 dirRead;

        public Vector3Int? goal;
        public Vector3Int? reachedGoal;

        public float priority;
        public Vector3 force = new Vector3();

        public bool Moved
        {
            get
            {
                return moved;
            }
        }

        public float radius = 1.0f;

        public void SetRoutesMap(RoutesMap routesMap) {
            this.routesMap = routesMap;
            this.routesMap.AddAgent(this);
        }

        public void DrawGizmos()
        {
            if (currentPath.Count > 0)
            {
                Gizmos.color = Color.red;
                var offset = new Vector3Int(1, 3, 1);
                for (var i = 0; i < currentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(currentPath[i] + offset, currentPath[i + 1] + offset);
                }
            }
        }

        void Start() {
            priority = Random.Range(0.0f, 1.0f);
        }

        void Update()
        {
            moved = false;
            UpdateForce();
            UpdateCurrentPath();

            if (reachedGoal.HasValue) {
                reachedGoalRead = reachedGoal.Value;
            }

            if (goal.HasValue) {
                goal = goal.Value;
            }
        }


        public void ConsiderReachedGoal() {
            if (goal == null) {
                throw new System.Exception("unexpected");
            }
            reachedGoal = goal;
            goal = null;
            currentPath.Clear();
        }

        void UpdateCurrentPath()
        {
            var currentNode = routesMap.GetNode(position);

            if (goal != null) {
                if (currentNode == routesMap.GetNode(goal.Value)) {
                    ConsiderReachedGoal();
                }
            }

            if (goal != null) {
                if (currentPath.Count == 0) {
                    // Reroute
                    AStar(goal.Value);
                }
            }

            if (currentPath.Count > 0)
            {
                Vector3Int next = currentPath[currentPath.Count - 1];

                if (currentNode == next)
                {
                    currentPath.RemoveAt(currentPath.Count - 1);
                    UpdateCurrentPath();
                }
                else
                {
                    Move(next);
                }
            }
        }

        public void Move(Vector3 to)
        {
            var diff = (to - position);

            if (diff.magnitude > speed)
            {
                diff = diff.normalized * speed;
            }

            position += diff;

            dirRead = diff.normalized;

            moved = true;
        }

        public float GetBumpY() {
            var result = _GetBumpY(position);
            if (result == null) {
                Debug.LogWarning("couldn't find coord to step on");
            }
            return result == null ? 0 : result.Value;
        }

        public float? _GetBumpY(Vector3 pos)
        {
            var nodeWrapper = routesMap.GetNodeCloseTo(pos);
            if (nodeWrapper == null)
            {
                return null;
            }

            var node = nodeWrapper.Value;
            var coords = routesMap.GetCoordsInNode(node);

            if (coords.Count == 0)
            {
                return null;
            }

            var minDis = Mathf.Infinity;
            var y = 0f;

            foreach (var coord in coords)
            {
                var xDis = coord.x - pos.x;
                var zDis = coord.z - pos.z;
                var dis = Mathf.Sqrt(xDis * xDis + zDis * zDis);
                if (dis < minDis)
                {
                    minDis = dis;
                    y = coord.y;
                }
            }

            return y - pos.y;
        }

        public void Push(Vector3 diff) {
            force += diff;
        }

        void UpdateForce() {
            var mag = force.magnitude;
            var dir = force.normalized;

            int count = 0;
            while (count <= 3)
            {
                if (TryPush(dir, mag))
                {
                    break;
                }

                mag /= 2.0f;
                count++;
            }

            force = new Vector3();
        }

        public bool TryPush(Vector3 dir, float mag) {
            var projected = position + dir * mag;
            var y = _GetBumpY(projected);
            if (y == null)
            {
                return false;
            }

            position = projected;
            return true;
        }

        public void SetGoal(Vector3 vector) {
            this.goal = routesMap.GetNode(vector);
            reachedGoal = null;
            AStar(this.goal.Value);
        }

        void AStar(Vector3 to, int maxSteps = 64)
        {
            var start = routesMap.GetNode(position);
            var end = routesMap.GetNode(to);

            if (start == end)
            {
                currentPath = new List<Vector3Int>();
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

                var connections = routesMap.GetConnections(currentNode.coord);

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

            currentPath = path;
        }

        public void Pushed(Vector3 dir) {
            //var mag = amount.magnitude;
            //if (goal != null) {
            //    var dot = Vector3.Dot(dir.normalized, (goal.Value - position).normalized);
            //    if (dot < -0.5) {
            //        // Pushed away from goal
            //        pushedAmount += 1.0f; 
            //    }
            //}
        }
    }
}