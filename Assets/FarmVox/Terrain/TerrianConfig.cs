using System;
using UnityEngine;

namespace FarmVox.Terrain
{
    [Serializable]
    public class TerrianConfig
    {
        public int ActualWaterLevel {
            get {
                return WaterLevel + GroundHeight;
            }
        }

        public int Size;

        public int DataSize
        {
            get
            {
                return Size + 3;
            }
        }
        
        public float MaxHeight;
        public float PlainHeight;
        public int WaterLevel;
        public int GroundHeight;
        public float AoStrength;
        public int TreeMinDis;

        public BiomeConfig Biome;

        public TerrianConfig()
        {
            Size = 32;
            MaxHeight = 64;
            PlainHeight = 4;
            WaterLevel = 0;
            GroundHeight = 12;
            AoStrength = 0.15f;
            TreeMinDis = 3;

            Biome = new BiomeConfig();
        }
    }
}
