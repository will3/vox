using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianChunk
    {
        private readonly HashSet<int> _waters = new HashSet<int>();

//        public bool RockNeedsUpdate = true;
//        public bool WaterNeedsUpdate = true;
//        public bool TreesNeedsUpdate = true;
//        public bool WaterfallsNeedsUpdate = true;

        public readonly Vector3Int Origin;

        private readonly int _size;
        private readonly int _dataSize;
        
        public TerrianChunk(Vector3Int key, int size)
        {
            Origin = key * size;
            _size = size;
            _dataSize = size + 3;
        }

        public void SetWater(Vector3Int coord, bool flag)
        {
            SetWater(coord.x, coord.y, coord.z, flag);
        }

        public void SetWater(int i, int j, int k, bool flag)
        {
            var index = GetIndex(i, j, k);
            if (flag)
            {
                _waters.Add(index);
            }
            else
            {
                _waters.Remove(index);
            }
        }

        public bool GetWater(Vector3Int coord)
        {
            return GetWater(coord.x, coord.y, coord.z);
        }

        private bool GetWater(int i, int j, int k)
        {
            var index = GetIndex(i, j, k);
            return _waters.Contains(index);
        }

        private int GetIndex(int i, int j, int k)
        {
            var index = i * _dataSize * _dataSize + j * _dataSize + k;
            return index;
        }

        public float Distance {
            get {
                var xDiff = Mathf.Abs(Origin.x + _size / 2);
                var yDiff = Mathf.Abs(Origin.z + _size / 2);
                return xDiff + yDiff;
            }
        }
        
        public void Dispose() {
            
        }
    }
}