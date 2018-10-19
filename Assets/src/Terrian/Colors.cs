using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Colors
    {
        public ColorGradient rockColorGradient;
        public Color stoneColor = GetColor("#676767");
        public Color trunkColor = GetColor("#4f402a");
        public Color leafColor = GetColor("#2f510c");
        public Color soil = GetColor("#413535");
        public Color road = GetColor("#bbaf4d");
        public Color special = GetColor("#ff0000");
        public Color waterColor;
        public ColorGradient grassColor;

        public Colors()
        {
            rockColorGradient = new ColorGradient(new Dictionary<float, Color> {
                    {0, GetColor("#654d1f")},
                    {1, GetColor("#654d1f")}
                })
            {
                Banding = 8
            };
            grassColor = new ColorGradient(new Dictionary<float, UnityEngine.Color> {
                {0, GetColor("#597420")},
                {1.0f, GetColor("#597420")},
                //{1, grass3}
            });
            waterColor = Colors.GetColor("#297eb6");
            waterColor.a = 0.4f;
        }

        public static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}
