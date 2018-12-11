using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    class TerrianChunkBuildGrid {

    }

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

        Terrain.Terrian terrian;

        Vector3Int origin;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        int size;

        public TerrianChunk(Vector3Int key, int size, Terrain.Terrian terrian)
        {
            this.key = key;
            origin = key * size;
            this.size = size;
            dataSize = size + 3;
            this.terrian = terrian;
        }

        public readonly int dataSize;

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

        public float Distance {
            get {
                var xDiff = Mathf.Abs(origin.x + size / 2);
                var yDiff = Mathf.Abs(origin.z + size / 2);
                return xDiff + yDiff;
            }
        }
        
        public void Dispose() {
            
        }
    }
}