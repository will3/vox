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
        public float ShadowStrength;
        public int TreeMinDis;
        public float NormalStrength;
        public float TreesNormalStrength;
        
        public float WaterfallShadowStrength;
        public float WaterfallSpeed;
        public float WaterfallWidth;
        public float WaterfallMin;
        public float WaterfallVariance;

        public float WallShadowStrength;
        public float WallNormalStrength;
        
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
            ShadowStrength = 0.5f;
            WaterfallShadowStrength = 0.2f;
            TreeMinDis = 3;
            NormalStrength = 0.4f;
            TreesNormalStrength = 0.2f;

            WaterfallSpeed = 2.0f;
            WaterfallWidth = 10.0f;
            WaterfallMin = 0.9f;
            WaterfallVariance = 0.7f;
        
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
