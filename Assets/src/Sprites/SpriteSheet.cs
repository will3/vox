using UnityEngine;

namespace FarmVox
{
    class SpriteSheet
    {
        public string[] idle = { };
        public string[] walk = { };
        public string[] attack = { };
        public string prefix = "";
        public Vector3 scale = new Vector3(1, 1, 1);
        public float walkFrameRate = 1.0f;
    }
}
