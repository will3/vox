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
            if (!_terrianChunk.waterNeedsUpdate)
            {
                return;
            }

            var origin = _terrianChunk.Origin;
            var chunk = _defaultLayer.GetChunk(origin);
            var waterChunk = _waterLayer.GetOrCreateChunk(_terrianChunk.Origin);
            if (chunk.Origin.y < _config.ActualWaterLevel)
            {
                float maxJ = _config.WaterLevel + _config.GroundHeight - chunk.Origin.y;
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
                                waterChunk.SetColor(i, j, k, _config.Colors.WaterColor);
                                _terrianChunk.SetWater(i, j, k, true);
                            }
                        }
                    }
                }
            }

            _terrianChunk.waterNeedsUpdate = false;
        }
        
        public float Priority
        {
            get { return Priorities.GenWater - _terrianChunk.Distance / 1024f; }
        }
    }
}