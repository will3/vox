using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(NavMeshSourceTag))]
    public class Chunk : MonoBehaviour
    {
        public int size = 32;
        public Vector3Int origin;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public ChunkOptions options;
        public bool dirty;
        public bool hasMesh;
        public Chunks chunks;

        public int DataSize => size;

        private float[] _data;
        private Color[] _colors;
        private Vector3[] _normals;

        public readonly HashSet<Vector3Int> SurfaceCoords = new HashSet<Vector3Int>();
        public readonly HashSet<Vector3Int> SurfaceCoordsUp = new HashSet<Vector3Int>();

        private bool _surfaceCoordsDirty = true;
        private ComputeBuffer _voxelDataBuffer;
        private List<VoxelData> _coordData = new List<VoxelData>();
        private Material _material;
        private Waterfalls _waterfalls;

        private static readonly int WaterfallShadowStrength = Shader.PropertyToID("_WaterfallShadowStrength");
        private static readonly int WaterfallSpeed = Shader.PropertyToID("_WaterfallSpeed");
        private static readonly int WaterfallWidth = Shader.PropertyToID("_WaterfallWidth");
        private static readonly int WaterfallMin = Shader.PropertyToID("_WaterfallMin");
        private static readonly int WaterfallVariance = Shader.PropertyToID("_WaterfallVariance");
        private static readonly int Origin = Shader.PropertyToID("_Origin");
        private static readonly int Size = Shader.PropertyToID("_Size");
        private static readonly int LightDir = Shader.PropertyToID("_LightDir");
        private static readonly int NormalBanding = Shader.PropertyToID("_NormalBanding");
        private static readonly int NormalStrength = Shader.PropertyToID("_NormalStrength");
        private static readonly int ShadowMap00 = Shader.PropertyToID("_ShadowMap00");
        private static readonly int ShadowMap01 = Shader.PropertyToID("_ShadowMap01");
        private static readonly int ShadowMap10 = Shader.PropertyToID("_ShadowMap10");
        private static readonly int ShadowMap11 = Shader.PropertyToID("_ShadowMap11");
        private static readonly int ShadowMapSize = Shader.PropertyToID("_ShadowMapSize");
        private static readonly int ShadowStrength = Shader.PropertyToID("_ShadowStrength");

        private ComputeBuffer _defaultVoxelDataBuffer;

        public Material Material
        {
            get
            {
                if (_material != null) return _material;
                if (options.transparent)
                {
                    _material = Materials.GetVoxelMaterialTrans();
                }
                else
                {
                    _material = Materials.GetVoxelMaterial();

                    if (_waterfalls == null)
                    {
                        _waterfalls = FindObjectOfType<Waterfalls>();
                    }

                    if (_waterfalls == null)
                    {
                        return _material;
                    }

                    _material.SetFloat(WaterfallShadowStrength, _waterfalls.shadowStrength);
                    _material.SetFloat(WaterfallSpeed, _waterfalls.speed);
                    _material.SetFloat(WaterfallWidth, _waterfalls.width);
                    _material.SetFloat(WaterfallMin, _waterfalls.min);
                    _material.SetFloat(WaterfallVariance, _waterfalls.variance);
                    _material.SetInt(NormalBanding, options.normalBanding);
                    _material.SetFloat(NormalStrength, options.normalStrength);
                }

                _material.SetVector(Origin, (Vector3) origin);
                _material.SetInt(Size, size);

                var defaultBuffer = ShadowMap.GetDefaultBuffer();
                _material.SetBuffer(ShadowMap00, defaultBuffer);
                _material.SetBuffer(ShadowMap01, defaultBuffer);
                _material.SetBuffer(ShadowMap10, defaultBuffer);
                _material.SetBuffer(ShadowMap11, defaultBuffer);
                _material.SetFloat(ShadowStrength, options.shadowStrength);

                return _material;
            }
        }

        public void SetVoxelData(List<VoxelData> coordData)
        {
            _coordData = coordData;
            UpdateVoxelDataBuffer();
        }

        private void UpdateVoxelDataBuffer()
        {
            _voxelDataBuffer?.Dispose();

            if (_coordData.Count == 0)
            {
                return;
            }

            _voxelDataBuffer = new ComputeBuffer(_coordData.Count, VoxelData.Size);
            _voxelDataBuffer.SetData(_coordData);
        }

        public ComputeBuffer GetVoxelDataBuffer()
        {
            if (_coordData.Count == 0)
            {
                return _defaultVoxelDataBuffer ?? (_defaultVoxelDataBuffer = new ComputeBuffer(1, VoxelData.Size));
            }

            return _voxelDataBuffer;
        }

        public void SetColors([NotNull] Color[] value)
        {
            _colors = value ?? throw new ArgumentNullException(nameof(value));
            dirty = true;
        }

        public void SetData([NotNull] float[] value)
        {
            _data = value ?? throw new ArgumentNullException(nameof(value));
            dirty = true;
            _surfaceCoordsDirty = true;
        }

        public void SetNormals([NotNull] Vector3[] normals)
        {
            _normals = normals ?? throw new ArgumentNullException(nameof(normals));
            dirty = true;
        }

        public Vector3 GetNormal(Vector3Int localCoord)
        {
            var index = GetIndex(localCoord);
            return _normals?[index] ?? Vector3.zero;
        }

        public Vector3 GetNormalLocal(Vector3Int localCoord)
        {
            return IsInBound(localCoord) ? GetNormal(localCoord) : chunks.GetNormal(localCoord + origin);
        }

        public void UpdateSurfaceCoords()
        {
            if (!_surfaceCoordsDirty)
            {
                return;
            }

            SurfaceCoords.Clear();
            SurfaceCoordsUp.Clear();

            for (var d = 0; d < 3; d++)
            {
                for (var i = 0; i < DataSize - 1; i++)
                {
                    for (var j = 0; j < DataSize - 1; j++)
                    {
                        for (var k = 0; k < DataSize - 1; k++)
                        {
                            var coordA = Vectors.GetVector3Int(i, j, k, d);
                            var coordB = Vectors.GetVector3Int(i + 1, j, k, d);
                            var a = Get(coordA);
                            var b = Get(coordB);

                            if (a > 0 == b > 0)
                            {
                                continue;
                            }

                            if (a > 0)
                            {
                                SurfaceCoords.Add(coordA);
                                if (d == 1)
                                {
                                    SurfaceCoordsUp.Add(coordA);
                                }
                            }
                            else
                            {
                                SurfaceCoords.Add(coordB);
                            }
                        }
                    }
                }
            }

            _surfaceCoordsDirty = false;
        }

        public float GetLocal(Vector3Int coord)
        {
            return IsInBound(coord)
                ? Get(coord)
                : chunks.Get(origin + coord);
        }

        public float Get(Vector3Int coord)
        {
            if (_data == null || _data.Length == 0)
            {
                return 0;
            }

            var i = coord.x;
            var j = coord.y;
            var k = coord.z;

            if (i < 0 || i >= DataSize ||
                j < 0 || j >= DataSize ||
                k < 0 || k >= DataSize)
            {
                throw new Exception("out of bounds:" + new Vector3Int(i, j, k));
            }

            var index = GetIndex(coord);
            return _data[index];
        }

        public void Set(Vector3Int coord, float v)
        {
            if (_data == null || _data.Length == 0)
            {
                _data = new float[DataSize * DataSize * DataSize];
            }

            var i = coord.x;
            var j = coord.y;
            var k = coord.z;

            if (i < 0 || i >= DataSize ||
                j < 0 || j >= DataSize ||
                k < 0 || k >= DataSize)
            {
                throw new Exception("out of bounds:" + new Vector3Int(i, j, k));
            }

            var index = GetIndex(coord);
            _data[index] = v;
            dirty = true;
            _surfaceCoordsDirty = true;
        }

        public void SetColor(Vector3Int coord, Color v)
        {
            if (_colors == null || _colors.Length == 0)
            {
                _colors = new Color[DataSize * DataSize * DataSize];
            }

            var index = GetIndex(coord);
            _colors[index] = v;
            dirty = true;
        }

        public Color GetColorLocal(Vector3Int coord)
        {
            return IsInBound(coord)
                ? GetColor(coord)
                : chunks.GetColor(origin + coord);
        }

        public Color GetColor(Vector3Int coord)
        {
            var index = GetIndex(coord);
            return _colors[index];
        }

        private int GetIndex(Vector3Int coord)
        {
            var index = coord.x * DataSize * DataSize + coord.y * DataSize + coord.z;
            return index;
        }

        private void OnDrawGizmosSelected()
        {
            if (_normals == null)
            {
                return;
            }

            UpdateSurfaceCoords();

            foreach (var surfaceCoord in SurfaceCoords)
            {
                var normal = GetNormal(surfaceCoord);
                var offset = new Vector3(0.5f, 1.0f, 0.5f);
                Gizmos.DrawLine(surfaceCoord + origin + offset, surfaceCoord + origin + offset + normal);
            }
        }

        public void OnDestroy()
        {
            _voxelDataBuffer?.Dispose();
            _defaultVoxelDataBuffer?.Dispose();
        }

        public void UpdateShadowBuffers(ComputeBuffer[] shadowMaps)
        {
            Material.SetBuffer(ShadowMap00, shadowMaps[0]);
            Material.SetBuffer(ShadowMap01, shadowMaps[1]);
            Material.SetBuffer(ShadowMap10, shadowMaps[2]);
            Material.SetBuffer(ShadowMap11, shadowMaps[3]);
        }

        public void SetLightDir(Vector3Int lightDir)
        {
            Material.SetVector(LightDir, (Vector3) lightDir);
        }

        public bool IsInBound(Vector3Int localCoord)
        {
            return localCoord.x >= 0 &&
                   localCoord.y >= 0 &&
                   localCoord.z >= 0 &&
                   localCoord.x < size &&
                   localCoord.y < size &&
                   localCoord.z < size;
        }

        public void SetShadowMapSize(int dataSize)
        {
            Material.SetInt(ShadowMapSize, dataSize);
        }

        public void SetNormal(Vector3Int coord, Vector3 normal)
        {
            if (_normals == null)
            {
                _normals = new Vector3[DataSize * DataSize * DataSize];
            }

            var index = GetIndex(coord);
            _normals[index] = normal;
            dirty = true;
        }
    }
}