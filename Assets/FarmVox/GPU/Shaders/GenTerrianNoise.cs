using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public struct GenTerrianNoise
    {
        public float Height { get; set; }
        
        public float RockColor { get; set; }
        
        public float Grass { get; set; }

        public static int Stride
        {
            get { return sizeof(float) * 3; }
        }
    }
}