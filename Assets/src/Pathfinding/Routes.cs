using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class Routes
    {
        public class Connection
        {
            public Vector3Int node;
            public float cost;
            public Connection(Vector3Int next, float cost)
            {
                this.node = next;
                this.cost = cost;
            }
        }

        private Dictionary<int, HashSet<Vector3Int>> sides = new Dictionary<int, HashSet<Vector3Int>>();

        private Terrian terrian;
        private int size;

        public Routes(Terrian terrian) {
            this.terrian = terrian;
            this.size = terrian.Size;
        }

        private Dictionary<Vector3Int, HashSet<Connection>> map = new Dictionary<Vector3Int, HashSet<Connection>>();

        private bool CanEnter(Vector3Int coord) {
            if(this.terrian.GetTree(coord)) {
                return false;
            }

            return true;
        }

        public void LoadChunk(Chunk chunk)
        {
            LoadConnections(chunk);

            foreach(var kv in map) {
                if (kv.Key.x >= chunk.Origin.x + size || 
                    kv.Key.y >= chunk.Origin.y + size ||
                    kv.Key.z >= chunk.Origin.z + size) {

                    Debug.Log("out of bounds");
                }
            }

            LoadSides();
        }

        void LoadSides() {
            sides.Clear();
            foreach (var coord in map.Keys)
            {
                if (!HasConnectionXZ(coord, -1, 0)) {
                    var left = new Vector3Int(-1, 0, 0) + coord;    
                    AddSide(left);
                }

                if (!HasConnectionXZ(coord, 1, 0))
                {
                    var right = new Vector3Int(1, 0, 0) + coord;
                    AddSide(right);
                }

                if (!HasConnectionXZ(coord, 0, -1))
                {
                    var back = new Vector3Int(0, 0, -1) + coord;
                    AddSide(back);
                }

                if (!HasConnectionXZ(coord, 0, 1))
                {
                    var forward = new Vector3Int(0, 0, 1) + coord;
                    AddSide(forward);
                }
            }
        }

        bool HasConnectionXZ(Vector3Int from, int dx, int dz) {
            var x = from.x + dx;
            var z = from.z + dz;
            var connections = map[from];
            foreach (var coord in connections) {
                if (coord.node.x == x && coord.node.z == z) {
                    return true;
                }
            }
            return false;
        }

        void AddSide(Vector3Int side) {
            if (!sides.ContainsKey(side.y)) {
                sides[side.y] = new HashSet<Vector3Int>();
            }
            sides[side.y].Add(side);
        }

        void LoadConnections(Chunk chunk) {
            chunk.UpdateSurfaceCoords();
            var coords = chunk.surfaceCoordsUp;
            var origin = chunk.Origin;

            foreach (var coord in coords)
            {
                if (coord.x >= size || coord.y >= size || coord.z >= size || 
                    coord.x < 0 || coord.y < 0 || coord.z < 0) {
                    continue;
                }
                map[coord + origin] = new HashSet<Connection>();
            }

            var keys = map.Keys;
            foreach (var a in keys)
            {
                if (!CanEnter(a))
                {
                    continue;
                }
                foreach (var b in keys)
                {
                    if (!CanEnter(b))
                    {
                        continue;
                    }

                    if (a == b)
                    {
                        continue;
                    }

                    var distanceSq = (a - b).sqrMagnitude;
                    if (distanceSq <= 3)
                    {
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
                            if (!CanEnter(outsideCoord)) continue;
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
                            if (!CanEnter(outsideCoord)) continue;
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
                            if (!CanEnter(outsideCoord)) continue;
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
                            if (!CanEnter(outsideCoord)) continue;
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
            var nodes = map.Keys;

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

        public void DrawGizmos()
        {
            Gizmos.color = new Color(0.8f, 0.8f, 0.8f);
            var offset = new Vector3(0.5f, 1.5f, 0.5f);


            foreach (var kv in map)
            {
                var from = kv.Key + offset;
                foreach (var b in kv.Value)
                {
                    var to = b.node + offset;
                    Gizmos.DrawLine(from, to);
                }
            }

            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
            var size = new Vector3(0.5f, 0.5f, 0.5f);
            foreach(var kv in sides) {
                foreach(var side in kv.Value) {
                    Gizmos.DrawCube(side + offset, size);
                }
            }
        }
    }
}