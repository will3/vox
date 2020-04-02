using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Water : MonoBehaviour
    {
        public Ground ground;
        public Chunks chunks;
        public WaterConfig config;
        public int size = 32;
        public int extraWaterChunks = 1;

        private void Awake()
        {
            TerrianEvents.Instance.ColumnGenerated += OnColumnGenerated;
        }

        private void Start()
        {
            var extra = new Vector3Int(extraWaterChunks, 0, extraWaterChunks);
            var min = ground.gridOffset;
            var max = ground.gridOffset + ground.numGridsToGenerate;
            var waterMin = min - extra;
            var waterMax = max + extra;

            for (var i = waterMin.x; i < waterMax.x; i++)
            {
                for (var j = waterMin.z; j < waterMax.z; j++)
                {
                    if (i >= min.x && i < max.x && j >= min.z && j < max.z)
                    {
                        continue;
                    }

                    var column = new Vector3Int(i, 0, j) * size;
                    var y = Mathf.FloorToInt(config.waterLevel / (float) size) * size;
                    var origin = new Vector3Int(column.x, y, column.z);

                    GenerateChunk(origin);
                }
            }
        }

        private void OnDestroy()
        {
            TerrianEvents.Instance.ColumnGenerated -= OnColumnGenerated;
        }

        private void OnColumnGenerated(Vector3Int column)
        {
            GenerateChunk(column);
        }

        private void GenerateChunk(Vector3Int column)
        {
            var y = Mathf.FloorToInt(config.waterLevel / (float) size) * size;
            var origin = new Vector3Int(column.x, y, column.z);

            var waterChunk = chunks.GetOrCreateChunk(origin);

            var chunk = ground.GetChunk(origin);

            GenerateChunk(origin, waterChunk, chunk);
        }

        private void GenerateChunk(Vector3Int origin, Chunk waterChunk, Chunk chunk)
        {
            float maxJ = config.waterLevel - origin.y;
            if (maxJ > size)
            {
                maxJ = size;
            }

            for (var i = 0; i < waterChunk.DataSize; i++)
            {
                for (var k = 0; k < waterChunk.DataSize; k++)
                {
                    for (var j = 0; j <= maxJ; j++)
                    {
                        if (chunk != null && chunk.Get(i, j, k) > 0)
                        {
                            continue;
                        }

                        waterChunk.Set(i, j, k, 1);
                        waterChunk.SetColor(i, j, k, this.config.waterColor);
                    }
                }
            }
        }

        public void UnloadChunk(Vector3Int chunk)
        {
            chunks.UnloadChunk(chunk);
        }
    }
}