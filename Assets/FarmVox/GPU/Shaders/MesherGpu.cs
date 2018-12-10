using System;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class MesherGpu : IDisposable
    {
        public struct Triangle
        {
            public Vector3 A;
            public Vector3 B;
            public Vector3 C;
            public Color ColorA;
            public Color ColorB;
            public Color ColorC;
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
        private readonly int _size;
        private readonly int _dataSize;
        private readonly int _workGroups = 8;
        
        public int NormalBanding = 6;
        public bool UseNormals = true;
        public bool IsWater = false;

        private ComputeBuffer voxelBuffer;
        private ComputeBuffer colorsBuffer;
        private ComputeBuffer trianglesBuffer;

        public float NormalStrength = 0.0f;
        
        public MesherGpu(int size, int dataSize)
        {
            _size = size;
            _dataSize = dataSize;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");

            trianglesBuffer =
                new ComputeBuffer(_dataSize * _dataSize * _dataSize, Triangle.Size, ComputeBufferType.Append);
            voxelBuffer = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 3);
            colorsBuffer = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 4);
        }

        public void Dispatch()
        {
            _shader.SetInt("_DataSize", _dataSize);
            _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            _shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);

            _shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);

            _shader.SetFloat("_NormalBanding", NormalBanding);
            _shader.SetInt("_UseNormals", UseNormals ? 1 : 0);
            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_NormalStrength", NormalStrength);
            _shader.SetFloat("_AoStrength", TerrianConfig.Instance.AoStrength);

            var dispatchNumber = Mathf.CeilToInt(_dataSize / (float)_workGroups);
            _shader.Dispatch(0, 3 * dispatchNumber, dispatchNumber, dispatchNumber);
        }

        public Triangle[] ReadTriangles() {
            var count = AppendBufferCounter.Count(trianglesBuffer);
            var triangles = new Triangle[count];

            trianglesBuffer.GetData(triangles);

            return triangles;
        }

        public void SetData(float[] data)
        {
            voxelBuffer.SetData(data);
        }

        public void SetColors(Color[] colors)
        {
            colorsBuffer.SetData(colors);
        }

        public void Dispose()
        {
            if (voxelBuffer != null) voxelBuffer.Dispose();
            if (colorsBuffer != null) colorsBuffer.Dispose();
            if (trianglesBuffer != null) trianglesBuffer.Dispose();
        }
    }
}