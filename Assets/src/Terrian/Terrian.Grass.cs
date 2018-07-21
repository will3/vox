using UnityEngine;
using System.Collections;
using LibNoise.Generator;

namespace FarmVox
{
    partial class Terrian {
        private void GenerateGrass(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.grassNeedsUpdate)
            {
                return;
            }

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);
            var grassNoise = new Perlin3DGPU(config.grassNoise, chunk.dataSize, origin);
            grassNoise.Dispatch();
            var grassNoiseData = grassNoise.Read();
            grassNoise.Dispose();

            foreach (var kv in chunk.Normals)
            {
                var coord = kv.Key;
                var voxelNormal = kv.Value;
                if (terrianChunk.GetWater(coord.x, coord.y, coord.z))
                {
                    continue;
                }

                var normal = Vector3.Dot(Vector3.up, voxelNormal);
                var normalValue = config.grassNormalFilter.GetValue(normal);

                if (normalValue <= 0) {
                    continue;
                }

                var index = chunk.GetIndex(coord);
                var grassNoiseValue = (float)grassNoiseData[index];
                var absY = coord.y + chunk.Origin.y;
                var height = absY / config.maxHeight;
                var heightValue = config.grassHeightFilter.GetValue(height);
                                        
                if (heightValue <= 0f)
                {
                    continue;
                }

                var value = normalValue * heightValue * config.grassMultiplier + grassNoiseValue + config.grassOffset;

                if (value < 0) {
                    continue;
                }

                var c = Colors.grassGradient.GetValue(value);

                chunk.SetColor(coord.x, coord.y, coord.z, c);
            }

            terrianChunk.grassNeedsUpdate = false;
        }
    }
}
