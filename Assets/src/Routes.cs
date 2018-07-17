using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox {
    public class Routes
    {
        public class Connection {
            public Vector3Int node;
            public float cost;
            public Connection(Vector3Int next, float cost) {
                this.node = next;
                this.cost = cost;
            }
        }

        private Dictionary<Vector3Int, HashSet<Connection>> map = new Dictionary<Vector3Int, HashSet<Connection>>();

        public Dictionary<Vector3Int, HashSet<Connection>> Map
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
                map[coord + origin] = new HashSet<Connection>();
            }

            var keys = map.Keys;
            foreach(var a in keys) {
                foreach(var b in keys) {
                    if (a == b) {
                        continue;
                    }

                    var distanceSq = (a - b).sqrMagnitude;
                    if (distanceSq <= 3) {
                        var distance = Mathf.Sqrt(distanceSq);
                        map[a].Add(new Connection(b, distance));
                        map[b].Add(new Connection(a, distance));
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
                                var distance = (a - outsideCoord).magnitude;
                                map[a].Add(new Connection(outsideCoord, distance));
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
                                var distance = (a - outsideCoord).magnitude;
                                map[a].Add(new Connection(outsideCoord, distance));
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
                                var distance = (a - outsideCoord).magnitude;
                                map[a].Add(new Connection(outsideCoord, distance));
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
                                var distance = (a - outsideCoord).magnitude;
                                map[a].Add(new Connection(outsideCoord, distance));
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