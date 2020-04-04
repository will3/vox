using System;

namespace FarmVox.Scripts
{
    [Serializable]
    public struct GpuNoise
    {
        public float frequency;
        public float amplitude;
        public int seed;
        public float lacunarity;
        public float persistence;
        public int octaves;
        public float yScale;
        public float xzScale;
        public int type;
        public float offset;

        public static int Stride => sizeof(float) * 7 + sizeof(int) * 3;
    }
}