using System.Collections;
using System.Collections.Generic;
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
        public Water water;
        private LightController _lightController;
        private Vector3Int LightDir => _lightController.lightDir.GetDirVector();
        public BoundsInt bounds;
        public bool useBounds;
        private static readonly int VoxelData = Shader.PropertyToID("_VoxelData");

        private void Awake()
        {
            ShadowEvents.Instance.ShadowMapUpdated += OnShadowMapUpdated;
        }

        private void Start()
        {
            water = FindObjectOfType<Water>();
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
                    DrawChunk(chunk);
                    yield return null;
                }

                yield return null;
            }

            // ReSharper disable once IteratorNeverReturns
        }

        private void DrawChunk(Chunk chunk)
        {
            if (chunk.Mesh != null)
            {
                Destroy(chunk.Mesh);
            }

            chunk.Mesh = MeshGpu(chunk);
            chunk.meshRenderer.material = chunk.Material;
            chunk.meshFilter.sharedMesh = chunk.Mesh;
            chunk.meshCollider.sharedMesh = chunk.Mesh;

            chunk.Dirty = false;

            ShadowEvents.Instance.PublishChunkUpdated(chunk.origin);
        }

        private Mesh MeshGpu(Chunk chunk)
        {
            var quads = MeshCpu(chunk);

            var builder = new MeshBuilder();
            var meshResult = builder
                .AddQuads(quads, waterfalls.GetWaterfallChunk(chunk.origin))
                .Build();

            chunk.SetVoxelData(meshResult.VoxelData);

            // Update voxel data buffer
            chunk.Material.SetBuffer(VoxelData, chunk.GetVoxelDataBuffer());

            return meshResult.Mesh;
        }

        private IEnumerable<Quad> MeshCpu(Chunk chunk)
        {
            var size = chunk.DataSize;
            var quads = new List<Quad>();
            var isWater = chunk.options.isWater;
            var aoStrength = chunk.options.aoStrength;
            var waterLevel = water.config.waterLevel;
            var waterOpacity = water.config.opacity;

            for (var d = 0; d < 3; d++)
            {
                for (var i = 0; i < size; i++)
                {
                    for (var j = 0; j < size; j++)
                    {
                        for (var k = 0; k < size; k++)
                        {
                            if (i == 0 || i >= size - 2 || j == 0 || j >= size - 2 || k == 0 || k >= size - 2)
                            {
                                continue;
                            }

                            var ca = GetVectorInt(i, j, k, d);
                            var cb = GetVectorInt(i + 1, j, k, d);

                            var va = chunk.Get(ca);
                            var vb = chunk.Get(cb);

                            if (va > 0 == vb > 0)
                            {
                                continue;
                            }

                            var front = va > 0;
                            var i2 = i + 1;
                            var coord = front ? ca : cb;

                            var v1 = GetVector(i2, j, k, d);
                            var v2 = GetVector(i2, j + 1, k, d);
                            var v3 = GetVector(i2, j + 1, k + 1, d);
                            var v4 = GetVector(i2, j, k + 1, d);
                            var color = chunk.GetColor(coord);

                            if (isWater)
                            {
                                var worldCoord = chunk.origin + coord;
                                var depth = waterLevel - worldCoord.y;

                                if (depth > 0)
                                {
                                    var depthRatio = Mathf.Pow(waterOpacity, depth - 1);
                                    color *= depthRatio;
                                }
                            }

                            var aoI = front ? i + 1 : i;
                            var v00 = chunk.Get(GetVectorInt(aoI, j - 1, k - 1, d));
                            var v01 = chunk.Get(GetVectorInt(aoI, j, k - 1, d));
                            var v02 = chunk.Get(GetVectorInt(aoI, j + 1, k - 1, d));
                            var v10 = chunk.Get(GetVectorInt(aoI, j - 1, k, d));
                            var v12 = chunk.Get(GetVectorInt(aoI, j + 1, k, d));
                            var v20 = chunk.Get(GetVectorInt(aoI, j - 1, k + 1, d));
                            var v21 = chunk.Get(GetVectorInt(aoI, j, k + 1, d));
                            var v22 = chunk.Get(GetVectorInt(aoI, j + 1, k + 1, d));

                            var ao = new Vector4(
                                CalcAo(v10, v01, v00, aoStrength),
                                CalcAo(v01, v12, v02, aoStrength),
                                CalcAo(v12, v21, v22, aoStrength),
                                CalcAo(v21, v10, v20, aoStrength));

                            var quad = new Quad
                            {
                                Color = color,
                                Coord = coord,
                                Normal = chunk.GetNormal(coord)
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

        private Vector3Int GetVectorInt(int i, int j, int k, int d)
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

        private Vector3 GetVector(int i, int j, int k, int d)
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
            return 1.0f - (CalcAoRaw(s1F, s2F, cf) * aoStrength);
        }
    }
}