using System.Collections.Generic;

namespace FarmVox
{
    public enum NoiseType {
        FBM = 0,
        Ridged = 1,
        Turbulence = 2
    }

    public class Noise
    {
        public float Frequency = 0.01f;
        public float Amplitude = 1.0f;
        public int Seed = 1337;
        public float Lacunarity = 2.0f;
        public float Persistence = 0.5f;
        public int Octaves = 5;
        public float YScale = 1.0f;
        public float XzScale = 1.0f;
        public NoiseType Type;

        public ValueGradient Filter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, -1.0f},
            {0.0f, 0.0f},
            {1.0f, 1.0f}
        });
    }
}