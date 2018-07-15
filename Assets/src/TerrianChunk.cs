using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class TerrianChunk
    {
        private readonly Dictionary<Vector3Int, Vector3> normals = new Dictionary<Vector3Int, Vector3>();

        public IDictionary<Vector3Int, Vector3> Normals
        {
            get
            {
                return normals;
            }
        }

        private readonly HashSet<int> waters = new HashSet<int>();
        private readonly Routes routes = new Routes();

        public Routes Routes
        {
            get
            {
                return routes;
            }
        }

        private Vector3Int key;
        public bool rockNeedsUpdate = true;
        public bool waterNeedsUpdate = true;
        public bool grassNeedsUpdate = true;
        public bool treesNeedsUpdate = true;
        public bool normalsNeedsUpdate = true;
        public bool shadowsNeedsUpdate = true;
        public bool housesNeedsUpdate = true;
        public bool routesNeedsUpdate = true;
        public Terrian Terrian;

        private int distance;
        private Vector3Int origin;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        public Chunk Chunk;
        private readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();

        private int size;

        public int Distance
        {
            get
            {
                return distance;
            }
        }

        public TerrianChunk(Vector3Int key, int size)
        {
            this.key = key;
            this.origin = key * size;
            this.size = size;
        }

        public void UpdateDistance(int x, int z)
        {
            var xDis = Mathf.Abs(x - this.key.x);
            var zDis = Mathf.Abs(z - this.key.z);
            distance = Mathf.Max(xDis, zDis);
        }

        public void SetTree(Vector3Int coord, bool flag)
        {
            if (flag)
            {
                trees.Add(coord);
            }
            else
            {
                trees.Remove(coord);
            }
        }

        public bool GetTree(Vector3Int coord)
        {
            return trees.Contains(coord);
        }

        public void SetNormal(Vector3Int coord, Vector3 normal)
        {
            normals[coord] = normal;
        }

        public Vector3 GetNormal(Vector3Int coord)
        {
            return normals[coord];
        }

        public void SetWater(int i, int j, int k, bool flag)
        {
            var index = getIndex(i, j, k);
            if (flag)
            {
                waters.Add(index);
            }
            else
            {
                waters.Remove(index);
            }
        }

        public bool GetWater(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            return waters.Contains(index);
        }

        private int getIndex(int i, int j, int k)
        {
            int index = i * size * size + j * size + k;
            return index;
        }

        public float GetOtherTrees(Vector3Int from)
        {
            float min = 5.0f;
            float sqrMin = min * min;
            var amount = 0f;
            foreach (var coord in trees)
            {
                var sqrDis = (coord - from).sqrMagnitude;
                if (sqrDis < sqrMin) {
                    amount += 1.0f / sqrDis;
                }
            }

            return amount;
        }

        public void UpdateRoutes() {
            if (!routesNeedsUpdate) {
                return;
            }
            routes.Clear();
            routes.LoadChunk(Chunk);
            routesNeedsUpdate = false;
        }

        public void DrawRoutesGizmos() {
            Gizmos.color = Color.red;
            var offset = new Vector3(0.5f, 1.5f, 0.5f);
            foreach(var kv in routes.Map) {
                var from = kv.Key + offset;
                foreach(var b in kv.Value) {
                    var to = b + offset;
                    Gizmos.DrawLine(from, to);
                }
            }
        }
    }
}