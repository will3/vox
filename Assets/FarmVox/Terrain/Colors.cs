using System;
using FarmVox.Editor;
using FarmVox.GPU;
using UnityEngine;
using UnityEngine.Serialization;

namespace FarmVox.Terrain
{
    [Serializable]
    public class Colors
    {
        [FormerlySerializedAs("RockColorGradient")] public ColorGradient RockColor;
        
        [ColorHtmlProperty]
        public Color TrunkColor;
        
        [ColorHtmlProperty]
        public Color LeafColor;
        
        [ColorHtmlProperty]
        public Color WaterColor;
        
        public ColorGradient GrassColor;

        public Colors()
        {        
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
            WaterColor = ColorUtils.GetColor("#297eb6");
            WaterColor.a = 0.4f;

            RockColor = new ColorGradient(ColorUtils.GetColor("#654d1f"));
            GrassColor = new ColorGradient(ColorUtils.GetColor("#597420"));
        }
    }
}
