using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    class TerrianChunk
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
        private Routes routes = new Routes();

        private Vector3Int key;
        public bool generated = false;
        public bool GeneratedWater = false;
        public bool GeneratedGrass = false;
        public bool GeneratedTrees = false;
        public bool GeneratedNormals = false;
        public bool GeneratedShadows = false;
        public bool GeneratedHouses = false;

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

        public bool HasTreeCloseBy(Vector3Int from, float minTreeDis)
        {
            foreach (var coord in trees)
            {
                if ((coord - from).magnitude < minTreeDis)
                {
                    return true;
                }
            }
            return false;
        }
    }
}