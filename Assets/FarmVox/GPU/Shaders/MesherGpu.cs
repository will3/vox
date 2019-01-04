using System;
using FarmVox.Terrain;
using JetBrains.Annotations;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class MesherGpu : IDisposable
    {
        public struct Triangle
        {
            [UsedImplicitly]
            public Vector3 A;
            
            [UsedImplicitly]
            public Vector3 B;
            
            [UsedImplicitly]
            public Vector3 C;
            
            [UsedImplicitly]
            public Color ColorA;
            
            [UsedImplicitly]
            public Color ColorB;
            
            [UsedImplicitly]
            public Color ColorC;
            
            [UsedImplicitly]
            public int Index;

            public static int Size
            {
                get
                {
                    return
                        sizeof(float) * 3 +
                        sizeof(float) * 3 +
                        sizeof(float) * 3 +
                        sizeof(float) * 4 +
                        sizeof(float) * 4 +
                        sizeof(float) * 4 +
                        sizeof(int);
                }
            }
        }

        private readonly ComputeShader _shader;
        private readonly int _dataSize;
        private readonly TerrianConfig _config;
        private readonly int _workGroups = 8;
        
        public int NormalBanding = 6;
        public bool UseNormals = true;
        public bool IsWater = false;

        private readonly ComputeBuffer _voxelBuffer;
        private readonly ComputeBuffer _colorsBuffer;
        private readonly ComputeBuffer _trianglesBuffer;

        public float NormalStrength = 0.0f;
        
        public MesherGpu(int dataSize, TerrianConfig config)
        {
            _dataSize = dataSize;
            _config = config;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");

            _trianglesBuffer =
                new ComputeBuffer(_dataSize * _dataSize * _dataSize, Triangle.Size, ComputeBufferType.Append);
            _voxelBuffer = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 3);
            _colorsBuffer = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 4);
        }

        public void Dispatch()
        {
            _shader.SetInt("_DataSize", _dataSize);
            _shader.SetBuffer(0, "_VoxelBuffer", _voxelBuffer);
            _shader.SetBuffer(0, "_TrianglesBuffer", _trianglesBuffer);

            _shader.SetBuffer(0, "_ColorsBuffer", _colorsBuffer);

            _shader.SetFloat("_NormalBanding", NormalBanding);
            _shader.SetInt("_UseNormals", UseNormals ? 1 : 0);
            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_NormalStrength", NormalStrength);
            _shader.SetFloat("_AoStrength", _config.AoStrength);

            var dispatchNumber = Mathf.CeilToInt(_dataSize / (float)_workGroups);
            _shader.Dispatch(0, 3 * dispatchNumber, dispatchNumber, dispatchNumber);
        }

        public Triangle[] ReadTriangles() {
            var count = AppendBufferCounter.Count(_trianglesBuffer);
            var triangles = new Triangle[count];

            _trianglesBuffer.GetData(triangles);

            return triangles;
        }

        public void SetData(float[] data)
        {
            _voxelBuffer.SetData(data);
        }

        public void SetColors(Color[] colors)
        {
            _colorsBuffer.SetData(colors);
        }

        public void Dispose()
        {
            if (_voxelBuffer != null) _voxelBuffer.Dispose();
            if (_colorsBuffer != null) _colorsBuffer.Dispose();
            if (_trianglesBuffer != null) _trianglesBuffer.Dispose();
        }
    }
}