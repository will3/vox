﻿using System;
using System.Collections.Generic;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Terrain
{
    [Serializable]
    public class BiomeConfig
    {
        public Colors Colors;
        
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
        public float TreeAmount;
        public ValueGradient TreeDensityFilter;
        #endregion

        #region waterfall
        public Random WaterfallRandom;
        public Noise WaterfallNoise;
        public ValueGradient WaterfallHeightFilter;
        public float WaterfallChance;
        #endregion

        #region river
        public Noise RiverNoise;
        public ValueGradient RiverNoiseFilter;
        #endregion

        #region stone
        public Noise StoneNoise;
        public Noise StoneNoise2;
        public float StoneThreshold;
        public ValueGradient StoneHeightFilter;
        #endregion
        
        private readonly Random _r;

        public ValueGradient HeightFilter;
        
        public BiomeConfig(int seed = 1337)
        {
            _r = new Random(seed);
            HeightNoise = NextNoise();
            HeightNoise.Frequency = 0.015f;
            HeightNoise.YScale = 0.4f;
            HeightNoise.Octaves = 5;

            HeightFilter = new ValueGradient(new Dictionary<float, float>
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
            TreeAmount = 2.0f;
            WaterfallRandom = NextRandom();

            WaterfallNoise = NextNoise();
            WaterfallNoise.Frequency = 0.005f;
            WaterfallChance = 0.01f;
            
            StoneNoise = NextNoise();
            StoneNoise.Frequency = 0.01f;
            StoneNoise.Amplitude = 0.5f;
            StoneNoise2 = NextNoise();
            StoneNoise2.Frequency = 0.005f;
            StoneNoise2.YScale = 4.0f;
            StoneNoise2.Amplitude = 1.2f;
            StoneThreshold = 0.5f;
            StoneHeightFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1.0f, 1.0f},
                {0f, 1.0f},
                {1.0f, 0.0f},
            });
            
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
    }
}