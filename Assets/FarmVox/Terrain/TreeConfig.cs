using System;
using System.Collections.Generic;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Terrain
{
    [Serializable]
    public class TreeConfig
    {
        public Color trunkColor;
        public Color leafColor;
        
        public Perlin noise;
        public Random random;
        public ValueGradient heightGradient;
        public ValueGradient densityFilter;
        public float threshold = 0.4f;

        public TreeConfig()
        {
            noise = NoiseUtils.NextPerlin();

            random = NoiseUtils.NextRandom();

            noise.Frequency = 0.005f;
            noise.OctaveCount = 5;

            heightGradient = new ValueGradient(1, 0);

            densityFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {1, 1}
            });
            
            trunkColor = ColorUtils.GetColor("#4f402a");
            leafColor = ColorUtils.GetColor("#2f510c");
        }
    }
}