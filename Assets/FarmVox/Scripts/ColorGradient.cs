using System;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class ColorGradient
    {
        public Gradient gradient;
        public int banding;
        public bool useSolidColor;
        public Color solidColor;

        public ColorGradient(Color color)
        {
            gradient = GradientUtils.CreateGradient(color);
        }

        public ColorGradient(string color) : this(ColorUtils.GetColor(color))
        {
        }

        public Color GetColor(float r)
        {
            return useSolidColor ? solidColor : gradient.Evaluate(r);
        }
    }
}