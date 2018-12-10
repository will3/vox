using System;
using System.Collections.Generic;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Terrain
{
    [Serializable]
    public class TerrianConfig
    {
        public readonly Colors Colors = new Colors();

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

        public float AoStrength = 0.15f;
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
            var noise = new Noise {Seed = _r.Next()};
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

        public TerrianConfig(int seed = 1337)
        {
            _r = new Random(seed);
            HeightNoise = NextNoise();
            HeightNoise.Frequency = 0.015f;
            HeightNoise.YScale = 0.4f;
            HeightNoise.Octaves = 5;
            HeightNoise.Filter = new ValueGradient(new Dictionary<float, float>
            {
                {-1.0f, -0.4f},
                {-0.2f, 0.05f},
                {0.15f, 0.1f},
                {0.5f, 1.0f},
                {1, 1.2f}
            });

            RockNoise = NextNoise();

            RockColorNoise = NextNoise();
            RockColorNoise.Frequency = 0.05f;
            RockColorNoise.YScale = 4.0f;
            RockColorNoise.Amplitude = 1.0f;

            GrassNoise = NextNoise();

            TreeNoise = NextPerlin();

            TreeRandom = NextRandom();
            WaterfallRandom = NextRandom();

            WaterfallNoise = NextNoise();
            WaterfallNoise.Frequency = 0.005f;

            StoneNoise = NextNoise();
            StoneNoise.Frequency = 0.01f;
            StoneNoise.Amplitude = 0.5f;
            StoneNoise2 = NextNoise();
            StoneNoise2.Frequency = 0.005f;
            StoneNoise2.YScale = 4.0f;
            StoneNoise2.Amplitude = 1.2f;

            RiverNoise = NextNoise();
            RiverNoise.Type = NoiseType.FBM;
            RiverNoise.YScale = 0.2f;
            RiverNoise.Frequency = 0.05f;
            RiverNoise.Persistence = 0.5f;
            RiverNoise.Octaves = 7;
            RiverNoise.Amplitude = 4f;

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

            GrassNoise.Frequency = 0.01f;
            GrassNoise.Amplitude = 2.0f;

            RockNoise.Frequency = 0.02f;
            RockNoise.Amplitude = 1.5f;

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
