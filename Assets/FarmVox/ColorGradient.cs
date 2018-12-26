using System;
using UnityEngine;

namespace FarmVox
{
    [Serializable]
    public class ColorGradient
    {
        public float[] Keys;
        public Color[] Colors;
        public int Banding;

        public ColorGradient(Color color)
        {
            Keys = new[] {0.0f};
            Colors = new[] { color};
        }
    }
}