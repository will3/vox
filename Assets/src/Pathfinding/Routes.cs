using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Priority_Queue;

namespace FarmVox
{
    public partial class Routes
    {
        Vector3Int origin;
        private Terrian terrian;
        private int size;
        public RoutesMap routesMap;
        public int resolution = 2;

        public Routes(Vector3Int origin, Terrian terrian)
        {
            this.terrian = terrian;
            this.size = terrian.Size;
            this.origin = origin;
            var halfSize = this.size / resolution;
        }

        // private HashSet<Vector3Int> nodes = new HashSet<Vector3Int>();

        private Mutex editingMutex = new Mutex();

        public void LoadChunk(Chunk chunk, TerrianConfig config)
        {
            editingMutex.WaitOne();

            LoadConnections(chunk, config);

            editingMutex.ReleaseMutex();
        }

        void AddConnectedNodes(FastPriorityQueue<RouteNode> leads, HashSet<Vector3Int> visited, Vector3Int node) {
            AddExistingNodeWithOffset(leads, visited, node, -1, -1, 0);
            AddExistingNodeWithOffset(leads, visited, node, -1, 0, 0);
            AddExistingNodeWithOffset(leads, visited, node, -1, 1, 0);

            AddExistingNodeWithOffset(leads, visited, node, 1, -1, 0);
            AddExistingNodeWithOffset(leads, visited, node, 1, 0, 0);
            AddExistingNodeWithOffset(leads, visited, node, 1, 1, 0);

            AddExistingNodeWithOffset(leads, visited, node, 0, -1, -1);
            AddExistingNodeWithOffset(leads, visited, node, 0, 0, -1);
            AddExistingNodeWithOffset(leads, visited, node, 0, 1, -1);

            AddExistingNodeWithOffset(leads, visited, node, 0, -1, 1);
            AddExistingNodeWithOffset(leads, visited, node, 0, 0, 1);
            AddExistingNodeWithOffset(leads, visited, node, 0, 1, 1);
        }

        void AddExistingNodeWithOffset(FastPriorityQueue<RouteNode> leads, HashSet<Vector3Int> visited, Vector3Int node, int stepX, int stepY, int stepZ) {
            var n = node + new Vector3Int(stepX, stepY, stepZ) * 2;
            if (visited.Contains(n)) {
                return;
            }
            if (HasNode(n)) {
                var routeNode = new RouteNode(n);
                float cost = 0.0f;
                leads.Enqueue(routeNode, cost);
            }
        }

        private const int maxNodeSize = 32768;

        public Vector3Int[] AStar(Vector3 now, Vector3 to, int maxSteps = 16)
        {
            var currentNode = GetNode(now);
            var leads = new FastPriorityQueue<RouteNode>(maxNodeSize);
            leads.Enqueue(new RouteNode(currentNode), 0.0f);
            var visited = new HashSet<Vector3Int>();
                
            int stepCount = 0;
            while (leads.Count > 0) {
                AddConnectedNodes(leads, visited, currentNode);

                if (stepCount >= maxSteps)
                {
                    break;
                }
                stepCount++;

                var removed = leads.Dequeue();
                visited.Add(removed.coord);
            }

            //if (!HasNodeBelow(now))
            //{
            //    Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
            //    return null;
            //}

            //var coord = new Vector3Int(Mathf.FloorToInt(now.x), Mathf.FloorToInt(now.y - 1), Mathf.FloorToInt(now.z));
            //var connected = GetConnectedCoords(coord);

            //Vector3Int? closestNext = null;
            //var minDis = Mathf.Infinity;

            //foreach(var next in connected) {
            //    var dis = (next - to).sqrMagnitude;
            //    if (dis < minDis) {
            //        minDis = dis;
            //        closestNext = next;
            //    }
            //}

            //return closestNext;
            return null;
        }

        //private Vector3Int PosToCoord(Vector3 pos)
        //{
        //    return new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y) - 1, Mathf.FloorToInt(pos.z));
        //}

        //private Vector3 CoordToPos(Vector3Int coord)
        //{
        //    return coord + new Vector3(0.5f, 1.5f, 0.5f);
        //}

