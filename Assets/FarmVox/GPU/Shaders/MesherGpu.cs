using System;
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

            public Vector3 D;
            
            public Color Color;

            public Vector4 AO;
            
            public int X;
            
            public int Y;
            
            public int Z;

            public int GetIndex(int size)
            {
                return X * size * size + Y * size + Z;
            }

            public Vector3Int Coord
            {
                get { return new Vector3Int(X, Y, Z); }
            }

            public static int Size
            {
                get
                {
                    return
                        sizeof(float) * 3 * 4 +
                        sizeof(float) * 4 +
                        sizeof(float) * 4 +
                        sizeof(int) * 3;
                }
            }
        }

        private readonly ComputeShader _shader;
        private readonly int _size;
        private readonly MesherSettings _settings;
        private readonly int[] _workGroups = {8, 8, 4};
        
        public int NormalBanding = 6;
        public bool UseNormals = true;
        public bool IsWater = false;

        private readonly ComputeBuffer _voxelBuffer;
        private readonly ComputeBuffer _colorsBuffer;
        private readonly ComputeBuffer _trianglesBuffer;

        public float NormalStrength = 0.0f;
        
        public MesherGpu(int size, MesherSettings settings)
        {
            _size = size;
            _settings = settings;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");

            _trianglesBuffer =
                new ComputeBuffer(_size * _size * _size, Triangle.Size, ComputeBufferType.Append);
            _voxelBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float));
            _colorsBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float) * 4);
        }

        public void Dispatch()
        {
            _shader.SetInt("_Size", _size);
            _shader.SetBuffer(0, "_VoxelBuffer", _voxelBuffer);
            _shader.SetBuffer(0, "_TrianglesBuffer", _trianglesBuffer);

            _shader.SetBuffer(0, "_ColorsBuffer", _colorsBuffer);

            _shader.SetFloat("_NormalBanding", NormalBanding);
            _shader.SetInt("_UseNormals", UseNormals ? 1 : 0);
            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_NormalStrength", NormalStrength);
            _shader.SetFloat("_AoStrength", _settings.AoStrength);

            _shader.Dispatch(0, 
                3 * Mathf.CeilToInt(_size / (float) _workGroups[0]),
                Mathf.CeilToInt(_size / (float) _workGroups[1]),
                Mathf.CeilToInt(_size / (float) _workGroups[2]));
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