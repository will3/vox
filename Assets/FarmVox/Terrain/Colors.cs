﻿using System;
using FarmVox.Editor;
using FarmVox.GPU;
using UnityEngine;

namespace FarmVox.Terrain
{
    [Serializable]
    public class Colors
    {
        public ColorGradient2 RockColorGradient;

        public int RockColorBanding;

        [ColorHtmlProperty] 
        public Color StoneColor;
        
        [ColorHtmlProperty]
        public Color TrunkColor;
        
        [ColorHtmlProperty]
        public Color LeafColor;
        
        [ColorHtmlProperty]
        public Color WaterColor;
        
        public ColorGradient2 GrassColor;
        
        public int GrassColorBanding;

        public Colors()
        {        
            StoneColor = ColorUtils.GetColor("#676767");
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
            WaterColor = ColorUtils.GetColor("#297eb6");
            WaterColor.a = 0.4f;

            RockColorGradient = new ColorGradient2(ColorUtils.GetColor("#654d1f"));
            GrassColor = new ColorGradient2(ColorUtils.GetColor("#597420"));
        }
    }
}