        public Vector3Int? GetExistingNode(Vector3 vector) {
            var rf = (float)resolution;
            var node = new Vector3Int(
                Mathf.FloorToInt(vector.x / rf) * resolution,
                Mathf.FloorToInt(vector.y / rf) * resolution,
                Mathf.FloorToInt(vector.z / rf) * resolution);

            if (HasNode(node)) {
                return node;    
            }
            return null;
        }

        private bool HasNode(Vector3Int node) {
            var o = routesMap.GetOrigin(node.x, node.y, node.z);
            if (o != this.origin) {
                var routes = routesMap.GetRoutes(o);    
                if (routes == null) {
                    return false;
                }
                return routes.HasNode(node);
            } else {
                return nodes.ContainsKey(node);
            }
        }

        public Vector3Int GetNode(Vector3 vector) {
            var rf = (float)resolution;
            return new Vector3Int(
                Mathf.FloorToInt(vector.x / rf) * resolution,
                Mathf.FloorToInt(vector.y / rf) * resolution,
                Mathf.FloorToInt(vector.z / rf) * resolution);
        }

        public Vector3Int? SteppingNode(Vector3Int node) {
            var up = new Vector3Int(node.x, node.y + resolution, node.z);
            if (HasNode(up)) {
                return up;
            }

            if (HasNode(node)) {
                return node;
            }

            var down = new Vector3Int(node.x, node.y - resolution, node.z);
            if (HasNode(down)) {
                return down;
            }

            return null;
        }

        public Vector3? Drag(Vector3 now, Vector3 to) {
            var nodeNullable = GetExistingNode(now);
            if (nodeNullable == null) {
                Debug.Log("invalid position");
                return null;
            }
            var node = nodeNullable.Value;

            var diff = (to - now);
            diff.y = 0;

            var maxMag = 0.4f;
            if (diff.magnitude > maxMag) {
                diff = diff.normalized * maxMag;
            }

            var projected = now + diff;

            if (diff.x > 0) {
                var nextNode = SteppingNode(node + new Vector3Int(resolution, 0, 0));
                if (nextNode == null) {
                    if (projected.x > node.x + 1) {
                        projected.x = node.x + 1;
                    }
                }
            }

            if (diff.x < 0) {
                var nextNode = SteppingNode(node + new Vector3Int(-resolution, 0, 0));
                if (nextNode == null) {
                    if (projected.x < node.x + 1) {
                        projected.x = node.x + 1;
                    }
                }
            }

            if (diff.z > 0)
            {
                var nextNode = SteppingNode(node + new Vector3Int(0, 0, resolution));
                if (nextNode == null)
                {
                    if (projected.z > node.z + 1)
                    {
                        projected.z = node.z + 1;
                    }
                }
            }

            if (diff.z < 0)
            {
                var nextNode = SteppingNode(node + new Vector3Int(0, 0, -resolution));
                if (nextNode == null)
                {
                    if (projected.z < node.z + 1)
                    {
                        projected.z = node.z + 1;
                    }
                }
            }

            ;
            var projectedNode = GetNode(projected);
            var projectedSteppingNode = SteppingNode(projectedNode);

            if (projectedSteppingNode == null) {
                // Huuhhhh?
                return null;
            }

            projected.y = projectedSteppingNode.Value.y;
            return projected;
            //var coord = PosToCoord(now);
            //var connection = GetConnection(coord);
            //if (connection == null) {
            //    Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
            //    return null;    
            //}
            //var currentCoord = PosToCoord(now);
            //var currentCoordMid = CoordToPos(currentCoord);

            //if (diff.x < 0) {
            //    var left = GetConnection(currentCoord + new Vector3Int(-1, 0, 0));
            //    if (left == null)
            //    {
            //        if (projected.x < currentCoordMid.x) {
            //            projected.x = currentCoordMid.x;
            //        }
            //    }    
            //}

            //if (diff.x > 0) {
            //    var right = GetConnection(currentCoord + new Vector3Int(1, 0, 0));
            //    if (right == null) {
            //        if (projected.x > currentCoordMid.x) {
            //            projected.x = currentCoordMid.x;
            //        }
            //    }
            //}

            //if (diff.z < 0) {
            //    var back = GetConnection(currentCoord + new Vector3Int(0, 0, -1));
            //    if (back == null) {
            //        if (projected.z < currentCoordMid.z) {
            //            projected.z = currentCoordMid.z;
            //        }
            //    }
            //}

            //if (diff.z > 0) {
            //    var front = GetConnection(currentCoord + new Vector3Int(0, 0, 1));
            //    if (front == null) {
            //        if (projected.z > currentCoordMid.z)
            //        {
            //            projected.z = currentCoordMid.z;
            //        }
            //    }
            //}

            //var nextCoord = PosToCoord(projected);

            //connection = GetConnection(nextCoord);

            //if (connection == null) {
            //    // can't move
            //    return null;
            //}

            //projected.y = connection.Value.y + 1.5f;

            //return projected;
        }

