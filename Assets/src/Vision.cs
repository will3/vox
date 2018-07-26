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

        private bool dirty = true;
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

    class CirclePattern {
        public static Dictionary<Vector2Int, float> GetCirlce(float diameter, float blurDis = 2) {
            var loopSize = Mathf.CeilToInt(diameter);
            if (loopSize % 2 == 1) {
                loopSize += 1;
            }

            var radius = diameter / 2.0f;
            var solidDis = radius - blurDis;

            var map = new Dictionary<Vector2Int, float>();

            var mid = loopSize / 2;
            for (var i = 0; i < loopSize; i++) {
                for (var j = 0; j < loopSize; j++) {
                    var x = i - mid;
                    var y = j - mid;

                    var dis = Mathf.Sqrt(x * x + y * y);

                    var coord = new Vector2Int(x, y);
                    if (dis < solidDis) {
                        map[coord] = 1.0f;
                    } else if (dis < radius) {
                        var ratio = 1 - (dis - solidDis) / blurDis;
                        map[coord] = ratio;
                    }
                }
            }

            return map;
        }
    }
}