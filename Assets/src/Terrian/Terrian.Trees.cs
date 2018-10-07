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
            var treeNoise = config.treeNoise;

            if (!terrianChunk.treesNeedsUpdate)
            {
                return;
            }

            var pine = new Pine(3.0f, 10, 2);
            //var pine = new Maple(7.0f, 2);

            var origin = terrianChunk.Origin;
            var chunk = DefaultLayer.GetChunk(origin);
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

                if (config.treeRandom.NextDouble() * treeDensity > 0.02)
                {
                    continue;
                }

                var type = chunk.GetType(localCoord);
                if (type == VoxelType.Stone) {
                    continue;
                }

                var relY = j + chunk.Origin.y - config.groundHeight;

                if (relY <= config.waterLevel) {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                var height = relY / config.maxHeight;
                var treeHeightValue = config.treeHeightGradient.GetValue(height);

                var value = noise * treeHeightValue * config.treeAmount;

                if (value < 0.5f) { continue; }

                var radius = 3;
                var treeBoundsSize = new Vector3Int(radius, radius, radius);
                var treeBounds = new BoundsInt(globalCoord - treeBoundsSize, treeBoundsSize * 2);
                if (treeMap.HasTrees(treeBounds))
                {
                    continue;
                }

                var tree = pine.Place(this, TreeLayer, globalCoord, config);

                treeMap.AddTree(tree);
            }

            var treeChunk = TreeLayer.GetChunk(terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }

            terrianChunk.treesNeedsUpdate = false;
        }
    }
}
