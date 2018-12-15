using System.Globalization;
using UnityEngine;

namespace FarmVox.Terrain
{
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