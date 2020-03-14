using System.Collections;
using System.Diagnostics;
using System.Linq;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using FarmVox.Voxel;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FarmVox.Scripts
{
    public class ChunksMesher : MonoBehaviour
    {
        public float aoStrength = 0.15f;
        public Chunks[] chunksToDraw;
        public VoxelShadowMap shadowMap;
        public Waterfalls waterfalls;
        public float waitForSeconds = 0.2f;
        private LightDirection _lightDirection;
        private Vector3Int _lightDir;

        private IEnumerator Start()
        {
            _lightDirection = FindObjectOfType<LightDirection>();
            if (_lightDirection == null)
            {
                Debug.LogError($"Cannot find component of type {typeof(LightDirection)}");
            }

            _lightDir = _lightDirection.lightDir;

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
                    yield return new WaitForSeconds(waitForSeconds);
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

            shadowMap.ChunkDrawn(chunk.origin);
        }

        private Mesh MeshGpu(Chunk chunk, Waterfalls waterfalls)
        {
            var chunks = chunk.Chunks;

            var mesherSettings = new MesherSettings
            {
                AoStrength = aoStrength
            };

            using (var mesher = new MesherGpu(chunk.DataSize, mesherSettings, _lightDir))
            {
                mesher.UseNormals = chunks.useNormals;
                mesher.IsWater = chunks.isWater;
                mesher.NormalStrength = chunk.Chunks.normalStrength;

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