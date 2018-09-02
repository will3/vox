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
        public bool generateWater = true;
        public bool generateTrees = true;

        public float aoStrength = 0.05f;
        public float shadowStrength = 0.4f;

#region ground
        public Noise heightNoise;
        public Noise rockNoise;
        public ColorGradient rockColorGradient;
        public Noise rockColorNoise;
        public ValueGradient heightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, -0.4f},
            {-0.2f, 0.05f},
            {0.15f, 0.1f},
            {0.5f, 1.0f},
            {1, 1.2f}
        });
#endregion

#region grass
        public Noise grassNoise;
        public Random grassRandom;
        public ValueGradient grassHeightFilter;
        public ValueGradient grassNormalFilter;
        public float grassOffset = 0f;
        public float grassMultiplier = 1.2f;
        public ColorGradient grassColor;
#endregion

#region tree
        public Perlin treeNoise;
        public Random treeRandom;
        public ValueGradient treeHeightGradient;
        public float treeSparse = 5.0f;
        public float treeAmount = 2.0f;
        public ValueGradient treeDensityFilter;
#endregion

#region waterfall
        public Random waterfallRandom;
        public Noise waterfallNoise;
        public ValueGradient waterfallHeightFilter;
#endregion

#region river
        public Noise riverNoise;
        public ValueGradient riverNoiseFilter;
#endregion

#region water
        public Color waterColor;
        #endregion

        #region stone
        public Noise stoneNoise;
        public Noise stoneNoise2;
        public Color stoneColor = GetColor("#676767");
        #endregion

        public Noise monsterNoise;

        public Random townRandom;
        public Random roadRandom;
        public Random monsterRandom;

        Random r;

        public Color trunkColor = GetColor("#4f402a");
        public Color leafColor = GetColor("#2f510c");

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

            stoneNoise = NextNoise();
            stoneNoise.frequency = 0.01f;
            stoneNoise.amplitude = 0.5f;
            stoneNoise2 = NextNoise();
            stoneNoise2.frequency = 0.005f;
            stoneNoise2.yScale = 4.0f;
            stoneNoise2.amplitude = 1.2f;

            waterColor = GetColor("#297eb6");
            waterColor.a = 0.4f;

            riverNoise = NextNoise();
            riverNoise.type = NoiseType.FBM;
            riverNoise.yScale = 0.2f;
            riverNoise.frequency = 0.05f;
            riverNoise.persistence = 0.5f;
            riverNoise.octaves = 7;
            riverNoise.amplitude = 4f;

            riverNoiseFilter = new ValueGradient(new Dictionary<float, float>() {
                {0, 0},
                //{0.9f, 1},
                //{1.0f, 1}
            });

            waterfallHeightFilter = new ValueGradient(new Dictionary<float, float>()
            {
                {0, 0},
                {0.3f, 0},
                {0.5f, 1},
                {1.0f, 1}
            });

            grassNoise.frequency = 0.02f;
            grassNoise.amplitude = 0.0f;

            rockNoise.frequency = 0.02f;
            rockNoise.amplitude = 1.5f;

            heightNoise.frequency = 0.015f;
            heightNoise.yScale = 0.4f;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;

            treeHeightGradient = new ValueGradient(1, 0);

            treeDensityFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {1, 1}
            });

            grassHeightFilter = new ValueGradient(new Dictionary<float, float>{
                { 0, 0.5f },
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
                {0, GetColor("#654d1f")},
                {1, GetColor("#654d1f")}
            });
            rockColorGradient.banding = 8;

            //var grass1 = GetColor("#cec479");
            //var grass2 = GetColor("#1EA14E");
            // var grass3 = GetColor("#285224");

            grassColor = new ColorGradient(new Dictionary<float, UnityEngine.Color> {
                {0, GetColor("#597420")},
                {1.0f, GetColor("#597420")},
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
