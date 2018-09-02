using UnityEngine;

namespace FarmVox
{
    public class SpriteSheet
    {
        protected string[] idle = { };

        protected string[] walk = { };
        protected float walkFrameRate = 1.0f;

        protected string[] attack = { };
        protected string prefix = "";
        protected Vector3 scale = new Vector3(1, 1, 1);

        float frameCounter = 0;

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
        }

        protected string[] NamesWithPrefix(string name, int count)
        {
            var list = new string[count];
            for (var i = 0; i < count; i++)
            {
                list[i] = name + "_" + i;
            }
            return list;
        }

        string currentTexture;

        public string CurrentTexture
        {
            get
            {
                return currentTexture;
            }
        }

        public void Walk(float factor = 1.0f) {
            var total = walk.Length;
            var frame = Mathf.FloorToInt(frameCounter * walkFrameRate);
            frame %= total;
            currentTexture = walk[frame];
            frameCounter += (Time.deltaTime * factor);
        }
    }
}
