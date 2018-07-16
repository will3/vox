using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class ColorGradient
    {
        private Dictionary<float, Color> map = new Dictionary<float, Color>();
        private List<float> keys = new List<float>();

        public ColorGradient(Color a, Color b)
        {
            keys.Add(0);
            keys.Add(1);
            map[0] = a;
            map[1] = b;
        }

        public void Add(float position, Color v)
        {
            map[position] = v;
            keys.Add(position);
            keys.Sort();
        }

        public Color GetValue(float ratio)
        {
            for (int i = 0; i < keys.Count - 1; i++)
            {
                float min = keys[i];
                float max = keys[i + 1];

                if (max > ratio)
                {
                    var minV = map[min];
                    var maxV = map[max];
                    var r = (ratio - min) / (max - min);

                    return Color.Lerp(minV, maxV, r);
                }
            }

            throw new Exception("ratio must be between 0 and 1");
        }
    }
}