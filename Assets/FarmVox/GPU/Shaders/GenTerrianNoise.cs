﻿using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public struct GenTerrianNoise
    {
        public float Height { get; set; }
        
        public Color RockColor { get; set; }
        
        public float Grass { get; set; }
        
        public float Stone { get; set; }
        
        public float Stone2 { get; set; }

        public static int Stride
        {
            get
            {
                return sizeof(float) * 4 + 
                       sizeof(float) * 4;
            }
        }
    }
}