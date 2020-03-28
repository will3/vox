using System.Collections;
using System.Linq;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class ChunksMesher : MonoBehaviour
    {
        public Chunks[] chunksToDraw;
        public Waterfalls waterfalls;
        private LightController _lightController;
        private Vector3Int LightDir => _lightController.lightDir.GetDirVector();
        public BoundsInt bounds;
        public bool useBounds;

        private void Start()
        {
            ShadowEvents.Instance.ShadowMapUpdated += OnShadowMapUpdated;
            StartCoroutine(DrawLoop());
        }

        private void OnDestroy()
        {
            ShadowEvents.Instance.ShadowMapUpdated -= OnShadowMapUpdated;
        }

        private void OnShadowMapUpdated(Vector3Int origin, int dataSize, ComputeBuffer[] buffers)
        {
            foreach (var chunks in chunksToDraw)
            {
                var chunk = chunks.GetChunk(origin);
                if (chunk == null)
                {
                    continue;
                }

                chunk.SetLightDir(LightDir);
                chunk.UpdateShadowBuffers(buffers);
                chunk.SetShadowMapSize(dataSize);
            }
        }

        private IEnumerator DrawLoop()
        {
            _lightController = FindObjectOfType<LightController>();
            if (_lightController == null)
            {
                Logger.LogComponentNotFound(typeof(LightController));
            }

            while (true)
            {
                var chunks = chunksToDraw
                    .Select(c => c.ChunkList)
                    .SelectMany(x => x)
                    .Where(c => c.Dirty)
                    .ToArray();

                foreach (var chunk in chunks)
                {
                    DrawChunk(chunk, waterfalls);
                    yield return null;
                }

                yield return null;
            }
        }

        private void DrawChunk(Chunk chunk, Waterfalls waterfalls)
        {
            if (chunk.Mesh != null)
            {
                Destroy(chunk.Mesh);
            }

            chunk.Mesh = MeshGpu(chunk, waterfalls);
            chunk.meshRenderer.material = chunk.Material;
            chunk.meshFilter.sharedMesh = chunk.Mesh;
            chunk.meshCollider.sharedMesh = chunk.Mesh;

            chunk.Dirty = false;

            ShadowEvents.Instance.PublishChunkUpdated(chunk.origin);
        }

        private Mesh MeshGpu(Chunk chunk, Waterfalls waterfalls)
        {
            var options = chunk.options;

            using (var mesher = new MesherGpu(chunk.DataSize, LightDir, bounds, chunk.origin, useBounds)
            {
                AoStrength = options.aoStrength,
                UseNormals = options.useNormals,
                IsWater = options.isWater,
                NormalStrength = options.normalStrength
            })
            {
                mesher.SetData(chunk.data);
                mesher.SetColors(chunk.colors);

                mesher.Dispatch();

                var triangles = mesher.ReadTriangles();

                var builder = new MeshBuilder();
                var meshResult = builder
                    .AddTriangles(triangles, waterfalls.GetWaterfallChunk(chunk.origin))
                    .Build();

                chunk.SetVoxelData(meshResult.VoxelData);

                // Update voxel data buffer
                chunk.Material.SetBuffer("_VoxelData", chunk.GetVoxelDataBuffer());

                return meshResult.Mesh;
            }
        }
    }
}