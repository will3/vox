using System;
using System.Globalization;
using FarmVox.GPU;
using UnityEngine;

namespace FarmVox.Terrain
{
    [Serializable]
    public class Colors
    {
        public ColorGradient RockColorGradient;

        [ColorHtmlProperty] 
        public Color StoneColor;
        
        [ColorHtmlProperty]
        public Color TrunkColor;
        
        [ColorHtmlProperty]
        public Color LeafColor;
        
        [ColorHtmlProperty]
        public Color Soil;
        
        [ColorHtmlProperty]
        public Color WaterColor;
        
        public ColorGradient GrassColor;

        public Colors()
        {        
            StoneColor = ColorUtils.GetColor("#676767");
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
            Soil = ColorUtils.GetColor("#413535");
            WaterColor = ColorUtils.GetColor("#297eb6"); //, 0.4f);

            RockColorGradient = new ColorGradient();
            RockColorGradient.Gradient.SetKeys(
                new []{new GradientColorKey(ColorUtils.GetColor("#654d1f"), 0) }, 
                new GradientAlphaKey[2]);
            
            GrassColor = new ColorGradient();
            GrassColor.Gradient.SetKeys(
                new[] {new GradientColorKey(ColorUtils.GetColor("#597420"), 0)},
                new GradientAlphaKey[2]);
        }
    }

    public static class ColorUtils
    {
        public static Color GetColor(string hex)
        {    
            hex = hex.TrimStart('#');

            if (hex.Length == 6)
            {
                return new Color32(byte.Parse(hex.Substring(0,2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                    byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                    255);
            }

            return new Color32(byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
        }
    }
}
