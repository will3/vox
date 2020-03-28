using System;
using System.Linq;
using FarmVox.Objects;
using FarmVox.Scripts.Voxel;
using FarmVox.Terrain;
using FarmVox.Voxel;
using LibNoise;
using UnityEngine;

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

        private readonly QuadTree<GameObject> _treeMap = new QuadTree<GameObject>(32);

        private ModuleBase _noiseModule;

        private ModuleBase NoiseModule => _noiseModule ?? (_noiseModule = ModuleBuilder.Build(config.noise));

        private void Start()
        {
            TerrianEvents.Instance.GroundGenerated += OnGroundGenerated;
        }

        private void OnDestroy()
        {
            TerrianEvents.Instance.GroundGenerated -= OnGroundGenerated;
        }

        private void OnGroundGenerated(Vector3Int origin)
        {
            GenerateChunk(origin);
        }

        private void GenerateChunk(Vector3Int origin)
        {
            var groundConfig = ground.config;
            var defaultLayer = ground.chunks;

            var pine = new PineObject(config.treeRadius, config.treeHeight, config.trunkHeight);

            var chunk = defaultLayer.GetChunk(origin);
            chunk.UpdateNormals();

            foreach (var kv in chunk.Normals)
            {
                var localCoord = kv.Key;

                if (!chunk.IsInBound(localCoord))
                {
                    continue;
                }

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

                pine.Place(chunks, globalCoord - pine.Pivot, config);

                var treeGo = Instantiate(treePrefab, transform);
                treeGo.transform.position = globalCoord + new Vector3(1.5f, 0, 1.5f);

                _treeMap.Add(globalCoord, treeGo);
            }

            var treeChunk = chunks.GetChunk(origin);
            if (treeChunk != null)
            {
                treeChunk.UpdateSurfaceCoords();
            }
        }

        public void UnloadChunk(Vector3Int origin)
        {
            chunks.UnloadChunk(origin);
            var trees = _treeMap.UnloadChunk(origin);
            foreach (var tree in trees)
            {
                Destroy(tree);
            }
        }
    }
}