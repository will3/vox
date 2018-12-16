using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class TerrianColumn
    {
        Vector3Int origin;
        int size;

        public int Size
        {
            get
            {
                return size;
            }
        }

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        public float Distance {
            get {
                var xDiff = Mathf.Abs(origin.x + size / 2);
                var yDiff = Mathf.Abs(origin.y + size / 2);
                return xDiff + yDiff;
            }
        }

        List<TerrianChunk> terrianChunks;

        public bool generatedGround = false;
        public bool generatedTerrian = false;
        public bool generatedShadows = false;
        public bool generatedWater = false;
        public bool generatedTrees = false;
        public bool generatedGrass = false;
        public bool drawn = false;

        public List<TerrianChunk> TerrianChunks
        {
            get
            {
                return terrianChunks;
            }
        }

        public TerrianColumn(int size, Vector3Int origin, List<TerrianChunk> terrianChunks)
        {
            this.size = size;
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
            }
        }
    }
}
