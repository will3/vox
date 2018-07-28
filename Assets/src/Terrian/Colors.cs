using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{

    public static class Colors
    {
        public static Color grass;
        public static Color grass2;
        public static Color soil;
        public static Color water;
        public static Color trunk;
        public static Color leaf;
        public static Color road;
        public static Color special;

        public static ColorGradient grassGradient;

        static Colors()
        {
            var grass3 = GetColor("#cec479");
            grass = GetColor("#447c3e");
            grass2 = GetColor("#285224");

            soil = GetColor("#413535");
            water = GetColor("#5A81AD");
            trunk = GetColor("#4f402a");
            leaf = GetColor("#295e21");
            road = GetColor("#bbaf4d");
            special = GetColor("#ff0000");

            grassGradient = new ColorGradient(new Dictionary<float, Color> {
                {0, grass3},
                {0.2f, grass},
                //{0.79f, grass},
                //{0.8f, grass2},
                {1, grass}
            });

            grassGradient.banding = 8;
        }

        public static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}