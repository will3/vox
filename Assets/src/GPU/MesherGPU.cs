using UnityEngine;

namespace FarmVox
{
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Color colorA;
        public Color colorB;
        public Color colorC;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 3 + sizeof(float) * 4 * 3;
            }
        }
    }

    public class MesherGPU
    {
        private readonly ComputeShader shader;
        private readonly int size;
        private readonly int dataSize;
        private readonly int workGroups = 8;
        public int normalBranding = 5;
        public bool useNormals = true;
        public bool isWater = false;
        private ComputeBuffer shadowBuffer;

        public MesherGPU(int size)
        {
            this.size = size;
            this.dataSize = size + 3;
            shader = Resources.Load<ComputeShader>("Shaders/Mesher");
            shadowBuffer = new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float));
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

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorsBuffer, ComputeBuffer trianglesBuffer, TerrianChunk terrianChunk)
        {
            shader.SetInt("_DataSize", dataSize);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);
            shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);
            shader.SetInt("_NormalBranding", normalBranding);
            shader.SetInt("_UseNormals", useNormals ? 1 : 0);
            shader.SetInt("_IsWater", isWater ? 1 : 0);

            var shadowBufferData = new float[dataSize * dataSize * dataSize];
            foreach(var coord in terrianChunk.Chunk.surfaceCoords) {
                var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;
                var v = terrianChunk.Chunk.GetLighting(coord);
                shadowBufferData[index] = v;
            }

            shadowBuffer.SetData(shadowBufferData);
            shader.SetBuffer(0, "_ShadowBuffer", shadowBuffer);

            var disptachNumber = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, 3 * disptachNumber, disptachNumber, disptachNumber);
        }

        public int Count(ComputeBuffer from) {
            var countBuffer = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);

            ComputeBuffer.CopyCount(from, countBuffer, 0);
            int[] counter = new int[] { 0, 0, 0, 0 };
            countBuffer.GetData(counter);
            int count = counter[0];

            countBuffer.Dispose();

            return count;
        }

        public Triangle[] ReadTriangles(ComputeBuffer trianglesBuffer) {
            var count = Count(trianglesBuffer);
            var triangles = new Triangle[count];

            trianglesBuffer.GetData(triangles);

            return triangles;
        }

        public void Dispose() {
            shadowBuffer.Dispose();
        }
    }
}