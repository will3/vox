using UnityEngine;
using System.Collections;

public static class Colors
{
    public static Color grass;
    public static Color rock;
    public static Color water;
    public static Color trunk;
    public static Color leaf;
    public static Color roof;
    public static Color wall;

    static Colors() {
        ColorUtility.TryParseHtmlString("#63912C", out grass);
        //ColorUtility.TryParseHtmlString("#727D75", out rock);
        ColorUtility.TryParseHtmlString("#A48474", out rock);
        ColorUtility.TryParseHtmlString("#5A81AD", out water);
        ColorUtility.TryParseHtmlString("#3D3329", out trunk);
        //ColorUtility.TryParseHtmlString("#FF0000", out trunk);
        ColorUtility.TryParseHtmlString("#2B4E16", out leaf);
        //ColorUtility.TryParseHtmlString("#E1952B", out leaf);

        ColorUtility.TryParseHtmlString("#713F16", out roof);
        ColorUtility.TryParseHtmlString("#B0B5B9", out wall);
    }
}
