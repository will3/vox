using UnityEngine;

namespace FarmVox
{
    public class ArcherSpriteSheet : SpriteSheet
    {
        public ArcherSpriteSheet()
        {
            idle = new string[] { "blue_0" };
            walk = NamesWithPrefix("blue", 2);
            walkFrameRate = 8.0f;
            scale = new Vector3(0.7f, 1.0f, 0.7f) * 20.0f;
        }
    }
}
