using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class GenTreesWorker : IWorker
    {
        private readonly TerrianConfig _config;
        private readonly TerrianChunk _terrianChunk;
        private readonly Chunks _defaultLayer;
        private readonly Chunks _treeLayer;
        private readonly Terrian _terrian;
        private readonly TreeMap _treeMap;

        public GenTreesWorker(TerrianConfig config, TerrianChunk terrianChunk, Chunks defaultLayer, Chunks treeLayer,
            Terrian terrian, TreeMap treeMap)
        {
            _config = config;
            _terrianChunk = terrianChunk;
            _defaultLayer = defaultLayer;
            _treeLayer = treeLayer;
            _terrian = terrian;
            _treeMap = treeMap;
        }
        
        public void Start()
        {
            var treeNoise = _config.Biome.TreeNoise;

            var pine = new Pine(3.0f, 10, 2);

            var origin = _terrianChunk.Origin;
            var chunk = _defaultLayer.GetChunk(origin);
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;
                var normal = kv.Value;
                
                // Cannot be stored in tree map
                if (localCoord.x >= _config.Size || localCoord.y >= _config.Size || localCoord.z >= _config.Size)
                {
                    continue;
                }
                
                var i = localCoord.x;
                var j = localCoord.y;
                var k = localCoord.z;

                var globalCoord = localCoord + chunk.Origin;
                var noise = (float)treeNoise.GetValue(globalCoord);
                var treeDensity = _config.Biome.TreeDensityFilter.GetValue(noise);

                if (_config.Biome.TreeRandom.NextDouble() * treeDensity > 0.02)
                {
                    continue;
                }

                var relY = j + chunk.Origin.y - _config.GroundHeight;

                if (relY <= _config.WaterLevel) {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                var height = relY / _config.MaxHeight;
                var treeHeightValue = _config.Biome.TreeHeightGradient.GetValue(height);

                var value = noise * treeHeightValue * _config.Biome.TreeAmount;

                if (value < 0.5f) { continue; }

                var radius = _config.TreeMinDis;
                var treeBoundsSize = new Vector3Int(radius, radius, radius);
                var treeBounds = new BoundsInt(globalCoord - treeBoundsSize, treeBoundsSize * 2);
                if (_treeMap.HasTrees(treeBounds))
                {
                    continue;
                }

                var tree = pine.Place(_terrian, _treeLayer, globalCoord, _config);

                _treeMap.AddTree(tree);
            }

            var treeChunk = _treeLayer.GetChunk(_terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }
        }
        
        public float Priority
        {
            get { return Priorities.GenTrees - _terrianChunk.Distance / 1024f; }
        }
    }
}