﻿using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class GenGroundWorker : IWorker
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
        
        public void Start()
        {
            var origin = _terrianChunk.Origin;
            var chunk = _layer.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(_config.Size, origin, _config);

            var voxelBuffer = genTerrianGpu.CreateVoxelBuffer();
            var colorBuffer = genTerrianGpu.CreateColorBuffer();

            genTerrianGpu.Dispatch(voxelBuffer, colorBuffer);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var colorBufferData = new Color[colorBuffer.count];
            colorBuffer.GetData(colorBufferData);

            chunk.SetColors(colorBufferData);
            chunk.SetData(voxelBufferData);

            voxelBuffer.Dispose();
            colorBuffer.Dispose();
        }
        
        public float Priority
        {
            get { return Priorities.GenGround - _terrianChunk.Distance / 1024f; }
        }
    }
}