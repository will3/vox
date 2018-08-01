using System.Collections.Generic;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox
{
    public class TerrianConfig
    {
        static TerrianConfig instance;

        public static TerrianConfig Instance {
            get {
                if (instance == null) {
                    instance = new TerrianConfig();
                }
                return instance;
            }
        }

        public int size = 32;
        public int maxChunksY = 2;
        public int generateDis = 2;
        public float maxHeight = 64;
        public float hillHeight = 48;
        public float plainHeight = 4;
        public int waterLevel = 0;
        public int groundHeight = 12;
        public int maxChunksX = 3;

#region ground
        public Noise heightNoise;
        public Noise canyonNoise;
        public Noise rockNoise;
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
        public ColorGradient grassGradient;
#endregion

#region tree
        public Perlin treeNoise;
        public Random treeRandom;
        public ValueGradient treeHeightGradient;
        public float treeSparse = 5.0f;
        public float treeAmount = 2.0f;
        public ValueGradient treeCanyonFilter;
        public ValueGradient treeDensityFilter;
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

        Random r;

        Perlin NextPerlin()
        {
            var noise = new Perlin();
            noise.Seed = r.Next();
            return noise;
        }

        Noise NextNoise() {
            var noise = new Noise();
            noise.seed = r.Next();
            return noise;
        }

        Random NextRandom() {
            var seed = r.Next();
            var random = new Random(seed);
            return random;
        }

        public BoundsInt BoundsInt {
            get {
                var boundingCubeSize = new Vector3Int(maxChunksX, maxChunksX, maxChunksX) * size;
                return  new BoundsInt(boundingCubeSize * -1, boundingCubeSize * 2);
            }
        }

        TerrianConfig(int seed = 1337)
        {
            r = new Random(seed);
            heightNoise = NextNoise();
            canyonNoise = NextNoise();
            rockNoise = NextNoise();
            monsterNoise = NextNoise();

            rockColorNoise = NextNoise();
            rockColorNoise.frequency = 0.05f;
            rockColorNoise.yScale = 4.0f;
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
            grassNoise.amplitude = 0.0f;

            rockNoise.frequency = 0.02f;
            rockNoise.amplitude = 1.5f;

            heightNoise.frequency = 0.015f;
            heightNoise.yScale = 0.4f;

            canyonNoise.frequency = 0.01f;
            canyonNoise.yScale = 0.5f;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;

            treeHeightGradient = new ValueGradient(1, 0);

            treeCanyonFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {-0.1f, 1},
                {0.1f, 1},
                {1, 0}
            });

            treeDensityFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {1, 1}
            });

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

            rockColorGradient = new ColorGradient(new Dictionary<float, UnityEngine.Color> {
                {0, GetColor("#A7714C")},
                {1, GetColor("#A7714C")}
            });
            rockColorGradient.banding = 8;

            //var grass1 = GetColor("#cec479");
            var grass2 = GetColor("#2A9151");
            // var grass3 = GetColor("#285224");

            grassGradient = new ColorGradient(new Dictionary<float, UnityEngine.Color> {
                {0, grass2},
                {1.0f, grass2},
                //{1, grass3}
            });

            // grassGradient.banding = 8;
        }

        public static UnityEngine.Color GetColor(string hex)
        {
            UnityEngine.Color color = UnityEngine.Color.white;
            UnityEngine.ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}
