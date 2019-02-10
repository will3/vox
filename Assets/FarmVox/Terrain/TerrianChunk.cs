using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianChunk : IWaterfallChunk
    {
        private readonly HashSet<int> _waters = new HashSet<int>();

        public readonly Vector3Int Origin;

        private readonly int _size;
        private readonly int _dataSize;
        private readonly Dictionary<Vector3Int, float> _waterfalls = new Dictionary<Vector3Int, float>();
        
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

        /// <summary>
        /// Set water fall, with local coord
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="v"></param>
        public void SetWaterfall(Vector3Int coord, float v)
        {
            _waterfalls[coord] = v;
        }

        /// <summary>
        /// Get water fall, with local coord
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public float GetWaterfall(Vector3Int coord)
        {
            float value;
            _waterfalls.TryGetValue(coord, out value);
            return value;
        }
        
        public void Dispose() {
            
        }
    }
}