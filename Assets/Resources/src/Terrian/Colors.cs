using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{

    public static class Colors
    {
        public static Color soil;
        public static Color road;
        public static Color special;

        static Colors()
        {
            //var grass3 = GetColor("#cec479");
            //var grass = GetColor("#447c3e");
             //var grass2 = GetColor("#285224");

            soil = GetColor("#413535");
            road = GetColor("#bbaf4d");
            special = GetColor("#ff0000");

            //rock
            //{ 0, GetColor("#cec479")},
                //{ 1, GetColor("#a77757")}

        }

        public static Color GetColor(string hex)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(hex, out color);
            return color;
        }
    }
}