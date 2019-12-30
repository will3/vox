using UnityEngine;

namespace FarmVox.Scripts
{
    public static class ActorSpriteSheetLoader
    {
        public static ActorSpriteSheet Load(TextAsset text)
        {
            var json = text.text;

            var spriteSheet = JsonUtility.FromJson<ActorSpriteSheet>(json);
            spriteSheet.AssertSpriteSheetValid();

            return spriteSheet;
        }
    }
}