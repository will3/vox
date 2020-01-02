using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;

namespace FarmVox.Workers
{
    public class GenWaterWorker : IWorker
    {
        private readonly TerrianChunk _terrianChunk;
        private readonly Chunks _defaultLayer;
        private readonly Chunks _waterLayer;
        private readonly TerrianConfig _config;
        
        public GenWaterWorker(TerrianChunk terrianChunk, Chunks defaultLayer, Chunks waterLayer, TerrianConfig config)
        {
            _terrianChunk = terrianChunk;
            _defaultLayer = defaultLayer;
            _waterLayer = waterLayer;
            _config = config;
        }
        
        public void Start()
        {
            var origin = _terrianChunk.Origin;
            var chunk = _defaultLayer.GetChunk(origin);
            var waterChunk = _waterLayer.GetOrCreateChunk(_terrianChunk.Origin);
            if (chunk.origin.y < _config.ActualWaterLevel)
            {
                float maxJ = _config.WaterLevel + _config.GroundHeight - chunk.origin.y;
                if (maxJ > chunk.size)
                {
                    maxJ = chunk.size;
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
                                waterChunk.SetColor(i, j, k, _config.Biome.WaterColor);
                            }
                        }
                    }
                }
            }
        }
    }
}