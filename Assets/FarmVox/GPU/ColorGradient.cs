using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.GPU
{
    [Serializable]
    public class ColorGradient
    {
        public Gradient Gradient;

        public int Banding;

        public int Count {
            get { return Gradient.colorKeys.Length; }
        }

        public void SetColors(Dictionary<float, Color> colors)
        {
            Gradient.SetKeys(new GradientColorKey[2], new GradientAlphaKey[2]);
        }

        public Color[] GetValues()
        {
            return Gradient.colorKeys.Select(u => u.color).ToArray();
        }

        public float[] GetKeys()
        {
            return Gradient.colorKeys.Select(u => u.time).ToArray();
        }
	}
}