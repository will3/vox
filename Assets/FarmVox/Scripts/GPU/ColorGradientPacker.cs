using UnityEngine;

namespace FarmVox.Scripts.GPU
{
    public static class ColorGradientPacker
    {
        public static Color[] PackColorGradient(ColorGradient colorGradient, int segments = 32)
        {
            var results = new Color[segments + 1];
            for (var i = 0; i <= segments; i++)
            {
                var t = i / (float) segments;
                var color = colorGradient.GetColor(t);
                results[i] = color;
            }

            return results;
        }
    }
}