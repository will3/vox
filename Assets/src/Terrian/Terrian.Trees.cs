using UnityEngine;

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

            var chunk = terrianChunk.Chunk;

            foreach (var kv in chunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                var i = coord.x;
                var j = coord.y;
                var k = coord.z;

                var absJ = j + chunk.Origin.y;
                if (absJ < minTreeJ)
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
                var value = (float)treeNoise.GetValue(globalCoord);

                var otherTrees = terrianChunk.GetOtherTrees(coord);

                value -= otherTrees * 4.0f;

                if (value < 0.4f) { continue; }

                print(pine.GetShape(), coord + chunk.Origin, treeLayer.Chunks, pine.Offset);

                terrianChunk.SetTree(coord, true);
            }

            var treeChunk = treeLayer.Chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }

            terrianChunk.treesNeedsUpdate = false;
        }
    }
}
