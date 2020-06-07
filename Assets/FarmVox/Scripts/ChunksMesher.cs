using System.Collections.Generic;
using System.Diagnostics;
using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FarmVox.Scripts
{
    public class ChunksMesher : MonoBehaviour
    {
        public Chunks[] chunksToDraw;
        public Ground ground;
        public Waterfalls waterfalls;
        public Water water;
        public bool logMeshing;
        private LightController _lightController;
        private Vector3Int LightDir => _lightController.lightDir.GetDirVector();
        private static readonly int VoxelData = Shader.PropertyToID("_VoxelData");

        private void Awake()
        {
            ShadowEvents.Instance.ShadowMapUpdated += OnShadowMapUpdated;
        }

        private void Start()
        {
            water = FindObjectOfType<Water>();
            ground = FindObjectOfType<Ground>();

            _lightController = FindObjectOfType<LightController>();
            if (_lightController == null)
            {
                Logger.LogComponentNotFound(typeof(LightController));
            }
        }

        private void OnDestroy()
        {
            ShadowEvents.Instance.ShadowMapUpdated -= OnShadowMapUpdated;
        }

        private void OnShadowMapUpdated(Vector3Int origin)
        {
            foreach (var chunks in chunksToDraw)
            {
                var chunk = chunks.GetChunk(origin);
                if (chunk == null)
                {
                    continue;
                }

                chunk.SetLightDir(LightDir);
                chunk.UpdateShadowBuffers();
            }
        }

        private void Update()
        {
            var chunksCount = 0;
            var verticesCount = 0;
            var trianglesCount = 0;

            var stopwatch = Stopwatch.StartNew();

            foreach (var chunks in chunksToDraw)
            {
                foreach (var chunk in chunks.ChunkList)
                {
                    if (!chunk.dirty)
                    {
                        continue;
                    }

                    DrawChunk(chunk);

                    chunksCount++;
                    var mesh = chunk.meshFilter.mesh;
                    verticesCount += mesh.vertexCount;
                    trianglesCount += mesh.triangles.Length;
                }
            }

            if (chunksCount > 0)
            {
                Debug.Log($"Meshed {chunksCount} chunks, " +
                          $"{verticesCount} vertices, " +
                          $"{trianglesCount} triangles, " +
                          $"took {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private void DrawChunk(Chunk chunk)
        {
            if (chunk.hasMesh)
            {
                Destroy(chunk.meshFilter.sharedMesh);
            }

            var meshResult = CalcMesh(chunk);

            chunk.SetVoxelData(meshResult.VoxelData);

            // Update voxel data buffer
            chunk.Material.SetBuffer(VoxelData, chunk.GetVoxelDataBuffer());

            var mesh = meshResult.Mesh;

            if (logMeshing)
            {
                Debug.Log($"Meshed {chunk.origin} {mesh.triangles.Length} triangles {mesh.vertices.Length} vertices");
            }

            chunk.meshRenderer.material = chunk.Material;
            chunk.meshFilter.sharedMesh = mesh;
            chunk.meshCollider.sharedMesh = mesh;
            chunk.hasMesh = true;
            chunk.dirty = false;

            ShadowEvents.Instance.PublishChunkUpdated(chunk.origin);
        }

        private MeshResult CalcMesh(Chunk chunk)
        {
            var quads = MeshQuads(chunk);

            var builder = new MeshBuilder();

            var meshResult = builder
                .AddQuads(quads, waterfalls.GetWaterfallChunk(chunk.origin))
                .Build();

            return meshResult;
        }

        private IEnumerable<Quad> MeshQuads(Chunk chunk)
        {
            var size = chunk.size;
            var quads = new List<Quad>();
            var isWater = chunk.options.isWater;
            var aoStrength = chunk.options.aoStrength;
            var waterLevel = water.config.waterLevel;
            var waterOpacity = water.config.opacity;
            var chunkY = chunk.origin.y;

            for (var d = 0; d < 3; d++)
            {
                if (isWater && d != 1)
                {
                    continue;
                }

                // Optimize this
                for (var i = -1; i < size; i++)
                {
                    for (var j = 0; j < size; j++)
                    {
                        for (var k = 0; k < size; k++)
                        {
                            var ca = GetVectorInt(i, j, k, d);
                            var cb = GetVectorInt(i + 1, j, k, d);

                            var va = chunk.GetLocal(i, j, k, d);
                            var vb = chunk.GetLocal(i + 1, j, k, d);

                            if (va > 0 == vb > 0)
                            {
                                continue;
                            }

                            if (!ground.Bounds.Contains(chunk.origin + cb) ||
                                !ground.Bounds.Contains(chunk.origin + ca))
                            {
                                continue;
                            }

                            var front = va > 0;

                            if (isWater && !front)
                            {
                                continue;
                            }

                            var i2 = i + 1;
                            var coord = front ? ca : cb;

                            var v1 = GetVector(i2, j, k, d);
                            var v2 = GetVector(i2, j + 1, k, d);
                            var v3 = GetVector(i2, j + 1, k + 1, d);
                            var v4 = GetVector(i2, j, k + 1, d);
                            var color = chunk.GetColorLocal(coord);

                            if (!isWater)
                            {
                                var worldCoordY = chunkY + coord.y;
                                var depth = waterLevel - worldCoordY;

                                if (depth > 0)
                                {
                                    var depthRatio = Mathf.Pow(waterOpacity, depth - 1);
                                    color.r *= depthRatio;
                                    color.g *= depthRatio;
                                    color.b *= depthRatio;
                                }
                            }

                            var aoI = front ? i + 1 : i;
                            var v00 = chunk.GetLocal(aoI, j - 1, k - 1, d);
                            var v01 = chunk.GetLocal(aoI, j, k - 1, d);
                            var v02 = chunk.GetLocal(aoI, j + 1, k - 1, d);
                            var v10 = chunk.GetLocal(aoI, j - 1, k, d);
                            var v12 = chunk.GetLocal(aoI, j + 1, k, d);
                            var v20 = chunk.GetLocal(aoI, j - 1, k + 1, d);
                            var v21 = chunk.GetLocal(aoI, j, k + 1, d);
                            var v22 = chunk.GetLocal(aoI, j + 1, k + 1, d);

                            var ao = new Vector4(
                                CalcAo(v10, v01, v00, aoStrength),
                                CalcAo(v01, v12, v02, aoStrength),
                                CalcAo(v12, v21, v22, aoStrength),
                                CalcAo(v21, v10, v20, aoStrength));

                            var quad = new Quad
                            {
                                Color = color,
                                Coord = coord,
                                Normal = chunk.GetNormalLocal(coord)
                            };

                            if (front)
                            {
                                quad.A = v1;
                                quad.B = v2;
                                quad.C = v3;
                                quad.D = v4;
                                quad.AO = ao;
                            }
                            else
                            {
                                quad.A = v3;
                                quad.B = v2;
                                quad.C = v1;
                                quad.D = v4;
                                quad.AO = new Vector4(ao[2], ao[1], ao[0], ao[3]);
                            }

                            quads.Add(quad);
                        }
                    }
                }
            }

            return quads;
        }

        private static Vector3Int GetVectorInt(int i, int j, int k, int d)
        {
            switch (d)
            {
                case 0:
                    return new Vector3Int(i, j, k);
                case 1:
                    return new Vector3Int(k, i, j);
                default:
                    return new Vector3Int(j, k, i);
            }
        }

        private static Vector3 GetVector(int i, int j, int k, int d)
        {
            switch (d)
            {
                case 0:
                    return new Vector3(i, j, k);
                case 1:
                    return new Vector3(k, i, j);
                default:
                    return new Vector3(j, k, i);
            }
        }

        private static float CalcAoRaw(float s1F, float s2F, float cf)
        {
            var s1 = s1F > 0;
            var s2 = s2F > 0;
            var c = cf > 0;

            if (s1 && s2)
            {
                return 1.0f;
            }

            var count = 0;
            if (s1) count++;
            if (s2) count++;
            if (c) count++;

            switch (count)
            {
                case 0:
                    return 0.0f;
                case 1:
                    return 0.33f;
                default:
                    return 0.66f;
            }
        }

        private static float CalcAo(float s1F, float s2F, float cf, float aoStrength)
        {
            return 1.0f - CalcAoRaw(s1F, s2F, cf) * aoStrength;
        }
    }
}