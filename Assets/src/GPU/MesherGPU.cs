using UnityEngine;

namespace FarmVox
{
    public class MesherGPU
    {
        public struct Triangle
        {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;
            public Color colorA;
            public Color colorB;
            public Color colorC;
            public int index;

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

        readonly ComputeShader shader;
        readonly int size;
        readonly int dataSize;
        readonly int workGroups = 8;
        public int normalBranding = 6;
        public bool useNormals = true;
        public bool isWater = false;

        public MesherGPU(int size)
        {
            this.size = size;
            dataSize = size + 3;
            shader = Resources.Load<ComputeShader>("Shaders/Mesher");
        }

        public ComputeBuffer CreateTrianglesBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, Triangle.Size, ComputeBufferType.Append);
        }

        public ComputeBuffer CreateVoxelBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float) * 3);
        }

        public ComputeBuffer CreateColorBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float) * 4);
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorsBuffer, ComputeBuffer trianglesBuffer, TerrianChunk terrianChunk, Chunk chunk)
        {
            shader.SetInt("_DataSize", dataSize);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);

            shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);

            shader.SetInt("_NormalBranding", normalBranding);
            shader.SetInt("_UseNormals", useNormals ? 1 : 0);
            shader.SetInt("_IsWater", isWater ? 1 : 0);
            shader.SetFloat("_NormalStrength", TerrianConfig.Instance.normalStrength);

            var disptachNumber = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, 3 * disptachNumber, disptachNumber, disptachNumber);
        }

        public Triangle[] ReadTriangles(ComputeBuffer trianglesBuffer) {
            var count = AppendBufferCounter.Count(trianglesBuffer);
            var triangles = new Triangle[count];

            trianglesBuffer.GetData(triangles);

            return triangles;
        }
    }
}