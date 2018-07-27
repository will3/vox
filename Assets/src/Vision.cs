using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Vision
    {
        static Vision instance;

        public static Vision Instance {
            get
            {
                if (instance == null)
                {
                    instance = new Vision();
                }
                return instance;
            }
        }

        HashSet<VisionSource> sources = new HashSet<VisionSource>();
        public int size = 32;

        public void AddSource(VisionSource source)
        {
            sources.Add(source);
        }

        public void RemoveSource(VisionSource source)
        {
            sources.Remove(source);
        }

        bool dirty = true;
        public void UpdateMap(VisionMap map) {
            if (!dirty) {
                return;
            }
            var pattern = CirclePattern.GetCirlce(200.0f, 40.0f);
            map.Clear();
            foreach (var source in sources) {
                var position = source.gameObject.transform.position;

                foreach (var kv in pattern)
                {
                    var c = new Vector2(position.x, position.z) + kv.Key;
                    map.Set(c, kv.Value);
                }
            }

            dirty = false;
        }
    }
}