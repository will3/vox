using System.Collections.Generic;
using UnityEngine;

public partial class Card
{
    static class Textures
    {
        private static Dictionary<string, Texture2D> textureMap = new Dictionary<string, Texture2D>();

        public static Texture2D Load(string name)
        {
            if (textureMap.ContainsKey(name)) {
                return textureMap[name];
            }

            textureMap[name] = Resources.Load<Texture2D>("Textures/" + name);
            return textureMap[name];
        }
    }
}
