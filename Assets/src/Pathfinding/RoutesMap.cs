using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public class RoutesMap
    {
        Terrian terrian;
        int resolution = 2;

        public RoutesMap(Terrian terrian) {
            this.terrian = terrian;
        }

        readonly Dictionary<Vector3Int, Routes> map = new Dictionary<Vector3Int, Routes>();

        public Routes GetOrCreateRoutes(Vector3Int origin) {
            if (!map.ContainsKey(origin)) {
                map[origin] = new Routes(origin, terrian);
                map[origin].routesMap = this;
            }

            return map[origin];
        }

        Routes GetRoutes(Vector3Int origin) {
            if (map.ContainsKey(origin)) {
                return map[origin];
            }
            return null;
        }

        public Vector3Int GetOrigin(Vector3 coord)
        {
            return terrian.GetOrigin(coord.x, coord.y, coord.z);
        }

        public Vector3Int GetOrigin(float i, float j, float k) {
            return terrian.GetOrigin(i, j, k);
        }

        public void DrawGizmos() {
            foreach(var kv in map) {
                kv.Value.DrawGizmos();
            }
        }

        public Vector3Int GetNode(Vector3 vector)
        {
            var rf = (float)resolution;
            return new Vector3Int(
                Mathf.FloorToInt(vector.x / rf) * resolution,
                Mathf.FloorToInt(vector.y / rf) * resolution,
                Mathf.FloorToInt(vector.z / rf) * resolution);
        }

        public Vector3Int? GetExistingNode(Vector3 vector) {
            var node = GetNode(vector);
            if (HasNode(node)) {
                return node;
            }
            return null;
        }

        public Vector3Int? GetNodeCloseTo(Vector3 vector) {
            var node = GetNode(vector);
            if (HasNode(node)) {
                return node;
            }

            var connections = GetConnections(node);

            var minDis = Mathf.Infinity;
            Vector3Int? n = null;
            foreach(var connection in connections) {
                var dis = (connection - vector).magnitude;
                if (dis < minDis) {
                    minDis = dis;
                    n = connection;
                }
            }
            return n;
        }

        public HashSet<Vector3Int> GetConnections(Vector3Int node)
        {
            var set = new HashSet<Vector3Int>();
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        if (i == 0 && k == 0 && (j == -1 || j == 1))
                        {
                            continue;
                        }

                        var next = node + new Vector3Int(i, j, k) * resolution;
                        if (HasNode(next))
                        {
                            set.Add(next);
                        }
                    }
                }
            }
            return set;
        }

        public bool HasNode(Vector3Int node)
        {
            var origin = GetOrigin(node.x, node.y, node.z);
            if (!map.ContainsKey(origin)) {
                return false;
            }

            return map[origin].HasNode(node);
        }

        public HashSet<Vector3Int> GetCoordsInNode(Vector3Int node) {
            var origin = GetOrigin(node);
            if (!map.ContainsKey(origin))
            {
                return new HashSet<Vector3Int>();
            }

            return map[origin].GetCoordsInNode(node);
        }

        List<RoutingAgent> agents = new List<RoutingAgent>();

        public void AddAgent(RoutingAgent agent) {
            agents.Add(agent);
        }

        public void Update() {
            UpdatePhysics();
        }

        void UpdatePhysics() {
            for (var i = 0; i < agents.Count; i++) {
                for (var j = i; j < agents.Count; j++)
                {
                    if (i == j) {
                        continue;
                    }

                    var b = agents[j];
					var a = agents[i];
                    var factor = 1.0f;

                    if (a.goal != null || b.goal != null)
                    {
                        continue;
                    }

                    var diff = (b.position - a.position);
                    diff += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 0.01f;

                    var dis = diff.magnitude;
                    if (dis < a.radius + b.radius)
                    {
                        var dir = diff.normalized;
                        var force = dir * (a.radius + b.radius - dis) * factor;

                        float aAmount = 1.0f;
                        float bAmount = 1.0f;

                        if (a.goal == null && b.goal == null) {
                            aAmount = 0.5f;
                            bAmount = 0.5f;
                        } else if (a.goal != null && b.goal != null) {
                            if (a.priority > b.priority) {
                                aAmount = 0.0f;
                                bAmount = 1.0f;
                            } else {
                                aAmount = 1.0f;
                                bAmount = 0.0f;
                            }
                        } else if (a.goal != null) {
                            aAmount = 0.2f;
                            bAmount = 0.8f;
                        } else if (b.goal != null) {
                            aAmount = 0.8f;
                            bAmount = 0.2f;
                        }

                        aAmount = 0.5f;
                        bAmount = 0.5f;

                        b.Push(force * bAmount);
                        a.Push(-force * aAmount);

                        if (b.reachedGoal.HasValue) {
                            if (a.goal == b.reachedGoal)
                            {
                                a.ConsiderReachedGoal();
                            }    
                        }

                        if (a.reachedGoal.HasValue) {
                            if (b.goal == a.reachedGoal)
                            {
                                b.ConsiderReachedGoal();
                            }
                        }
                    }
                }    
            }
        }
    }
}