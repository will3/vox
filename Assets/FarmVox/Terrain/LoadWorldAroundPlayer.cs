using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Scripts;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class LoadWorldAroundPlayer : MonoBehaviour
    {
        public int size = 32;
        public Ground ground;
        public Water water;
        public Trees trees;
        public Waterfalls waterfalls;
        public float waitForSeconds = 0.2f;

        private readonly Dictionary<Vector3Int, TerrianChunk> _map = new Dictionary<Vector3Int, TerrianChunk>();
        private readonly HashSet<Vector3Int> _columnGenerated = new HashSet<Vector3Int>();
        private readonly HashSet<Vector3Int> _waterfallGenerated = new HashSet<Vector3Int>();

        private string _lastConfig;

        public Vector3Int numGridsToGenerate = new Vector3Int(2, 3, 2);

        private IEnumerator Start()
        {
            while (true)
            {
                var playerOrigin = transform.position.GetOrigin(size);
                playerOrigin.y = 0;

                var columns = new List<Vector3Int>();
                for (var i = -numGridsToGenerate.x; i <= numGridsToGenerate.x; i++)
                {
                    for (var k = -numGridsToGenerate.z; k <= numGridsToGenerate.z; k++)
                    {
                        var columnOrigin = playerOrigin + new Vector3Int(i, 0, k) * size;

                        if (_columnGenerated.Contains(columnOrigin))
                        {
                            continue;
                        }

                        columns.Add(columnOrigin);
                    }
                }

                foreach (var column in columns.OrderBy(c =>
                    (transform.position - (c + new Vector3(size, 0, size) / 2.0f)).sqrMagnitude))
                {
                    foreach (var chunk in GetChunks(column))
                    {
                        ground.GenerateChunk(chunk);
                        trees.GenerateTrees(chunk);
                        water.GenerateChunk(chunk.Origin);

                        yield return new WaitForSeconds(waitForSeconds);
                    }

                    _columnGenerated.Add(column);
                }

                foreach (var column in _columnGenerated.Where(ShouldGenerateWaterfall))
                {
                    foreach (var chunk in GetChunks(column))
                    {
                        waterfalls.GenerateWaterfalls(chunk);
                    }

                    _waterfallGenerated.Add(column);

                    yield return new WaitForSeconds(waitForSeconds);
                }

                yield return new WaitForSeconds(waitForSeconds);
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

                    var nextOrigin = origin + new Vector3Int(i, 0, k) * size;

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
                var origin = new Vector3Int(columnOrigin.x, size * i, columnOrigin.z);
                var chunk = GetOrCreateTerrianChunk(origin);
                yield return chunk;
            }
        }

        private TerrianChunk GetOrCreateTerrianChunk(Vector3Int origin)
        {
            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            var key = new Vector3Int(origin.x / size, origin.y / size, origin.z / size);
            _map[origin] = new TerrianChunk(key, size);
            return _map[origin];
        }
    }
}