using System;
using FarmVox.Scripts.Voxel;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class WaterConfig
    {
        public Color waterColor = ColorUtils.GetColor("#297eb6", 0.4f);
        public int waterLevel = 12;
    }

    public class Water : MonoBehaviour
    {
        public Ground ground;
        public Chunks chunks;
        public WaterConfig config;

        private void Start()
        {
            TerrianEvents.Instance.GroundGenerated += OnGroundGenerated;
        }

        private void OnDestroy()
        {
            TerrianEvents.Instance.GroundGenerated -= OnGroundGenerated;
        }

        private void OnGroundGenerated(Vector3Int origin)
        {
            GenerateChunk(origin);
        }

        private void GenerateChunk(Vector3Int origin)
        {
            var waterChunk = chunks.GetOrCreateChunk(origin);

            if (origin.y >= config.waterLevel)
            {
                return;
            }

            var chunk = ground.GetChunk(origin);

            float maxJ = config.waterLevel - chunk.origin.y;
            if (maxJ > chunk.size)
            {
                maxJ = chunk.size;
            }

            for (var i = 0; i < waterChunk.DataSize; i++)
            {
                for (var k = 0; k < waterChunk.DataSize; k++)
                {
                    for (var j = 0; j < maxJ; j++)
                    {
                        if (chunk.Get(i, j, k) > 0)
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