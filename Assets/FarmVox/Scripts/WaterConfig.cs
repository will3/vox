using System;
using UnityEngine;

namespace FarmVox.Scripts
{
    [Serializable]
    public class WaterConfig
    {
        public Color waterColor = ColorUtils.GetColor("#297eb6", 0.4f);
        public int waterLevel = 12;
        public float opacity = 0.7f;
    }
}