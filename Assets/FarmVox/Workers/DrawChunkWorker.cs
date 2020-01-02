using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class DrawChunkWorker : IWorker
    {
        private readonly TerrianConfig _config;
        private readonly VoxelShadowMap _shadowMap;
        private readonly Chunk _chunk;
        private readonly TerrianChunk _terrianChunk;
        
        public DrawChunkWorker(TerrianConfig config, VoxelShadowMap shadowMap, Chunk chunk, TerrianChunk terrianChunk)
        {
            _config = config;
            _shadowMap = shadowMap;
            _chunk = chunk;
            _terrianChunk = terrianChunk;
        }
        
        public void Start()
        {
            
        }

        
    }
}