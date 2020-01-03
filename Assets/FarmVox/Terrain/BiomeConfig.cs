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
        public float HillHeight;
        
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

        public ValueGradient heightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {-1.0f, -0.27f },
            {-0.2f, 0.05f },
            {0.15f, 0.1f },
            {0.5f, 0.75f },
            {1, 1 }
        });
        
        public BiomeConfig(int seed = 1337)
        {
            HillHeight = 48;
            
            NoiseUtils.SetSeed(seed);
            HeightNoise = NoiseUtils.NextNoise();
            HeightNoise.Frequency = 0.015f;
            HeightNoise.YScale = 0.4f;
            HeightNoise.Octaves = 5;

            RockNoise = NoiseUtils.NextNoise();

            RockColorNoise = NoiseUtils.NextNoise();
            RockColorNoise.Frequency = 0.05f;
            RockColorNoise.YScale = 4.0f;
            RockColorNoise.Amplitude = 1.0f;

            GrassNoise = NoiseUtils.NextNoise();

            GrassNoise.Frequency = 0.01f;
            GrassNoise.Amplitude = 2.0f;

            RockNoise.Frequency = 0.02f;
            RockNoise.Amplitude = 1.5f;

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
            
            WaterColor = ColorUtils.GetColor("#297eb6");
            WaterColor.a = 0.4f;

            RockColor = new ColorGradient(ColorUtils.GetColor("#654d1f"));
            GrassColor = new ColorGradient(ColorUtils.GetColor("#597420"));
        }
    }
}