using System;
using LibNoise.Generator;

namespace FarmVox
{
    public class TerrianConfig
    {
        public int maxChunksY = 4;
        public int generateDis = 2;
        public int drawDis = 2;
        public int minTreeJ = 1;
        public int maxHeight = 64;
        public float hillHeight = 64;
        public float plainHeight = 12;

        public ValueGradient grassCurve;

        public Perlin heightNoise;
        public Perlin growthNoise;
        public Perlin grassNoise;
        public Perlin treeNoise;
        public Perlin canyonNoise;
        public Perlin heightNoise2;
        public Perlin townNoise;
        public Random townRandom;
        public Random roadRandom;

        private Random r;

        private Perlin NextNoise()
        {
            var noise = new Perlin();
            noise.Seed = r.Next();
            return noise;
        }

        private Random NextRandom() {
            var seed = r.Next();
            var random = new Random(seed);
            return random;
        }

        public TerrianConfig(int seed = 1337)
        {
            r = new Random(seed);
            heightNoise = NextNoise();
            growthNoise = NextNoise();
            grassNoise = NextNoise();
            treeNoise = NextNoise();
            canyonNoise = NextNoise();
            heightNoise2 = NextNoise();
            townNoise = NextNoise();
            townRandom = NextRandom();
            roadRandom = NextRandom();

            grassCurve = new ValueGradient();
            grassCurve.Add(0.3f, 0.8f);

            grassNoise.Frequency = 0.5f;

            heightNoise.Frequency = 0.01f;
            heightNoise.OctaveCount = 5;
            heightNoise2.OctaveCount = 8;
            growthNoise.OctaveCount = 5;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;
            canyonNoise.Frequency = 0.01f;

            townNoise.Frequency = 0.01f;
        }
    }
}
