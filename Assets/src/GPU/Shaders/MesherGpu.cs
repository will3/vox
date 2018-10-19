using UnityEngine;

namespace FarmVox
{
    public class MesherGpu
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

        public MesherGpu(int size)
        {
            this._size = size;
            _dataSize = size + 3;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");
        }

        public ComputeBuffer CreateTrianglesBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, Triangle.Size, ComputeBufferType.Append);
        }

        public ComputeBuffer CreateVoxelBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 3);
        }

        public ComputeBuffer CreateColorBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 4);
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorsBuffer, ComputeBuffer trianglesBuffer, TerrianChunk terrianChunk, Chunk chunk)
        {
            _shader.SetInt("_DataSize", _dataSize);
            _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            _shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);

            _shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);

            _shader.SetFloat("_NormalBanding", NormalBanding);
            _shader.SetInt("_UseNormals", UseNormals ? 1 : 0);
            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_NormalStrength", chunk.Chunks.NormalStrength);
            _shader.SetFloat("_AoStrength", TerrianConfig.Instance.AoStrength);

            var disptachNumber = Mathf.CeilToInt(_dataSize / (float)_workGroups);
            _shader.Dispatch(0, 3 * disptachNumber, disptachNumber, disptachNumber);
        }

        public Triangle[] ReadTriangles(ComputeBuffer trianglesBuffer) {
            var count = AppendBufferCounter.Count(trianglesBuffer);
            var triangles = new Triangle[count];

            trianglesBuffer.GetData(triangles);

            return triangles;
        }
    }
}