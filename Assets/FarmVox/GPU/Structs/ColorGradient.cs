
using UnityEngine;

namespace FarmVox.GPU.Structs
{
    public struct ColorGradient
    {
        public float[] Keys;
        public Vector4[] Colors;
        public int Banding;
        public int Size;

        public static int Stride
        {
            get { return sizeof(float) * 8 + sizeof(float) * 4 * 8 + sizeof(int) * 2; }
        }
    }
}