        //private Vector3Int? GetConnection(Vector3Int coord) {
        //    Vector3Int c = new Vector3Int(coord.x, coord.y + 2, coord.z);
        //    if (HasNode(c))
        //    {
        //        return c;
        //    }

        //    c = new Vector3Int(coord.x, coord.y + 1, coord.z);
        //    if (HasNode(c)) {
        //        return c;
        //    }

        //    if (HasNode(coord))
        //    {
        //        return coord;
        //    }

        //    c = new Vector3Int(coord.x, coord.y - 1, coord.z);
        //    if (HasNode(c))
        //    {
        //        return c;
        //    }

        //    c = new Vector3Int(coord.x, coord.y - 2, coord.z);
        //    if (HasNode(c))
        //    {
        //        return c;
        //    }
        //    return null;
        //}

        //private HashSet<Vector3Int> GetConnectedCoords(Vector3Int coord) {
        //    var set = new HashSet<Vector3Int>();
        //    for (var i = -1; i <= 1; i++) {
        //        for (var j = -1; j <= 1; j++)
        //        {
        //            for (var k = -1; k <= 1; k++)
        //            {
        //                if (i == 0 && j == 0 && k == 0) {
        //                    continue;
        //                }
        //                var next = coord + new Vector3Int(i, j, k);
        //                if (HasNode(next)) {
        //                    set.Add(next);
        //                }
        //            }
        //        }    
        //    }
        //    return set;
        //}

        //public bool HasNode(Vector3Int coord) {
        //    return false;
        //    //var o = routesMap.GetOrigin(coord);
        //    //if (o == origin)
        //    //{
        //    //    return nodes.Contains(coord);
        //    //}

        //    //var routes = routesMap.GetRoutes(o);
        //    //if (routes == null)
        //    //{
        //    //    return false;
        //    //}

        //    //return routes.HasNode(coord);
        //}

        //private bool HasNode(int i, int j, int k) {
        //    return HasNode(new Vector3Int(i, j, k));
        //}

        public void DrawGizmos()
        {
            editingMutex.WaitOne();

            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
            //var offset = new Vector3(0.5f, 1.5f, 0.5f);

            foreach(var kv in nodes) {
                var coord = kv.Key;
                var position = coord + new Vector3(1, 2.0f, 1);
                Gizmos.DrawCube(position, new Vector3(1.0f, 1.0f, 1.0f));
            }

            editingMutex.ReleaseMutex();
        }

        readonly Dictionary<Vector3Int, RouteNode> nodes = new Dictionary<Vector3Int, RouteNode>();
        readonly HashSet<Vector3Int> coords = new HashSet<Vector3Int>();

        void LoadConnections(Chunk chunk, TerrianConfig config)
        {
            nodes.Clear();

            var rf = (float)resolution;

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.surfaceCoordsUp) {
                var node = new Vector3Int(
                    Mathf.FloorToInt(coord.x / rf) * resolution,
                    Mathf.FloorToInt(coord.y / rf) * resolution,
                    Mathf.FloorToInt(coord.z / rf) * resolution) + origin;
                nodes[node] = new RouteNode(node);
                coords.Add(coord);
            }
        }
    }
}