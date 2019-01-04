using System;
using LibNoise.Generator;

namespace FarmVox
{
    public static class NoiseUtils {
        private static Random _r = new Random();
        
        public static void SetSeed(int seed)
        {
            _r = new Random(seed);
        }
        
        public static Perlin NextPerlin()
        {
            var noise = new Perlin {Seed = _r.Next()};
            return noise;
        }

        public static Noise NextNoise() {
            var noise = new Noise {Seed = _r.Next()};
            return noise;
        }

        public static Random NextRandom() {
            var seed = _r.Next();
            var random = new Random(seed);
            return random;
        }
    }
}