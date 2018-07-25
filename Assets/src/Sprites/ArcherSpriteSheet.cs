using UnityEngine;

namespace FarmVox
{
    public class ArcherSpriteSheet : SpriteSheet
    {
        public ArcherSpriteSheet()
        {
            idle = new string[] { "blue_0" };
            walk = SpriteSheets.NamesWithPrefix("blue", 2);
            scale = new Vector3(0.7f, 1.0f, 0.7f) * 14.0f;
        }
    }
}
