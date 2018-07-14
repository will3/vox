using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox {
    public class Routes
    {
        private Dictionary<Vector3Int, HashSet<Vector3Int>> map = new Dictionary<Vector3Int, HashSet<Vector3Int>>();

        public Dictionary<Vector3Int, HashSet<Vector3Int>> Map
        {
            get
            {
                return map;
            }
        }

        public void LoadChunk(Chunk chunk)
        {
            chunk.UpdateSurfaceCoords();
            var coords = chunk.SurfaceCoordsUp;
            var origin = chunk.Origin;

            foreach (var coord in coords) {
                map[coord + origin] = new HashSet<Vector3Int>();
            }

            var keys = map.Keys;
            foreach(var a in keys) {
                foreach(var b in keys) {
                    if (a == b) {
                        continue;
                    }

                    if ((a - b).sqrMagnitude <= 3) {
                        map[a].Add(b);
                        map[b].Add(a);
                    }
                }

                var chunks = chunk.Chunks;

                if (a.x == origin.x)
                {
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            var outsideCoord = new Vector3Int(a.x - 1, a.y + u, a.z + v);
                            if (chunks.IsUp(outsideCoord))
                            {
                                map[a].Add(outsideCoord);
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
                            if (chunks.IsUp(outsideCoord))
                            {
                                map[a].Add(outsideCoord);
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
                            if (chunks.IsUp(outsideCoord))
                            {
                                map[a].Add(outsideCoord);
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
                            if (chunks.IsUp(outsideCoord))
                            {
                                map[a].Add(outsideCoord);
                            }
                        }
                    }
                }
            }
        }

        public void Clear() {
            map.Clear();
        }

        public Vector3Int? GetNodeCloseTo(Vector3Int from)
        {
            var nodes = Map.Keys;

            Vector3Int? minNode = null;
            var minDis = Mathf.Infinity;

            foreach (var node in nodes)
            {
                var dis = (node - from).sqrMagnitude;
                if (dis < minDis)
                {
                    minDis = dis;
                    minNode = node;
                }
            }

            return minNode;
        }
    }
}