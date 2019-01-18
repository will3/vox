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
        public Color TrunkColor;
        
        public Color LeafColor;
        
        public Perlin TreeNoise;
        public Random TreeRandom;
        public ValueGradient TreeHeightGradient;
        public float TreeAmount;
        public ValueGradient TreeDensityFilter;

        public TreeConfig()
        {
            TreeNoise = NoiseUtils.NextPerlin();

            TreeRandom = NoiseUtils.NextRandom();
            TreeAmount = 2.0f;
            
            TreeNoise.Frequency = 0.05f;
            TreeNoise.OctaveCount = 5;

            TreeHeightGradient = new ValueGradient(1, 0);

            TreeDensityFilter = new ValueGradient(new Dictionary<float, float>
            {
                {-1, 0},
                {1, 1}
            });
            
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
        }
    }
}