using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Scripts
{
    static class Textures
    {
        private static readonly Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();

        public static Texture2D Load(string name)
        {
            if (TextureMap.ContainsKey(name)) {
                return TextureMap[name];
            }

            var texture = Resources.Load<Texture2D>("Textures/" + name);

            if (texture == null)
            {
                throw new Exception("Cannot load texture " + name);
            }

            TextureMap[name] = texture;

            return texture;
        }
    }
}
