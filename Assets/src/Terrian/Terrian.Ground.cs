﻿using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        private float GetValue(int i, int j, int k, int dataSize, Vector3Int origin, float[] heightNoiseData, float[] canyonNoiseData)
        {
            var plainHeight = config.plainHeight;
            var hillHeight = config.hillHeight;

            var index = i * dataSize * dataSize + j * dataSize + k;

            var biome = canyonNoiseData[index];

            float terrainHeight;
            if (biome < 0.1 && biome > -0.1)
            {
                var ratio = (float)(biome + 0.1f) / 0.2f;
                terrainHeight = plainHeight + (hillHeight - plainHeight) * ratio;
            }
            else if (biome > 0)
            {
                terrainHeight = hillHeight;
            }
            else
            {
                terrainHeight = plainHeight;
            }

            var absY = j + origin.y;
            var height = (1f - absY / (float)terrainHeight) - 0.5f;
            var value = height;
            var n1 = heightNoiseData[index];
            // var n2 = (float)heightNoise2.GetValue(new Vector3(i, j * 0.4f, k) * ) * 0.5f;
            return value + n1;
        }

        bool GenerateGround(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.rockNeedsUpdate)
            {
                return false;
            }

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetOrCreateChunk(origin);

            var genTerrianGPU = new GenTerrianGPU(size, origin, config);
            var voxelBuffer = genTerrianGPU.CreateVoxelBuffer();
            var colorBuffer = genTerrianGPU.CreateColorBuffer();
            genTerrianGPU.Dispatch(voxelBuffer, colorBuffer);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var colors = new Color[colorBuffer.count];
            colorBuffer.GetData(colors);
            chunk.SetColors(colors);

            var dataSize = chunk.dataSize;
            for (var i = 0; i < chunk.dataSize; i++)
            {
                for (var j = 0; j < chunk.dataSize; j++)
                {
                    for (var k = 0; k < chunk.dataSize; k++)
                    {
                        var index = i * dataSize * dataSize + j * dataSize + k;
                        chunk.Set(i, j, k, voxelBufferData[index]);
                        chunk.SetColor(i, j, k, colors[index]);
                    }
                }
            }

            terrianChunk.rockNeedsUpdate = false;

            voxelBuffer.Dispose();
            colorBuffer.Dispose();

            return true;
        }
    }
}