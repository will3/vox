﻿using System;
using System.Collections.Generic;
using FarmVox.Editor;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Terrain
{
    [Serializable]
    public class BiomeConfig
    {
        public float HillHeight;
        
        [ColorHtmlProperty]
        public Color TrunkColor;
        
        [ColorHtmlProperty]
        public Color LeafColor;
        
        [ColorHtmlProperty]
        public Color WaterColor;
        
        public ColorGradient RockColor;
        
        public ColorGradient GrassColor;
        
        #region ground
        public Noise HeightNoise;
        public Noise RockNoise;
        public Noise RockColorNoise;
        #endregion

        #region grass
        public Noise GrassNoise;
        public ValueGradient GrassHeightFilter;
        public ValueGradient GrassNormalFilter;
        public float GrassValue;
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
        
        private readonly Random _r;

        public ValueGradient HeightFilter;
        
        public BiomeConfig(int seed = 1337)
        {
            HillHeight = 48;
            
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
                { 0, 0.2f },
                { 0.5f, -0.5f},
                { 1, -1 } });

            GrassNormalFilter = new ValueGradient(
                new Dictionary<float, float>{
                    { -1, 0 },
                    { -0.5f, 0 },
                    { 1, 1 } });

            GrassValue = 1.0f;
            
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
            WaterColor = ColorUtils.GetColor("#297eb6");
            WaterColor.a = 0.4f;

            RockColor = new ColorGradient(ColorUtils.GetColor("#654d1f"));
            GrassColor = new ColorGradient(ColorUtils.GetColor("#597420"));
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