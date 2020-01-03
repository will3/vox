using System;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

namespace FarmVox
{
    public enum NoiseType
    {
        FBM = 0,
        Ridged = 1,
        Turbulence = 2
    }

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
    }

    public static class ModuleBuilder
    {
        public static ModuleBase Build(Noise noise)
        {
            return new ScaleBias(noise.amplitude, 0,
                new Scale(noise.xzScale, noise.yScale, noise.xzScale,
                    new ScaleBias(0.5f, 0.5f, new Perlin(noise.frequency, noise.lacunarity, noise.persistence,
                        noise.octaves, noise.seed,
                        QualityMode.Medium))));
        }
    }
}