using FarmVox.Threading;
using UnityEngine;

namespace FarmVox
{
    public class GenGroundWorker : Worker
    {
        private readonly TerrianChunk _terrianChunk;
        private readonly Chunks _layer;
        private readonly TerrianConfig _config;

        public GenGroundWorker(TerrianChunk terrianChunk, Chunks layer, TerrianConfig config)
        {
            _terrianChunk = terrianChunk;
            _layer = layer;
            _config = config;
        }
        
        public override void Start()
        {
            if (!_terrianChunk.rockNeedsUpdate)
            {
                return;
            }

            var origin = _terrianChunk.Origin;
            var chunk = _layer.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(_config.Size, origin, _config);

            var voxelBuffer = genTerrianGpu.CreateVoxelBuffer();
            var colorBuffer = genTerrianGpu.CreateColorBuffer();
            var typeBuffer = genTerrianGpu.CreateTypeBuffer();

            genTerrianGpu.Dispatch(voxelBuffer, colorBuffer, typeBuffer, _terrianChunk);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var typeBufferData = new int[typeBuffer.count];
            typeBuffer.GetData(typeBufferData);

            var colorBufferData = new Color[colorBuffer.count];
            colorBuffer.GetData(colorBufferData);

            chunk.SetColors(colorBufferData);
            chunk.SetData(voxelBufferData);
            chunk.SetTypes(typeBufferData);

            _terrianChunk.rockNeedsUpdate = false;

            voxelBuffer.Dispose();
            typeBuffer.Dispose();
            colorBuffer.Dispose();
        }
    }
}