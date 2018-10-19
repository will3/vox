using System;
using System.Collections.Generic;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox
{
    [Serializable]
    public class TerrianConfig
    {
        public readonly Colors Colors = new Colors();

        private static TerrianConfig _instance;

        public static TerrianConfig Instance {
            get { return _instance ?? (_instance = new TerrianConfig()); }
        }

        public int ActualWaterLevel {
            get {
                return WaterLevel + GroundHeight;
            }
        }

        public int Size = 32;
        public int MaxChunksY = 2;
        public float MaxHeight = 64;
        public float HillHeight = 48;
        public float PlainHeight = 4;
        public int WaterLevel = 0;
        public int GroundHeight = 12;
        public int MaxChunksX = 3;
        public bool GenerateWater = true;
        public bool GenerateTrees = true;

        public float AoStrength = 0.2f;
        public float ShadowStrength = 0.5f;

        public int TreeMinDis = 3;
        
#region ground
        public Noise HeightNoise;
        public Noise RockNoise;
        public Noise RockColorNoise;
#endregion

#region grass
        public Noise GrassNoise;
        public ValueGradient GrassHeightFilter;
        public ValueGradient GrassNormalFilter;

        #endregion

#region tree
        public Perlin TreeNoise;
        public Random TreeRandom;
        public ValueGradient TreeHeightGradient;
        public float TreeAmount = 2.0f;
        public ValueGradient TreeDensityFilter;
#endregion

#region waterfall
        public Random WaterfallRandom;
        public Noise WaterfallNoise;
        public ValueGradient WaterfallHeightFilter;
        public float WaterfallChance = 0.01f;
#endregion

#region river
        public Noise RiverNoise;
        public ValueGradient RiverNoiseFilter;
#endregion

#region stone
        public Noise StoneNoise;
        public Noise StoneNoise2;
        public float StoneThreshold = 0.5f;

        public ValueGradient StoneHeightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, 1.0f},
            {0f, 1.0f},
            {1.0f, 0.0f},
        });
#endregion

        private readonly Random _r;

        Perlin NextPerlin()
        {
            var noise = new Perlin {Seed = _r.Next()};
            return noise;
        }

        Noise NextNoise() {
            var noise = new Noise {seed = _r.Next()};
            return noise;
        }

        Random NextRandom() {
            var seed = _r.Next();
            var random = new Random(seed);
            return random;
        }

        public BoundsInt BoundsInt {
            get {
                var boundingCubeSize = new Vector3Int(MaxChunksX, MaxChunksX, MaxChunksX) * Size;
                return  new BoundsInt(boundingCubeSize * -1, boundingCubeSize * 2);
            }
        }

        private TerrianConfig(int seed = 1337)
        {
            _r = new Random(seed);
            HeightNoise = NextNoise();
            HeightNoise.frequency = 0.015f;
            HeightNoise.yScale = 0.4f;
            HeightNoise.octaves = 5;
            HeightNoise.filter = new ValueGradient(new Dictionary<float, float>
            {
                {-1.0f, -0.4f},
                {-0.2f, 0.05f},
                {0.15f, 0.1f},
                {0.5f, 1.0f},
                {1, 1.2f}
            });

            RockNoise = NextNoise();

            RockColorNoise = NextNoise();
            RockColorNoise.frequency = 0.05f;
            RockColorNoise.yScale = 4.0f;
            RockColorNoise.amplitude = 1.0f;

            GrassNoise = NextNoise();

            TreeNoise = NextPerlin();

            TreeRandom = NextRandom();
            WaterfallRandom = NextRandom();

            WaterfallNoise = NextNoise();
            WaterfallNoise.frequency = 0.005f;

            StoneNoise = NextNoise();
            StoneNoise.frequency = 0.01f;
            StoneNoise.amplitude = 0.5f;
            StoneNoise2 = NextNoise();
            StoneNoise2.frequency = 0.005f;
            StoneNoise2.yScale = 4.0f;
            StoneNoise2.amplitude = 1.2f;

            RiverNoise = NextNoise();
            RiverNoise.type = NoiseType.FBM;
            RiverNoise.yScale = 0.2f;
            RiverNoise.frequency = 0.05f;
            RiverNoise.persistence = 0.5f;
            RiverNoise.octaves = 7;
            RiverNoise.amplitude = 4f;

            RiverNoiseFilter = new ValueGradient(new Dictionary<float, float>() {
                {0, 0},
                //{0.9f, 1},
                //{1.0f, 1}
            });

            WaterfallHeightFilter = new ValueGradient(new Dictionary<float, float>()
            {
                {0, 0},
                {0.3f, 0},
                {0.5f, 1},
                {1.0f, 1}
            });

            GrassNoise.frequency = 0.01f;
            GrassNoise.amplitude = 2.0f;

            RockNoise.frequency = 0.02f;
            RockNoise.amplitude = 1.5f;

            TreeNoise.Frequency = 0.05f;
            TreeNoise.OctaveCount = 5;

            TreeHeightGradient = new ValueGradient(1, 0);

            TreeDensityFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {1, 1}
            });

            GrassHeightFilter = new ValueGradient(new Dictionary<float, float>{
                { 0, 0.5f },
                { 0.5f, 0},
                { 1, 0 } });

            GrassNormalFilter = new ValueGradient(
                new Dictionary<float, float>{
                { -1, 0 },
                { 0, 0 },
                { 0.49f, 0},
                { 0.5f, 1 },
                { 1, 1 } });
        }
    }
}
