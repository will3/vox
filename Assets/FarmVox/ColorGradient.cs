using System;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox
{
    [Serializable]
    public class ColorGradient
    {
        public Gradient Gradient;
        public int Banding;

        public ColorGradient(Color color)
        {
            Gradient = GradientUtils.CreateGradient(color);
        }
    }
}