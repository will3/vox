using UnityEngine;
using System.Collections;

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

            grassGradient = new ColorGradient(rock, grass);
            grassGradient.banding = 3;
            //grassGradient.Add(0.5f, grass);
        }

        public static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}