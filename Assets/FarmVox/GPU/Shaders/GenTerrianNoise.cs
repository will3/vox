namespace FarmVox
{
    struct GenTerrianNoise
    {
        public float Height { get; set; }
        public float RockColor { get; set; }
        public float Grass { get; set; }
        public float River { get; set; }
        public float Stone { get; set; }
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