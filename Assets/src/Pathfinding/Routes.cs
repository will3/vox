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

        public Vector3? AStar(Vector3 now, Vector3 to)
        {
            if (!HasNodeBelow(now))
            {
                Debug.Log("invalid pos " + now.x + "," + now.y + "," + now.z);
                return null;
            }

            var coord = new Vector3Int(Mathf.FloorToInt(now.x), Mathf.FloorToInt(now.y - 1), Mathf.FloorToInt(now.z));
            var connected = GetConnectedCoords(coord);

            Vector3Int? closestNext = null;
            var minDis = Mathf.Infinity;

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

            return Drag(now, result);
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

            if (result.y % 1.0f != 0.5f) {
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

        private bool HasNodeBelow(Vector3 node) {
            var coord = new Vector3Int(Mathf.FloorToInt(node.x), Mathf.FloorToInt(node.y), Mathf.FloorToInt(node.z));
            coord.y -= 1;
            return HasNode(coord);
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