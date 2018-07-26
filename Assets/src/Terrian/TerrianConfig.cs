using System;
using System.Collections.Generic;
using LibNoise.Generator;

namespace FarmVox
{
    public class TerrianConfig
    {
        public int maxChunksY = 4;
        public int generateDis = 4;
        public int drawDis = 4;
        public int minTreeJ = 1;
        public float maxHeight = 64;
        public float hillHeight = 64;
        public float plainHeight = 12;
        public int waterLevel = 2;

#region ground
        public Noise heightNoise;
        public Noise canyonNoise;
        public Noise rockNoise;
        public Noise scultNoise;
        public ColorGradient rockColorGradient;

        public Noise rockColorNoise;
#endregion

#region grass
        public Noise grassNoise;
        public Random grassRandom;
        public ValueGradient grassHeightFilter;
        public ValueGradient grassNormalFilter;
        public float grassOffset = 0f;
        public float grassMultiplier = 1.2f;
#endregion

#region tree
        public Perlin treeNoise;
        public Random treeRandom;
        public ValueGradient treeHeightGradient;
        public float treeSparse = 5.0f;
        public float treeAmount = 2.0f;
#endregion

#region waterfall
        public Random waterfallRandom;
        public Noise waterfallNoise;
        public ValueGradient waterfallHeightFilter;
#endregion

        public Noise monsterNoise;

        public Random townRandom;
        public Random roadRandom;
        public Random monsterRandom;

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

            rockColorNoise = NextNoise();
            rockColorNoise.frequency = 0.05f;
            rockColorNoise.yScale = 6.0f;
            rockColorNoise.amplitude = 1.0f;

            grassNoise = NextNoise();
            treeNoise = NextPerlin();

            townRandom = NextRandom();
            roadRandom = NextRandom();
            monsterRandom = NextRandom();
            treeRandom = NextRandom();
            waterfallRandom = NextRandom();
            grassRandom = NextRandom();

            waterfallNoise = NextNoise();
            waterfallNoise.frequency = 0.005f;

            waterfallHeightFilter = new ValueGradient(new Dictionary<float, float>()
            {
                {0, 0},
                {0.5f, 0},
                {1.0f, 1}
            });

            grassNoise.frequency = 0.02f;

            rockNoise.frequency = 0.02f;
            rockNoise.amplitude = 1.5f;
            scultNoise.frequency = 0.01f;
            scultNoise.yScale = 5f;
            scultNoise.octaves = 1;

            heightNoise.frequency = 0.015f;
            heightNoise.yScale = 0.4f;

            canyonNoise.frequency = 0.01f;
            canyonNoise.yScale = 0.5f;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;

            treeHeightGradient = new ValueGradient(1, 0);
            grassHeightFilter = new ValueGradient(new Dictionary<float, float>{
                { 0, 1 },
                { 0.5f, 0},
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
