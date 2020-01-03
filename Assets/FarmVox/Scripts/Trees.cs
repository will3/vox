using System.Linq;
using FarmVox.Objects;
using FarmVox.Terrain;
using FarmVox.Voxel;
using LibNoise;
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

        private ModuleBase _noiseModule;

        private ModuleBase NoiseModule => _noiseModule ?? (_noiseModule = ModuleBuilder.Build(config.noise));

        public void GenerateTrees(TerrianChunk terrianChunk)
        {
            var groundConfig = ground.config;
            var defaultLayer = ground.chunks;

            var pine = new PineObject(3.0f, 10, 2);

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;
                var normal = kv.Value;

                var j = localCoord.y;

                var globalCoord = localCoord + chunk.origin + new Vector3Int(0, 1, 0);
                var noise = (float) NoiseModule.GetValue(globalCoord);

                noise = config.densityFilter.GetValue(noise);

                var absY = j + chunk.origin.y;
                var relY = absY - groundConfig.groundHeight;

                if (absY <= water.config.waterLevel)
                {
                    continue;
                }

                var dot = Vector3.Dot(Vector3.up, normal);
                var directionFactor = Mathf.Clamp01(config.directionFilter.GetValue(dot));

                var height = relY / groundConfig.maxHeight;
                var heightFactor = Mathf.Clamp01(config.heightGradient.GetValue(height));

                var value = noise * heightFactor * directionFactor;

                if (value < config.bias)
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