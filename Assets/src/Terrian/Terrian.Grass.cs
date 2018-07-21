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

            var chunk = terrianChunk.Chunk;
            var origin = terrianChunk.Origin;
            var grassNoise = new Perlin3DGPU(config.grassNoise, chunk.dataSize, origin);
            grassNoise.Dispatch();
            var grassNoiseData = grassNoise.Read();
            grassNoise.Dispose();

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

                var index = chunk.GetIndex(coord);
                var grassNoiseValue = grassNoiseData[index];

                var value = upDot + grassNoiseValue * 0.2f;

                if (value < 0.4f)
                {
                    continue;
                }

                var v = config.grassCurve.GetValue(value);
                var c = Colors.grassGradient.GetValue(v);

                chunk.SetColor(coord.x, coord.y, coord.z, c);
            }

            terrianChunk.grassNeedsUpdate = false;
        }
    }
}
