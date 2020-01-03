using System;
using System.Collections.Generic;
using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class GroundConfig
    {
        public float maxHeight = 64;
        public float plainHeight = 4;
        public int groundHeight = 12;
        public float hillHeight = 64;

        public int size = 32;

        // 6F6B65 -> 9C9280
        public ColorGradient rockColor = new ColorGradient(ColorUtils.GetColor("#654d1f"));

        public ColorGradient grassColor = new ColorGradient(ColorUtils.GetColor("#597420"));

        public Noise heightNoise = new Noise
        {
            seed = 1599434415,
            frequency = 0.015f,
            yScale = 0.4f,
            octaves = 5
        };

        public Noise rockColorNoise = new Noise
        {
            frequency = 0.05f,
            yScale = 4.0f,
            amplitude = 1.0f
        };

        public Noise grassNoise = new Noise
        {
            frequency = 0.01f,
            amplitude = 2.0f
        };

        public ValueGradient grassHeightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {0, 0.2f},
            {0.5f, -0.5f},
            {1, -1}
        });

        public ValueGradient grassNormalFilter = new ValueGradient(
            new Dictionary<float, float>
            {
                {-1, 0},
                {-0.5f, 0},
                {1, 1}
            });

        public float grassValue = 1.0f;

        public ValueGradient heightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, -0.27f},
            {-0.2f, 0.05f},
            {0.15f, 0.1f},
            {0.5f, 0.75f},
            {1, 1}
        });
    }

    public class Ground : MonoBehaviour
    {
        public GroundConfig config;
        public Chunks chunks;
        public Water water;

        public void GenerateChunk(TerrianChunk terrianChunk, Terrian terrian)
        {
            var origin = terrianChunk.Origin;
            var chunk = chunks.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(config.size, origin, config, water.config);

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

        public bool IsGround(Vector3Int coord)
        {
            return chunks.Get(coord) > 0;
        }

        public Chunk GetChunk(Vector3Int coord)
        {
            return chunks.GetChunk(coord);
        }
    }
}