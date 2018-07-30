using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        void GenerateTrees(TerrianColumn column)
        {
            if (column.generatedTrees)
            {
                return;
            }

            foreach (var terrianChunk in column.TerrianChunks)
            {
                GenerateTrees(terrianChunk);
            }

            column.generatedTrees = true;
        }


        void GenerateTrees(TerrianChunk terrianChunk)
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
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;
                var normal = kv.Value;
                var i = localCoord.x;
                var j = localCoord.y;
                var k = localCoord.z;

                Vector3Int globalCoord = localCoord + chunk.Origin;
                var noise = (float)treeNoise.GetValue(globalCoord);
                var treeDensity = config.treeDensityFilter.GetValue(noise);

                if (config.treeRandom.NextDouble() * treeDensity > 0.1)
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

                var height = (absY - config.groundHeight) / config.maxHeight;
                var treeHeightValue = config.treeHeightGradient.GetValue(height);

                var value = noise * treeHeightValue * config.treeAmount;

                if (value < 0.5f) { continue; }

                var treeBoundsSize = 5.0f;
                var treeBounds = new Bounds(globalCoord, new Vector3(treeBoundsSize, treeBoundsSize, treeBoundsSize));
                if (treeMap.HasTrees(treeBounds))
                {
                    continue;
                }

                var tree = pine.Place(this, treeLayer, globalCoord, config);

                treeMap.AddTree(tree);

                if (!treeMap.HasTrees(treeBounds)) {
                    throw new System.Exception("Cant find tree just added");
                }
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
