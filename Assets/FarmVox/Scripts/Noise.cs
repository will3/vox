using System;

namespace FarmVox.Scripts
{
    [Serializable]
    public class Noise
    {
        public float frequency = 0.01f;
        public float amplitude = 1.0f;
        public int seed = 1337;
        public float lacunarity = 2.0f;
        public float persistence = 0.5f;
        public int octaves = 5;
        public float yScale = 1.0f;
        public float xzScale = 1.0f;
        public NoiseType type;
        public float offset;
    }
    
//    struct Noise {
//        int size;
//        int seed;
//        float frequency;
//        float lacunarity;
//        float persistence;
//        int octaves;
//        float3 origin;
//        float yScale;
//        float xzScale;
//        float amplitude;
//        int dataSize;
//        int type;
//        float offset;
//    };
}