using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        private void GenerateTrees(TerrianColumn column)
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
            var treeNoise = config.TreeNoise;

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
                var treeDensity = config.TreeDensityFilter.GetValue(noise);

                if (config.TreeRandom.NextDouble() * treeDensity > 0.02)
                {
                    continue;
                }

                var type = chunk.GetType(localCoord);
                if (type == VoxelType.Stone) {
                    continue;
                }

                var relY = j + chunk.Origin.y - config.GroundHeight;

                if (relY <= config.WaterLevel) {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                var height = relY / config.MaxHeight;
                var treeHeightValue = config.TreeHeightGradient.GetValue(height);

                var value = noise * treeHeightValue * config.TreeAmount;

                if (value < 0.5f) { continue; }

                var radius = config.TreeMinDis;
                var treeBoundsSize = new Vector3Int(radius, radius, radius);
                var treeBounds = new BoundsInt(globalCoord - treeBoundsSize, treeBoundsSize * 2);
                if (TreeMap.HasTrees(treeBounds))
                {
                    continue;
                }

                var tree = pine.Place(this, TreeLayer, globalCoord, config);

                TreeMap.AddTree(tree);
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
