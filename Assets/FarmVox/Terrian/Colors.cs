using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    [Serializable]
    public class Colors
    {
        public ColorGradient RockColorGradient;
        public Color StoneColor = GetColor("#676767");
        public Color TrunkColor = GetColor("#4f402a");
        public Color LeafColor = GetColor("#2f510c");
        public Color Soil = GetColor("#413535");
        public Color Road = GetColor("#bbaf4d");
        public Color Special = GetColor("#ff0000");
        public Color WaterColor = GetColor("#297eb6", 0.4f);
        public ColorGradient GrassColor;

        public Colors()
        {
            RockColorGradient = new ColorGradient(new Dictionary<float, Color> {
                    {0, GetColor("#654d1f")},
                    {1, GetColor("#654d1f")}
                })
            {
                Banding = 8
            };
            
            GrassColor = new ColorGradient(new Dictionary<float, Color> {
                {0, GetColor("#597420")},
                {1.0f, GetColor("#597420")}
            });
        }

        public static Color GetColor(string hex, float a = 1.0f)
        {
            Color color;
            ColorUtility.TryParseHtmlString(hex, out color);
            color.a = a;
            return color;
        }
    }
}
