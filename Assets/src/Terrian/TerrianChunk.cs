using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FarmVox
{
    public partial class TerrianChunk
    {
        readonly HashSet<int> waters = new HashSet<int>();

        public readonly Vector3Int key;
        public bool rockNeedsUpdate = true;
        public bool waterNeedsUpdate = true;
        public bool grassNeedsUpdate = true;
        public bool treesNeedsUpdate = true;
        public bool housesNeedsUpdate = true;
        public bool routesNeedsUpdate = true;
        public bool townPointsNeedsUpdate = true;
        public bool roadsNeedsUpdate = true;
        public bool enemiesNeedsUpdate = true;
        public bool floatingNeedsUpdate = true;
        public bool waterfallsNeedsUpdate = true;
        public TerrianColumn column;
        public ComputeBuffer colorBuffer;
        public ComputeBuffer canyonBuffer;

        Terrian terrian;

        int distance;
        Vector3Int origin;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        private readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();

        private int size;

        public int Distance
        {
            get
            {
                return distance;
            }
        }

        private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
        public Material Material
        {
            get
            {
                return material;
            }
        }

        public TerrianChunk(Vector3Int key, int size, Terrian terrian)
        {
            this.key = key;
            this.origin = key * size;
            this.size = size;
            this.dataSize = size + 3;
            this.terrian = terrian;
        }

        public readonly int dataSize;

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

        public void SetWater(Vector3Int coord, bool flag)
        {
            SetWater(coord.x, coord.y, coord.z, flag);
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

        public bool GetWater(Vector3Int coord)
        {
            return GetWater(coord.x, coord.y, coord.z);
        }

        public bool GetWater(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            return waters.Contains(index);
        }

        private int getIndex(int i, int j, int k)
        {
            int index = i * dataSize * dataSize + j * dataSize + k;
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
                if (sqrDis < sqrMin)
                {
                    amount += 1.0f / sqrDis;
                }
            }

            return amount;
        }

        readonly HashSet<Vector3Int> townPoints = new HashSet<Vector3Int>();

        public HashSet<Vector3Int> TownPoints
        {
            get
            {
                return townPoints;
            }
        }

        public TerrianConfig Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value;
            }
        }

        public void AddTownPoint(Vector3Int townPoint)
        {
            townPoints.Add(townPoint);
        }

        TerrianConfig config;

        HashSet<Vector3Int> floating = new HashSet<Vector3Int>();
        public void SetFloating(Vector3Int coord)
        {
            floating.Add(coord);
        }

        public Bounds Bounds {
            get {
                var bounds = new Bounds();
                bounds.min = origin;
                bounds.max = origin + new Vector3(size, size, size);
                return bounds;
            }
        }

        public ComputeBuffer LoadCanyonBuffer()
        {
            if (canyonBuffer == null)
            {
                var canyonNoise = new Perlin3DGPU(config.canyonNoise, dataSize, origin);
                canyonNoise.Dispatch();
                canyonBuffer = canyonNoise.Results;
            }

            return canyonBuffer;
        }

        public void Dispose() {
            Object.Destroy(material);
            if (colorBuffer != null) {
                colorBuffer.Dispose();
            }
            if (canyonBuffer != null) {
                canyonBuffer.Dispose();
            }
        }
    }
}