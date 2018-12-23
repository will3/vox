using UnityEngine;

namespace FarmVox.GPU
{
    public class ColorGradient2
    {
        public float[] Keys;
        public Vector4[] Colors;
        public int Banding;

        public ColorGradient2(Color color)
        {
            Keys = new[] {0.0f};
            Colors = new[] {(Vector4) color};
        }
    }
}