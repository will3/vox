using System.IO;
using UnityEngine;

namespace FarmVox.Scripts
{
    public static class ActorSpriteSheetLoader
    {
        public static ActorSpriteSheet Load(string name)
        {
            var path = Path.Combine("spritesheets", name);
            var asset = Resources.Load<TextAsset>(path);
            var json = asset.text;

            var spriteSheet = JsonUtility.FromJson<ActorSpriteSheet>(json);
            spriteSheet.AssertSpriteSheetValid();

            return spriteSheet;
        }
    }
}