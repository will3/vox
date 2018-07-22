using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FarmVox
{
    public class Routes
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
            
            return null;
            //var coord = PosToCoord(now);
            //var connection = GetConnection(coord);
            //if (connection == null) {
            //    Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
            //    return null;    
            //}

            //var diff = (to - now);
            //diff.y = 0;

            //var maxMag = 0.4f;
            //if (diff.magnitude > maxMag) {
            //    diff = diff.normalized * maxMag;
            //}

            //var projected = now + diff;

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

        HashSet<Vector3Int> coords = new HashSet<Vector3Int>();

        void LoadConnections(Chunk chunk, TerrianConfig config)
        {
            coords.Clear();

            var halfSize = chunk.Size / resolution;
            var dots = new Array3<int>(halfSize, halfSize + 1, halfSize);

            for (var i = 0; i < halfSize; i ++) 
            {
                for (var j = 0; j < halfSize + 1; j++)
                {
                    for (var k = 0; k < halfSize; k++)
                    {
                        var x = i * resolution;
                        var y = j * resolution;
                        var z = k * resolution;

                        var total = chunk.Get(x, y, z) >= 0 ? 1 : 0 +
                            chunk.Get(x + 1, y, z) >= 0 ? 1 : 0 +
                            chunk.Get(x, y + 1, z) >= 0 ? 1 : 0 +
                            chunk.Get(x + 1, y + 1, z) >= 0 ? 1 : 0 +
                            chunk.Get(x, y, z + 1) >= 0 ? 1 : 0 +
                            chunk.Get(x + 1, y, z + 1) >= 0 ? 1 : 0 +
                            chunk.Get(x, y + 1, z + 1) >= 0 ? 1 : 0 +
                            chunk.Get(x + 1, y + 1, z + 1) >= 0 ? 1 : 0;

                        dots.Set(i, j, k, total);
                    }
                }
            }

            var surfaces = new HashSet<Vector3Int>();
            for (var i = 0; i < halfSize; i++) {
                for (var j = 0; j < halfSize; j++) {
                    for (var k = 0; k < halfSize; k++) {
                        var a = dots.Get(i, j, k);
                        var b = dots.Get(i, j + 1, k);
                        if (a == 8 && b < 8 || (a < 8 && b == 0)) {
                            coords.Add(new Vector3Int(i, j, k) * resolution + origin);
                        }
                    }
                }
            }
        }
    }
}