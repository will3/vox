using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Water : MonoBehaviour
    {
        public Ground ground;
        public Chunks chunks;
        public Terrian terrian;

        public void GenerateChunk(Vector3Int origin)
        {
            var config = terrian.Config;
            var waterChunk = chunks.GetOrCreateChunk(origin);

            if (origin.y >= config.ActualWaterLevel)
            {
                return;
            }

            var chunk = ground.GetChunk(origin);
            
            float maxJ = terrian.Config.ActualWaterLevel - chunk.origin.y;
            if (maxJ > chunk.size)
            {
                maxJ = chunk.size;
            }

            for (var i = 0; i < waterChunk.DataSize; i++)
            {
                for (var k = 0; k < waterChunk.DataSize; k++)
                {
                    for (var j = 0; j < maxJ; j++)
                    {
                        if (chunk.Get(i, j, k) > 0)
                        {
                            continue;
                        }

                        waterChunk.Set(i, j, k, 1);
                        waterChunk.SetColor(i, j, k, config.Biome.WaterColor);
                    }
                }
            }
        }
    }
}