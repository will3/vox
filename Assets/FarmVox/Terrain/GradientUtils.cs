using UnityEngine;

namespace FarmVox.Terrain
{
    public static class GradientUtils
    {
        public static Gradient CreateGradient(Color color)
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new [] { new GradientColorKey(color, 0.0f) }, 
                new [] { new GradientAlphaKey(1.0f, 0.0f) });
            return gradient;
        }
    }
}