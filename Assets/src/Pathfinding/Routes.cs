using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FarmVox
{
    public class Routes
    {
        public Routes(Terrian terrian)
        {
            this.terrian = terrian;
            this.size = terrian.Size;
        }

        private Terrian terrian;
        private int size;

        private HashSet<Vector3Int> coordsInside = new HashSet<Vector3Int>();
        private HashSet<Vector3Int> connectedCoordsOutside = new HashSet<Vector3Int>();

        public HashSet<Vector3Int> ConnectedCoordsOutside
        {
            get
            {
                return connectedCoordsOutside;
            }
        }

        private Mutex editingMutex = new Mutex();

        public void LoadChunk(Chunk chunk, TerrianConfig config)
        {
            editingMutex.WaitOne();

            LoadConnections(chunk, config);

            editingMutex.ReleaseMutex();
        }

        public Vector3? AStar(Vector3 now, Vector3 to)
        {
            if (!HasNodeBelow(now))
            {
                Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
                return null;
            }

            var coord = new Vector3Int(Mathf.FloorToInt(now.x), Mathf.FloorToInt(now.y - 1), Mathf.FloorToInt(now.z));
            var connected = GetConnectedCoords(coord);

            var minDis = Mathf.Infinity;
            Vector3Int? closestNext = null;

            foreach(var next in connected) {
                var dis = (next - to).sqrMagnitude;
                if (dis < minDis) {
                    minDis = dis;
                    closestNext = next;
                }
            }

            if (closestNext == null) {
                return null;
            }

            var result = closestNext.Value + new Vector3(0.5f, 1.5f, 0.5f);

            ValidateResult(result);    

            return result;
            //return Drag(now, result);
        }

        public Vector3? Drag(Vector3 now, Vector3 to) {
            if (!HasNodeBelow(now))
            {
                Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
                return null;
            }

            var diff = (to - now);
            var maxMag = 0.4f;
            if (diff.magnitude > maxMag)
            {
                diff = diff.normalized * maxMag;
            }

            var result = Drag(now, diff, 1.0f) ?? Drag(now, diff, 0) ?? Drag(now, diff, -1.0f);
            if (result != null) {
                ValidateResult(result.Value);    
            }
            return result;
        }

        Vector3? Drag(Vector3 now, Vector3 diff, float diffY)
        {
            diff.y = diffY;
            var next = now + diff;

            if (!HasNodeBelow(next))
            {
                return null;
            }

            return next;
        }

        void ValidateResult(Vector3 result) {
            if (!HasNodeBelow(result)) {
                throw new System.Exception("not supposed to happen");
            }
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
                        if (HasCoord(next)) {
                            set.Add(next);
                        }
                    }
                }    
            }
            return set;
        }

        public bool HasCoord(Vector3Int coord) {
            return coordsInside.Contains(coord) || connectedCoordsOutside.Contains(coord);
        }

        private bool HasNodeBelow(Vector3 node) {
            var coord = new Vector3Int(Mathf.FloorToInt(node.x), Mathf.FloorToInt(node.y) - 1, Mathf.FloorToInt(node.z));
            return coordsInside.Contains(coord) || connectedCoordsOutside.Contains(coord);
        }

        private bool HasNode(Vector3Int node) {
            return coordsInside.Contains(node);
        }

        public void Clear() {
            coordsInside.Clear();
            connectedCoordsOutside.Clear();
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
                coordsInside.Add(coord + origin);
            }

            foreach (var a in coordsInside)
            {
                if (!CanEnter(a))
                {
                    continue;
                }

                var chunks = chunk.Chunks;

                if (a.x == origin.x)
                {
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            var outsideCoord = new Vector3Int(a.x - 1, a.y + u, a.z + v);
                            var o = terrian.GetOrigin(outsideCoord.x, outsideCoord.y, outsideCoord.z);
                            var tc = terrian.GetTerrianChunk(o);
                            if (tc == null) continue;
                            if (!CanEnter(outsideCoord)) continue;

                            if (chunks.IsUp(outsideCoord))
                            {
                                connectedCoordsOutside.Add(outsideCoord);
                                tc.Routes.connectedCoordsOutside.Add(a);
                            }
                        }
                    }
                }

                if (a.x == origin.x + chunk.Size - 1)
                {
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            var outsideCoord = new Vector3Int(a.x + 1, a.y + u, a.z + v);
                            var o = terrian.GetOrigin(outsideCoord.x, outsideCoord.y, outsideCoord.z);
                            var tc = terrian.GetTerrianChunk(o);
                            if (tc == null) continue;
                            if (!CanEnter(outsideCoord)) continue;

                            if (chunks.IsUp(outsideCoord))
                            {
                                connectedCoordsOutside.Add(outsideCoord);
                                tc.Routes.connectedCoordsOutside.Add(a);
                            }
                        }
                    }
                }

                if (a.z == origin.z)
                {
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            var outsideCoord = new Vector3Int(a.x + u, a.y + v, a.z - 1);
                            var o = terrian.GetOrigin(outsideCoord.x, outsideCoord.y, outsideCoord.z);
                            var tc = terrian.GetTerrianChunk(o);
                            if (tc == null) continue;
                            if (!CanEnter(outsideCoord)) continue;
                            if (chunks.IsUp(outsideCoord))
                            {
                                connectedCoordsOutside.Add(outsideCoord);
                                tc.Routes.connectedCoordsOutside.Add(a);
                            }
                        }
                    }
                }

                if (a.z == origin.z + chunk.Size - 1)
                {
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            var outsideCoord = new Vector3Int(a.x + u, a.y + v, a.z + 1);
                            var o = terrian.GetOrigin(outsideCoord.x, outsideCoord.y, outsideCoord.z);
                            var tc = terrian.GetTerrianChunk(o);
                            if (tc == null) continue;
                            if (!CanEnter(outsideCoord)) continue;
                            if (chunks.IsUp(outsideCoord))
                            {
                                connectedCoordsOutside.Add(outsideCoord);
                                tc.Routes.connectedCoordsOutside.Add(a);
                            }
                        }
                    }
                }
            }
        }
    }
}