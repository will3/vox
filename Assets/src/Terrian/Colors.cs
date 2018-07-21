using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{

    public static class Colors
    {
        public static Color grass;
        public static Color soil;
        public static Color rock;
        public static Color water;
        public static Color trunk;
        public static Color leaf;
        public static Color roof;
        public static Color brick;
        public static Color road;
        public static Color special;

        public static ColorGradient grassGradient;
        public static ColorGradient rockColorGradient;

        static Colors()
        {
            grass = GetColor("#457828");
            soil = GetColor("#413535");
            water = GetColor("#5A81AD");
            trunk = GetColor("#4f402a");
            leaf = GetColor("#295e21");
            roof = GetColor("#BD5222");
            brick = GetColor("#52575A");
            road = GetColor("#bbaf4d");
            special = GetColor("#ff0000");
            rock = GetColor("#6b615f");
            rock = GetColor("#cec479");

            //rock = GetColor("#555555");

            grassGradient = new ColorGradient(grass, grass);
            grassGradient.banding = 6;
            //grassGradient.Add(0.5f, grass);

            rockColorGradient = new ColorGradient(new Dictionary<float, Color>() {
                {0, GetColor("#7e752f")},
                {0.8f, GetColor("#cec479")},
                {1, GetColor("#cec479")}
            });

            rockColorGradient.banding = 8;
        }

        public static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}