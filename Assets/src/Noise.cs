using System.Collections;
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
        public float frequency = 0.01f;
        public float amplitude = 1.0f;
        public int seed = 1337;
        public float lacunarity = 2.0f;
        public float persistence = 0.5f;
        public int octaves = 5;
        public float yScale = 1.0f;
        public float xzScale = 1.0f;
        public NoiseType type;

        public ValueGradient filter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, -1.0f},
            {0.0f, 0.0f},
            {1.0f, 1.0f}
        });
    }
}