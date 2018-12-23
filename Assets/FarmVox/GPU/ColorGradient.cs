using UnityEngine;

namespace FarmVox.GPU
{
    public class ColorGradient
    {
        public float[] Keys;
        public Vector4[] Colors;
        public int Banding;

        public ColorGradient(Color color)
        {
            Keys = new[] {0.0f};
            Colors = new[] {(Vector4) color};
        }
    }
}