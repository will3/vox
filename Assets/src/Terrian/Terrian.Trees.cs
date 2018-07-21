﻿using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        private void GenerateTrees(TerrianChunk terrianChunk)
        {
            var minTreeJ = config.minTreeJ;
            var treeNoise = config.treeNoise;

            if (!terrianChunk.treesNeedsUpdate)
            {
                return;
            }

            var pine = new Pine(3.0f, 10, 2);

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);

            foreach (var kv in chunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                var i = coord.x;
                var j = coord.y;
                var k = coord.z;

                if (config.treeRandom.NextDouble() > 0.1)
                {
                    continue;
                }

                var absY = j + chunk.Origin.y;
                if (absY < minTreeJ)
                {
                    continue;
                }

                if (terrianChunk.GetWater(i, j, k))
                {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                Vector3 globalCoord = coord + chunk.Origin;
                var noise = (float)treeNoise.GetValue(globalCoord);
                var otherTrees = terrianChunk.GetOtherTrees(coord);

                var height = absY / config.maxHeight;
                var treeHeightValue = config.treeHeightGradient.GetValue(height);

                var value = noise * treeHeightValue * config.treeAmount - otherTrees * config.treeSparse;

                if (value < 0.5f) { continue; }

                print(pine.GetShape(), coord + chunk.Origin, treeLayer, pine.Offset);

                terrianChunk.SetTree(coord, true);
            }

            var treeChunk = treeLayer.GetChunk(terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }

            terrianChunk.treesNeedsUpdate = false;
        }
    }
}