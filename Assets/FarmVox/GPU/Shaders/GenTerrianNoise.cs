using JetBrains.Annotations;

namespace FarmVox.GPU.Shaders
{
    public struct GenTerrianNoise
    {
        [UsedImplicitly]
        public float Height { get; set; }
        
        [UsedImplicitly]
        public float RockColor { get; set; }
        
        [UsedImplicitly]
        public float Grass { get; set; }
        
        [UsedImplicitly]
        public float River { get; set; }
        
        [UsedImplicitly]
        public float Stone { get; set; }
        
        [UsedImplicitly]
        public float Stone2 { get; set; }

        public static int Stride
        {
            get
            {
                return sizeof(float) * 6;
            }
        }
    }
}