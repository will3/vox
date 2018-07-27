using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class TerrianColumn
    {
        Vector3Int origin;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        List<TerrianChunk> terrianChunks;
        public bool generatedTerrian = false;
        public bool generatedColliders = false;
        public bool generatedShadows = false;

        public List<TerrianChunk> TerrianChunks
        {
            get
            {
                return terrianChunks;
            }
        }

        public TerrianColumn(Vector3Int origin, List<TerrianChunk> terrianChunks)
        {
            if (origin.y != 0) {
                throw new System.ArgumentException("origin y has to be 0");
            }

            foreach (var tc in terrianChunks) {
                if (tc.Origin.x != origin.x || tc.Origin.z != origin.z) {
                    throw new System.ArgumentException("terrian chunk doesn't belong to column");
                }
            }

            this.origin = origin;
            this.terrianChunks = terrianChunks;
            foreach (var tc in terrianChunks)
            {
                tc.column = this;
            }
        }
    }
}
