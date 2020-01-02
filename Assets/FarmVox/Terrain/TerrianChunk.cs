using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianChunk
    {
        public readonly Vector3Int Origin;

        private readonly int _size;
        private readonly int _dataSize;

        public TerrianChunk(Vector3Int key, int size)
        {
            Origin = key * size;
            _size = size;
            _dataSize = size + 3;
        }
    }
}