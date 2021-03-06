using System.Collections.Generic;
using System.Diagnostics;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FarmVox.Scripts
{
    public class Ground : MonoBehaviour
    {
        public int size = 32;
        public GroundConfig config;
        public Chunks chunks;
        public Water water;
        public Stone stone;
        public Vector3Int numGridsToGenerate = new Vector3Int(3, 2, 3);
        public Vector3Int gridOffset = new Vector3Int(-1, 0, -1);
        public ComputeShader shader;
        private readonly HashSet<Vector3Int> _columns = new HashSet<Vector3Int>();
        private readonly List<Vector3Int> _columnsDirty = new List<Vector3Int>();

        public Vector3 Center
        {
            get
            {
                var center = Bounds.center;
                center.y = 0;
                return center;
            }
        }

        public IEnumerable<Vector3Int> Columns => _columns;

        public BoundsInt Bounds =>
            new BoundsInt
            {
                min = gridOffset * size,
                max = (gridOffset + new Vector3Int(numGridsToGenerate.x, numGridsToGenerate.y, numGridsToGenerate.z)) *
                      size
            };

        private void Awake()
        {
            stone = FindObjectOfType<Stone>();
        }

        private void Update()
        {
            UpdateColumns();

            var stopwatch = Stopwatch.StartNew();

            foreach (var column in _columnsDirty)
            {
                LoadColumn(column);
            }

            if (_columnsDirty.Count > 0)
            {
                Debug.Log($"Generated {_columnsDirty.Count} columns, " +
                          $"took {stopwatch.ElapsedMilliseconds}ms");
            }

            _columnsDirty.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }

        private void UpdateColumns()
        {
            for (var i = 0; i < numGridsToGenerate.x; i++)
            {
                for (var k = 0; k < numGridsToGenerate.z; k++)
                {
                    var column = (gridOffset + new Vector3Int(i, 0, k)) * size;

                    if (_columns.Contains(column))
                    {
                        continue;
                    }

                    _columnsDirty.Add(column);
                }
            }

            _columnsDirty.Sort((a, b) => CalcDistance(a).CompareTo(CalcDistance(b)));
        }

        private float CalcDistance(Vector3Int c)
        {
            return (transform.position - (c + new Vector3(size, 0, size) / 2.0f)).sqrMagnitude;
        }

        private void GenerateChunk(Vector3Int origin)
        {
            var chunk = chunks.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(config.size, origin, config, water.config, stone.config, Bounds, shader);

            using (var results = genTerrianGpu.Dispatch())
            {
                chunk.SetColors(results.ReadColors());
                chunk.SetNormals(results.ReadNormals());
                chunk.SetData(results.ReadData());
            }

            TerrianEvents.Instance.PublishGroundGenerated(origin);
        }

        public void UnloadChunk(Vector3Int origin)
        {
            _columns.Remove(origin);
            chunks.UnloadChunk(origin);
        }

        public bool IsGround(Vector3Int coord)
        {
            return chunks.Get(coord) > 0;
        }

        public Chunk GetChunk(Vector3Int coord)
        {
            return chunks.GetChunk(coord);
        }

        private void LoadColumn(Vector3Int column)
        {
            foreach (var chunk in GetChunks(column))
            {
                GenerateChunk(chunk);
            }

            _columns.Add(column);
            TerrianEvents.Instance.PublishColumnGenerated(column);
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