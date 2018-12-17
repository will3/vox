using System;
using System.Runtime.Remoting.Messaging;
using FarmVox.Editor;
using FarmVox.GPU;
using UnityEngine;

namespace FarmVox.Terrain
{
    [Serializable]
    public class Colors
    {
        public Gradient RockColorGradient;

        public int RockColorBanding;

        [ColorHtmlProperty] 
        public Color StoneColor;
        
        [ColorHtmlProperty]
        public Color TrunkColor;
        
        [ColorHtmlProperty]
        public Color LeafColor;
        
        [ColorHtmlProperty]
        public Color WaterColor;
        
        public Gradient GrassColor;
        
        public int GrassColorBanding;

        public Colors()
        {        
            StoneColor = ColorUtils.GetColor("#676767");
            TrunkColor = ColorUtils.GetColor("#4f402a");
            LeafColor = ColorUtils.GetColor("#2f510c");
            WaterColor = ColorUtils.GetColor("#297eb6");
            WaterColor.a = 0.4f;

            RockColorGradient = new Gradient();
            RockColorGradient.SetKeys(
                new []
                {
                    new GradientColorKey(ColorUtils.GetColor("#654d1f"), 0)
                }, 
                new []
                {
                    new GradientAlphaKey(1.0f, 0.0f), 
                });
            
            GrassColor = new Gradient();
            GrassColor.SetKeys(
                new[]
                {
                    new GradientColorKey(ColorUtils.GetColor("#597420"), 0)
                }, 
                new []
                {
                    new GradientAlphaKey(1.0f, 0.0f), 
                });
        }
    }
}
