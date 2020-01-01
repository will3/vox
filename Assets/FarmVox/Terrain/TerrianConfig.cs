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
        
        public int MaxChunksY;
        public float MaxHeight;
        public float PlainHeight;
        public int WaterLevel;
        public int GroundHeight;
        public int MaxChunksX;
        public float AoStrength;
        public int TreeMinDis;

        public BiomeConfig Biome;

        public TerrianConfig()
        {
            Size = 32;
            MaxChunksY = 2;
            MaxHeight = 64;
            PlainHeight = 4;
            WaterLevel = 0;
            GroundHeight = 12;
            MaxChunksX = 3;
            AoStrength = 0.15f;
            TreeMinDis = 3;

            Biome = new BiomeConfig();
        }

        public BoundsInt BoundsInt {
            get {
                var boundingCubeSize = new Vector3Int(MaxChunksX, MaxChunksX, MaxChunksX) * Size;
                return  new BoundsInt(boundingCubeSize * -1, boundingCubeSize * 2);
            }
        }
    }
}
