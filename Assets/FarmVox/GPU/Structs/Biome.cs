using UnityEngine;

namespace FarmVox.GPU.Structs
{
    public struct Biome
    {
        public float HillHeight;

        public float GroundHeight;

        public Vector4 WaterColor;

        public ColorGradient RockColor;

        public ColorGradient GrassColor;

        public ValueGradient GrassNormalFilter;

        public ValueGradient GrassHeightFilter;

        public ValueGradient HeightFilter;

        public int Stride
        {
            get { return sizeof(float) * 2 + sizeof(float) * 4 + ColorGradient.Stride * 2 + ValueGradient.Stride * 3; }
        }
    }
}