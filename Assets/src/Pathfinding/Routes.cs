using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FarmVox
{
    public class Routes
    {
        Vector3Int origin;
        public Routes(Vector3Int origin, Terrian terrian)
        {
            this.terrian = terrian;
            this.size = terrian.Size;
            this.origin = origin;
        }

        private Terrian terrian;
        private int size;
        public RoutesMap routesMap;

        private HashSet<Vector3Int> nodes = new HashSet<Vector3Int>();

        private Mutex editingMutex = new Mutex();

        public void LoadChunk(Chunk chunk, TerrianConfig config)
        {
            editingMutex.WaitOne();

            LoadConnections(chunk, config);

            editingMutex.ReleaseMutex();
        }

        public Vector3Int? AStar(Vector3 now, Vector3 to)
        {
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

        public Vector3 ForceDrag(Vector3 now, Vector3 to) {
            return to;
            //var diff = to - now;
            //var maxMag = 0.4f;
            //if (diff.magnitude > maxMag) {
            //    diff = diff.normalized * maxMag;
            //}
            //return now - diff;
        }

        private Vector3Int PosToCoord(Vector3 pos)
        {
            return new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y) - 1, Mathf.FloorToInt(pos.z));
        }

        private Vector3 CoordToPos(Vector3Int coord)
        {
            return coord + new Vector3(0.5f, 1.5f, 0.5f);
        }

        public Vector3? Drag(Vector3 now, Vector3 to) {
            var coord = PosToCoord(now);
            var connection = GetConnection(coord);
            if (connection == null) {
                Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
                return null;    
            }

            var diff = (to - now);
            diff.y = 0;

            var maxMag = 0.4f;
            if (diff.magnitude > maxMag) {
                diff = diff.normalized * maxMag;
            }

            var projected = now + diff;

            var currentCoord = PosToCoord(now);
            var currentCoordMid = CoordToPos(currentCoord);

            if (diff.x < 0) {
                var left = GetConnection(currentCoord + new Vector3Int(-1, 0, 0));
                if (left == null)
                {
                    if (projected.x < currentCoordMid.x) {
                        projected.x = currentCoordMid.x;
                    }
                }    
            }

            if (diff.x > 0) {
                var right = GetConnection(currentCoord + new Vector3Int(1, 0, 0));
                if (right == null) {
                    if (projected.x > currentCoordMid.x) {
                        projected.x = currentCoordMid.x;
                    }
                }
            }

            if (diff.z < 0) {
                var back = GetConnection(currentCoord + new Vector3Int(0, 0, -1));
                if (back == null) {
                    if (projected.z < currentCoordMid.z) {
                        projected.z = currentCoordMid.z;
                    }
                }
            }

            if (diff.z > 0) {
                var front = GetConnection(currentCoord + new Vector3Int(0, 0, 1));
                if (front == null) {
                    if (projected.z > currentCoordMid.z)
                    {
                        projected.z = currentCoordMid.z;
                    }
                }
            }

            var nextCoord = PosToCoord(projected);

            connection = GetConnection(nextCoord);

            if (connection == null) {
                // can't move
                return null;
            }

            projected.y = connection.Value.y + 1.5f;

            return projected;
        }

        private Vector3Int? GetConnection(Vector3Int coord) {
            Vector3Int c = new Vector3Int(coord.x, coord.y + 2, coord.z);
            if (HasNode(c))
            {
                return c;
            }

            c = new Vector3Int(coord.x, coord.y + 1, coord.z);
            if (HasNode(c)) {
                return c;
            }

            if (HasNode(coord))
            {
                return coord;
            }

            c = new Vector3Int(coord.x, coord.y - 1, coord.z);
            if (HasNode(c))
            {
                return c;
            }

            c = new Vector3Int(coord.x, coord.y - 2, coord.z);
            if (HasNode(c))
            {
                return c;
            }
            return null;
        }

        private HashSet<Vector3Int> GetConnectedCoords(Vector3Int coord) {
            var set = new HashSet<Vector3Int>();
            for (var i = -1; i <= 1; i++) {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) {
                            continue;
                        }
                        var next = coord + new Vector3Int(i, j, k);
                        if (HasNode(next)) {
                            set.Add(next);
                        }
                    }
                }    
            }
            return set;
        }

        public bool HasNode(Vector3Int coord) {
            var o = routesMap.GetOrigin(coord);
            if (o == origin)
            {
                return nodes.Contains(coord);
            }

            var routes = routesMap.GetRoutes(o);
            if (routes == null)
            {
                return false;
            }

            return routes.HasNode(coord);
        }

        private bool HasNode(int i, int j, int k) {
            return HasNode(new Vector3Int(i, j, k));
        }

        public void Clear() {
            nodes.Clear();
        }

        public void DrawGizmos()
        {
            editingMutex.WaitOne();

            //Gizmos.color = new Color(0.8f, 0.8f, 0.8f);
            //var offset = new Vector3(0.5f, 1.5f, 0.5f);


            //foreach (var coord in coords)
            //{
            //    var from = kv.Key + offset;
            //    foreach (var b in kv.Value)
            //    {
            //        var to = b.node + offset;
            //        Gizmos.DrawLine(from, to);
            //    }
            //}

            editingMutex.ReleaseMutex();
        }

        bool CanEnter(Vector3Int coord)
        {
            if (this.terrian.GetTree(coord))
            {
                return false;
            }

            return true;
        }

        void LoadConnections(Chunk chunk, TerrianConfig config)
        {
            chunk.UpdateSurfaceCoords();
            var coords = chunk.surfaceCoordsUp;
            var origin = chunk.Origin;

            foreach (var coord in coords)
            {
                if (coord.x >= size || coord.y >= size || coord.z >= size ||
                    coord.x < 0 || coord.y < 0 || coord.z < 0)
                {
                    continue;
                }
                if (coord.y + origin.y < config.waterLevel) {
                    continue;
                }
                nodes.Add(coord + origin);
            }
        }
    }
}