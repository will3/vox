using System;
using System.Collections.Generic;
using FarmVox.Scripts;
using LibNoise.Generator;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Terrain
{
    [Serializable]
    public class TreeConfig
    {
        public Color trunkColor = ColorUtils.GetColor("#4f402a");
        public Color leafColor = ColorUtils.GetColor("#2f510c");

        public Noise noise = new Noise
        {
            frequency = 0.005f,
            octaves = 5,
            amplitude = 0.5f
        };

        public ValueGradient heightGradient = new ValueGradient(1, 0);

        public ValueGradient densityFilter = new ValueGradient(new Dictionary<float, float>
        {
            {-1, 0},
            {1, 1}
        });
        
        public ValueGradient directionFilter = new ValueGradient(new Dictionary<float, float>
        {
            {1, 1},
            {0, 0}
        });

        public float bias = 0.4f;
        public int minDis = 4;

        public Random random = new Random();

        public int trunkHeight = 1;
        public float treeHeight = 10f;
        public float treeRadius = 3.0f;
        public float randomAmount = 1.0f;
    }
}