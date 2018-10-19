using System.Collections.Generic;
using UnityEngine;

public partial class Card
{
    static class Textures
    {
        private static readonly Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();

        public static Texture2D Load(string name)
        {
            if (TextureMap.ContainsKey(name)) {
                return TextureMap[name];
            }

            TextureMap[name] = Resources.Load<Texture2D>("Textures/" + name);
            return TextureMap[name];
        }
    }
}
