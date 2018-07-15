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

        static Colors()
        {
            ColorUtility.TryParseHtmlString("#509644", out grass);
            ColorUtility.TryParseHtmlString("#727D75", out rock);
            //ColorUtility.TryParseHtmlString("#A48474", out rock);
            ColorUtility.TryParseHtmlString("#5A81AD", out water);
            ColorUtility.TryParseHtmlString("#3D3329", out trunk);
            //ColorUtility.TryParseHtmlString("#FF0000", out trunk);
            ColorUtility.TryParseHtmlString("#3C5A30", out leaf);
            //ColorUtility.TryParseHtmlString("#E1952B", out leaf);

            ColorUtility.TryParseHtmlString("#674B3D", out roof);
            ColorUtility.TryParseHtmlString("#663D2D", out brick);
        }
    }
}