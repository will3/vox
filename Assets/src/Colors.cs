using UnityEngine;
using System.Collections;

namespace FarmVox
{

    public static class Colors
    {
        public static Color grass;
        public static Color rock;
        public static Color water;
        public static Color trunk;
        public static Color leaf;
        public static Color roof;
        public static Color brick;
        public static Color road;
        public static Color special;

        public static ColorGradient grassGradient;

        //
        static Colors()
        {
            grass = GetColor("#509644");
            rock = GetColor("#413535");
            water = GetColor("#5A81AD");
            trunk = GetColor("#4f402a");
            leaf = GetColor("#337828");
            roof = GetColor("#BD5222");
            brick = GetColor("#52575A");
            //road = GetColor("#74614c");
            road = GetColor("#bbaf4d");
            special = GetColor("#ff0000");

            grassGradient = new ColorGradient(rock, grass);
            //grassGradient.Add(0.5f, grass);
        }

        private static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}