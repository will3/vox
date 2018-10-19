using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        public void GenerateWaters(TerrianColumn column) 
        {
            if (column.generatedWater) {
                return;
            }

            foreach (var terrianChunk in column.TerrianChunks) {
                GenerateWaters(terrianChunk);
            }

            column.generatedWater = true;
        }

        public void GenerateWaters(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.waterNeedsUpdate)
            {
                return;
            }

            var origin = terrianChunk.Origin;
            var chunk = DefaultLayer.GetChunk(origin);
            var waterChunk = WaterLayer.GetOrCreateChunk(terrianChunk.Origin);
            if (chunk.Origin.y < config.ActualWaterLevel)
            {
                float maxJ = config.WaterLevel + config.GroundHeight - chunk.Origin.y;
                if (maxJ > chunk.Size)
                {
                    maxJ = chunk.Size;
                }
                for (var i = 0; i < chunk.DataSize; i++)
                {
                    for (var k = 0; k < chunk.DataSize; k++)
                    {
                        for (var j = 0; j < maxJ; j++)
                        {
                            if (chunk.Get(i, j, k) <= 0)
                            {
                                waterChunk.Set(i, j, k, 1);
                                waterChunk.SetColor(i, j, k, config.Colors.WaterColor);
                                terrianChunk.SetWater(i, j, k, true);
                            }
                        }
                    }
                }
            }

            terrianChunk.waterNeedsUpdate = false;
        }
    }
}
