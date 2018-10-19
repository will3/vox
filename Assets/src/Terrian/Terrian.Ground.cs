using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        float GetValue(int i, int j, int k, int dataSize, Vector3Int origin, float[] heightNoiseData, float[] canyonNoiseData)
        {
            var plainHeight = config.PlainHeight;
            var hillHeight = config.HillHeight;

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
            return value + n1;
        }

        private void GenerateGround(TerrianColumn column) {
            if (column.generatedGround) {
                return;
            }

            foreach (var terrianChunk in column.TerrianChunks) {
                GenerateGround(terrianChunk);

                heightMap.LoadChunk(this, terrianChunk);
            }

            column.generatedGround = true;
        }

        private void GenerateGround(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.rockNeedsUpdate)
            {
                return;
            }

            var origin = terrianChunk.Origin;
            var chunk = DefaultLayer.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(Size, origin, config);

            var voxelBuffer = genTerrianGpu.CreateVoxelBuffer();
            var colorBuffer = genTerrianGpu.CreateColorBuffer();
            var typeBuffer = genTerrianGpu.CreateTypeBuffer();

            genTerrianGpu.Dispatch(voxelBuffer, colorBuffer, typeBuffer, terrianChunk);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var typeBufferData = new int[typeBuffer.count];
            typeBuffer.GetData(typeBufferData);

            var colorBufferData = new Color[colorBuffer.count];
            colorBuffer.GetData(colorBufferData);

            chunk.SetColors(colorBufferData);
            chunk.SetData(voxelBufferData);
            chunk.SetTypes(typeBufferData);

            terrianChunk.rockNeedsUpdate = false;

            voxelBuffer.Dispose();
            typeBuffer.Dispose();
            colorBuffer.Dispose();
        }
    }
}
