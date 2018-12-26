using UnityEngine;

namespace FarmVox.GPU
{
    public static class ColorExtensions
    {
        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }
    }
}