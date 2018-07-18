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

        public Noise heightNoise;
        public Noise canyonNoise;

        public Perlin growthNoise;
        public Perlin grassNoise;
        public Perlin treeNoise;
        public Perlin heightNoise2;
        public Perlin townNoise;
        public Random townRandom;
        public Random roadRandom;

        private Random r;

        private Perlin NextPerlin()
        {
            var noise = new Perlin();
            noise.Seed = r.Next();
            return noise;
        }

        private Noise NextNoise() {
            var noise = new Noise();
            noise.seed = r.Next();
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
            growthNoise = NextPerlin();
            grassNoise = NextPerlin();
            treeNoise = NextPerlin();
            canyonNoise = NextNoise();
            heightNoise2 = NextPerlin();
            townNoise = NextPerlin();
            townRandom = NextRandom();
            roadRandom = NextRandom();

            grassCurve = new ValueGradient();
            grassCurve.Add(0.3f, 0.8f);

            grassNoise.Frequency = 0.5f;

            heightNoise.frequency = 0.005f;
            canyonNoise.frequency = 0.01f;
            heightNoise.yScale = 0.4f;

            heightNoise2.OctaveCount = 8;
            growthNoise.OctaveCount = 5;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;

            townNoise.Frequency = 0.01f;
        }
    }
}
