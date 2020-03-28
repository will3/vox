using System.Collections;
using System.Collections.Generic;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Ground : MonoBehaviour
    {
        public int size = 32;
        public GroundConfig config;
        public Chunks chunks;
        public Water water;
        public Stone stone;
        public Vector3Int numGridsToGenerate = new Vector3Int(1, 2, 1);

        public Vector3 Center =>
            new Vector3(0.5f, 0, 0.5f) * size;

        private readonly HashSet<Vector3Int> _columns = new HashSet<Vector3Int>();
        private readonly List<Vector3Int> _columnsDirty = new List<Vector3Int>();

        public IEnumerable<Vector3Int> Columns => _columns;

        public BoundsInt Bounds =>
            new BoundsInt
            {
                min = new Vector3Int(-numGridsToGenerate.x, 0, -numGridsToGenerate.z) * size,
                max = new Vector3Int(numGridsToGenerate.x + 1, 0, numGridsToGenerate.z + 1) * size
            };

        private IEnumerator Start()
        {
            while (true)
            {
                UpdateColumns();

                foreach (var column in _columnsDirty)
                {
                    LoadColumn(column);
                    yield return null;
                }

                _columnsDirty.Clear();
                yield return null;
            }

            // ReSharper disable once IteratorNeverReturns
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

        public void GenerateChunk(Vector3Int origin)
        {
            var chunk = chunks.GetOrCreateChunk(origin);

            if (stone == null)
            {
                stone = FindObjectOfType<Stone>();
            }

            var genTerrianGpu = new GenTerrianGpu(config.size, origin, config, water.config, stone.config);

            var voxelBuffer = genTerrianGpu.CreateVoxelBuffer();
            var colorBuffer = genTerrianGpu.CreateColorBuffer();

            genTerrianGpu.Dispatch(voxelBuffer, colorBuffer);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var colorBufferData = new Color[colorBuffer.count];
            colorBuffer.GetData(colorBufferData);

            chunk.SetColors(colorBufferData);
            chunk.SetData(voxelBufferData);

            voxelBuffer.Dispose();
            colorBuffer.Dispose();

            TerrianEvents.Instance.PublishGroundGenerated(origin);
        }

        public void UnloadChunk(Vector3Int origin)
        {
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