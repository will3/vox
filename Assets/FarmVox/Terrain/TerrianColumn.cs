using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianColumn
    {
        private readonly Vector3Int _origin;
        public readonly List<TerrianChunk> TerrianChunks;
        
        public float Distance {
            get
            {
                return Mathf.Abs(_origin.x) + Mathf.Abs(_origin.z);
            }
        }

        public TerrianColumn(int size, Vector3Int origin, List<TerrianChunk> terrianChunks)
        {
            if (origin.y != 0) {
                throw new System.ArgumentException("origin y has to be 0");
            }

            foreach (var tc in terrianChunks) {
                if (tc.Origin.x != origin.x || tc.Origin.z != origin.z) {
                    throw new System.ArgumentException("terrian chunk doesn't belong to column");
                }
            }

            _origin = origin;
            TerrianChunks = terrianChunks;
        }
    }
}
