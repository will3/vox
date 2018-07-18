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

        public static int Stride
        {
            get
            {
                return sizeof(float) * 3 * 3 + sizeof(float) * 4 * 3;
            }
        }
    }

    public class MesherGPU
    {
        private ComputeShader shader;
        private readonly int size;

        public MesherGPU(int size)
        {
            this.size = size;
            shader = Resources.Load<ComputeShader>("Shaders/Mesher");
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer trianglesBuffer, ComputeBuffer colorsBuffer)
        {
            shader.SetInt("_Size", size);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);
            shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);

            shader.Dispatch(0, 3 * size / 8, size / 8, size / 8);
        }
    }
}