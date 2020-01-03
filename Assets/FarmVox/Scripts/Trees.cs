using System.Linq;
using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;
using Tree = FarmVox.Terrain.Tree;

namespace FarmVox.Scripts
{
    [RequireComponent(typeof(Chunks))]
    public class Trees : MonoBehaviour
    {
        public Chunks chunks;
        public GameObject treePrefab;
        public TreeConfig config;
        public Ground ground;
        public Water water;
        
        private readonly QuadTree<Tree> _treeMap = new QuadTree<Tree>(32);

        public void GenerateTrees(TerrianChunk terrianChunk)
        {
            var groundConfig = ground.config;
            var defaultLayer = ground.chunks;

            var treeNoise = config.noise;

            var pine = new PineObject(3.0f, 10, 2);

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;
                var normal = kv.Value;

                // Cannot be stored in tree map
                if (localCoord.x >= groundConfig.size || localCoord.y >= groundConfig.size ||
                    localCoord.z >= groundConfig.size)
                {
                    continue;
                }

                var j = localCoord.y;

                var globalCoord = localCoord + chunk.origin + new Vector3Int(0, 1, 0);
                var noise = (float) treeNoise.GetValue(globalCoord);
                var treeDensity = config.densityFilter.GetValue(noise);

                if (config.random.NextDouble() * treeDensity > 0.02)
                {
                    continue;
                }

                var absY = j + chunk.origin.y;
                var relY = absY - groundConfig.groundHeight;

                if (absY <= water.config.waterLevel)
                {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                var height = relY / groundConfig.maxHeight;
                var treeHeightValue = config.heightGradient.GetValue(height);

                var value = noise * treeHeightValue;

                if (value < config.threshold)
                {
                    continue;
                }

                var radius = config.minDis;
                var treeBoundsSize = new Vector3Int(radius, radius, radius);
                var treeBounds = new BoundsInt(globalCoord - treeBoundsSize, treeBoundsSize * 2);
                if (_treeMap.Search(treeBounds).Any())
                {
                    continue;
                }

                var tree = pine.Place(chunks, globalCoord, config);

                var treeGo = Instantiate(treePrefab, transform);
                treeGo.transform.position = globalCoord + new Vector3(1.5f, 0, 1.5f);

                _treeMap.Add(globalCoord, tree);
            }

            var treeChunk = chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }
        }
    }
}