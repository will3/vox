using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Scripts;
using FarmVox.Terrain.Routing;
using FarmVox.Voxel;
using FarmVox.Workers;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class Terrian : MonoBehaviour
    {
        public TerrianConfig Config;

        private int Size => Config.Size;

        public Chunks defaultLayer;
        public Chunks waterLayer;
        public Trees trees;
        public Waterfalls waterfalls;

        public RouteChunks Routes { get; private set; }

        private readonly Dictionary<Vector3Int, TerrianChunk> _map = new Dictionary<Vector3Int, TerrianChunk>();
        private readonly HashSet<Vector3Int> _columnGenerated = new HashSet<Vector3Int>();
        private readonly HashSet<Vector3Int> _waterfallGenerated = new HashSet<Vector3Int>();

        private string _lastConfig;

        public Vector3Int numGridsToGenerate = new Vector3Int(2, 3, 2);

        private void Awake()
        {
            Routes = new RouteChunks(Config.Size);
        }

        private IEnumerator Start()
        {
            for (var i = -numGridsToGenerate.x; i < numGridsToGenerate.x; i++)
            {
                for (var k = -numGridsToGenerate.z; k < numGridsToGenerate.z; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * Size;

                    if (_columnGenerated.Contains(columnOrigin))
                    {
                        continue;
                    }

                    GenerateColumn(columnOrigin);
                    yield return null;

                    _columnGenerated.Add(columnOrigin);
                }
            }

            foreach (var column in _columnGenerated.Where(ShouldGenerateWaterfall))
            {
                foreach (var chunk in GetChunks(column))
                {
                    waterfalls.GenerateWaterfalls(chunk);
                }

                _waterfallGenerated.Add(column);
            }
        }

        private bool ShouldGenerateWaterfall(Vector3Int origin)
        {
            if (_waterfallGenerated.Contains(origin))
            {
                return false;
            }

            for (var i = -1; i <= 1; i++)
            {
                for (var k = -1; k <= 1; k++)
                {
                    if (i == 0 && k == 0)
                    {
                        continue;
                    }

                    var nextOrigin = origin + new Vector3Int(i, 0, k) * Size;

                    if (!_columnGenerated.Contains(nextOrigin))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private IEnumerable<TerrianChunk> GetChunks(Vector3Int columnOrigin)
        {
            for (var i = 0; i < numGridsToGenerate.y; i++)
            {
                var origin = new Vector3Int(columnOrigin.x, Size * i, columnOrigin.z);
                var chunk = GetOrCreateTerrianChunk(origin);
                yield return chunk;
            }
        }

        private void GenerateColumn(Vector3Int origin)
        {
            foreach (var chunk in GetChunks(origin))
            {
                new GenGroundWorker(chunk, defaultLayer, Config).Start();
                trees.GenerateTrees(this, chunk);
                new GenWaterWorker(chunk, defaultLayer, waterLayer, Config).Start();
                new GenRoutesWorker(chunk.Origin, Routes, defaultLayer).Start();
            }
        }

        public TerrianChunk GetTerrianChunk(Vector3Int origin)
        {
            return _map.TryGetValue(origin, out var terrianChunk) ? terrianChunk : null;
        }

        private TerrianChunk GetOrCreateTerrianChunk(Vector3Int origin)
        {
            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            var key = new Vector3Int(origin.x / Size, origin.y / Size, origin.z / Size);
            _map[origin] = new TerrianChunk(key, Size);
            return _map[origin];
        }

        public bool IsGround(Vector3Int coord)
        {
            return defaultLayer.Get(coord) > 0;
        }
    }
}