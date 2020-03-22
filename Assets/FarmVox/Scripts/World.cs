using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class World : MonoBehaviour
    {
        public int size = 32;
        public Ground ground;
        public Water water;
        public Trees trees;
        public Waterfalls waterfalls;
        public BuildingTiles tiles;

        public float waitForSeconds = 0.1f;

        private readonly HashSet<Vector3Int> _columns = new HashSet<Vector3Int>();
        private readonly List<Vector3Int> _columnsDirty = new List<Vector3Int>();
        private readonly HashSet<Vector3Int> _waterfallGenerated = new HashSet<Vector3Int>();
        private readonly HashSet<Vector3Int> _columnsToUnload = new HashSet<Vector3Int>();

        private string _lastConfig;

        public Vector3Int numGridsToGenerate = new Vector3Int(1, 2, 1);
        public int distanceToUnload = 1;

        public BoundsInt Bounds =>
            new BoundsInt
            {
                min = new Vector3Int(-numGridsToGenerate.x, 0, -numGridsToGenerate.z) * size,
                max = new Vector3Int(numGridsToGenerate.x + 1, 0, numGridsToGenerate.z + 1) * size
            };

        public Vector3 Center =>
            new Vector3(0.5f, 0, 0.5f) * size;

        private void Start()
        {
            StartCoroutine(GenerateWorldLoop());
            StartCoroutine(UnloadColumnsLoop());
        }

        private IEnumerator GenerateWorldLoop()
        {
            while (true)
            {
                UpdateColumns();

                foreach (var column in _columnsDirty)
                {
                    yield return LoadColumn(column);
                }

                _columnsDirty.Clear();

                var waterfallColumns = _columns.Where(ShouldGenerateWaterfall);
                foreach (var column in waterfallColumns)
                {
                    foreach (var chunk in GetChunks(column))
                    {
                        waterfalls.GenerateChunk(chunk);
                    }

                    _waterfallGenerated.Add(column);

                    yield return new WaitForSeconds(waitForSeconds);
                }

                yield return new WaitForSeconds(waitForSeconds);
            }
        }

        private IEnumerator UnloadColumnsLoop()
        {
            while (true)
            {
                UpdateColumnsToUnload();
                yield return UnloadColumns();

                yield return new WaitForSeconds(waitForSeconds);
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Return)) return;
            foreach (var column in _columns)
            {
                _columnsToUnload.Add(column);
            }
        }

        private void UpdateColumns()
        {
            for (var i = -numGridsToGenerate.x; i <= numGridsToGenerate.x; i++)
            {
                for (var k = -numGridsToGenerate.z; k <= numGridsToGenerate.z; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * size;

                    if (_columns.Contains(columnOrigin))
                    {
                        continue;
                    }

                    _columnsDirty.Add(columnOrigin);
                }
            }

            _columnsDirty.Sort((a, b) => CalcDistance(a).CompareTo(CalcDistance(b)));
        }

        private float CalcDistance(Vector3Int c)
        {
            return (transform.position - (c + new Vector3(size, 0, size) / 2.0f)).sqrMagnitude;
        }

        private IEnumerator LoadColumn(Vector3Int column)
        {
            foreach (var chunk in GetChunks(column))
            {
                ground.GenerateChunk(chunk);
                trees.GenerateChunk(chunk);
                water.GenerateChunk(chunk);
            }

            _columns.Add(column);

            yield return new WaitForSeconds(waitForSeconds);
        }

        private void UpdateColumnsToUnload()
        {
            var playerOrigin = transform.position.GetOrigin(size);
            playerOrigin.y = 0;

            var columnsToUnload = _columns.Where(column =>
            {
                var xDis = Mathf.Abs((playerOrigin.x - column.x) / size);
                var zDis = Mathf.Abs((playerOrigin.z - column.z) / size);
                return Mathf.Max(xDis, zDis) > distanceToUnload;
            }).ToArray();

            _columnsToUnload.UnionWith(columnsToUnload);
        }

        private IEnumerator UnloadColumns()
        {
            var copy = new HashSet<Vector3Int>(_columnsToUnload);
            foreach (var column in copy)
            {
                yield return UnloadColumn(column);
            }

            _columnsToUnload.ExceptWith(copy);
        }

        private IEnumerator UnloadColumn(Vector3Int column)
        {
            foreach (var chunk in GetChunks(column))
            {
                ground.UnloadChunk(chunk);
                trees.UnloadChunk(chunk);
                water.UnloadChunk(chunk);
                waterfalls.UnloadChunk(chunk);
                tiles.UnloadChunk(chunk);

                yield return new WaitForSeconds(waitForSeconds);
            }

            _columns.Remove(column);
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

                    if (!_columns.Contains(nextOrigin))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private IEnumerable<Vector3Int> GetChunks(Vector3Int columnOrigin)
        {
            for (var i = 0; i < numGridsToGenerate.y; i++)
            {
                yield return new Vector3Int(columnOrigin.x, size * i, columnOrigin.z);
            }
        }
    }
}