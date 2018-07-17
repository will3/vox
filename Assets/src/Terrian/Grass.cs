using UnityEngine;
using System.Collections;
using LibNoise.Generator;

namespace FarmVox
{
    public static class Grass
    {
        public static void Generate(TerrianChunk terrianChunk, TerrianConfig config)
        {
            var grassNoise = config.grassNoise;

            if (!terrianChunk.grassNeedsUpdate)
            {
                return;
            }

            var chunk = terrianChunk.Chunk;

            foreach (var kv in chunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                if (terrianChunk.GetWater(coord.x, coord.y, coord.z))
                {
                    continue;
                }

                var upDot = Vector3.Dot(Vector3.up, normal);

                Vector3 globalCoord = coord + chunk.Origin;

                var n = (float)grassNoise.GetValue(globalCoord * 0.05f) * 0.1f;

                var value = upDot + n;

                if (value < 0.0f) {
                    continue;
                }

                value = Mathf.Clamp(value, 0.0f, 1.0f);

                var v = config.grassCurve.GetValue(value);
                var c = Colors.grassGradient.GetValue(v);
                chunk.SetColor(coord.x, coord.y, coord.z, c);
            }

            terrianChunk.grassNeedsUpdate = false;
        }
    }
}
