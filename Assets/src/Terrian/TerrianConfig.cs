using System;
using System.Collections.Generic;
using LibNoise.Generator;

namespace FarmVox
{
    public class TerrianConfig
    {
        public int maxChunksY = 4;
        public int generateDis = 2;
        public int drawDis = 2;
        public int minTreeJ = 1;
        public float maxHeight = 64;
        public float hillHeight = 64;
        public float plainHeight = 12;
        public int waterLevel = 5;

        public Noise grassNoise;
        public Random grassRandom;
        public ValueGradient grassHeightFilter;
        public ValueGradient grassNormalFilter;
        public float grassOffset = 0f;
        public float grassMultiplier = 1.2f;

        public Noise heightNoise;
        public Noise canyonNoise;
        public Noise rockNoise;
        public Noise monsterNoise;
        public Noise scultNoise;

        public Perlin growthNoise;

        public Perlin heightNoise2;
        public Perlin townNoise;

        public Random townRandom;
        public Random roadRandom;
        public Random monsterRandom;

        public Perlin treeNoise;
        public Random treeRandom;
        public ValueGradient treeHeightGradient;
        public ColorGradient rockColorGradient;

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
            canyonNoise = NextNoise();
            rockNoise = NextNoise();
            monsterNoise = NextNoise();
            scultNoise = NextNoise();

            growthNoise = NextPerlin();
            grassNoise = NextNoise();
            treeNoise = NextPerlin();
            heightNoise2 = NextPerlin();
            townNoise = NextPerlin();

            townRandom = NextRandom();
            roadRandom = NextRandom();
            monsterRandom = NextRandom();
            treeRandom = NextRandom();
            grassRandom = NextRandom();

            grassNoise.frequency = 0.02f;

            rockNoise.frequency = 0.02f;
            scultNoise.frequency = 0.01f;
            scultNoise.yScale = 5f;
            scultNoise.octaves = 1;

            heightNoise.frequency = 0.015f;
            heightNoise.yScale = 0.4f;

            canyonNoise.frequency = 0.01f;
            canyonNoise.yScale = 0.5f;

            heightNoise2.OctaveCount = 8;
            growthNoise.OctaveCount = 5;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;

            townNoise.Frequency = 0.01f;

            treeHeightGradient = new ValueGradient(1, 0);
            grassHeightFilter = new ValueGradient(new Dictionary<float, float>{
                { 0, 1 },
                { 1, 0 } });

            grassNormalFilter = new ValueGradient(
                new Dictionary<float, float>{
                { -1, 0 },
                { 0, 0 },
                { 0.49f, 0},
                { 0.5f, 1 },
                { 1, 1 } });

            //rockColorGradient = new ColorGradient(GetColor("#ce8643"), GetColor("#cec479"));
        }

        //public static Color GetColor(string hex)
        //{
        //    Color color = Color.white;
        //    ColorUtility.TryParseHtmlString(hex, out color);
        //    return color;
        //}
    }
}
