using UnityEngine;

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

        void GenerateGround(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.rockNeedsUpdate)
            {
                return;
            }

            if (terrianChunk.Chunk == null)
            {
                terrianChunk.Chunk = defaultLayer.Chunks.GetOrCreateChunk(terrianChunk.Origin);
            }
            var chunk = terrianChunk.Chunk;
            var origin = chunk.Origin;

            var genTerrianGPU = new GenTerrianGPU(size, origin, config);
            var voxelBuffer = genTerrianGPU.CreateVoxelBuffer();
            var colorBuffer = genTerrianGPU.CreateColorBuffer();
            genTerrianGPU.Dispatch(voxelBuffer, colorBuffer);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var heightNoise = new Perlin3DGPU(config.heightNoise, chunk.dataSize, origin);
            var canyonNoise = new Perlin3DGPU(config.canyonNoise, chunk.dataSize, origin);
            heightNoise.Dispatch();
            canyonNoise.Dispatch();

            var heightNoiseData = heightNoise.Read();
            var canyonNoiseData = canyonNoise.Read();
            heightNoise.Dispose();
            canyonNoise.Dispose();

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
                        float value = GetValue(i, j, k, dataSize, origin, heightNoiseData, canyonNoiseData);
                        chunk.Set(i, j, k, value);
                        //if (value > 0)
                        //{
                        //   
                        //}
                        if (Mathf.Approximately(voxelBufferData[index], value)) {
                            throw new System.Exception("value not");
                        }
                        if (colors[index] != Colors.rock) {
                            throw new System.Exception("oh no");
                        }
                        chunk.SetColor(i, j, k, colors[index]);
                        //chunk.Colors[index] = colors[index];
                    }
                }
            }

            terrianChunk.rockNeedsUpdate = false;
        }
    }
}
