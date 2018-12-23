using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace FarmVox
{
    [Serializable]
    public class ValueGradient
    {
        public float[] Keys;
        public float[] Values;

        public ValueGradient(float min, float max)
        {
            Keys = new[] {0.0f, 1.0f};
            Values = new[] {min, max};
        }

        public ValueGradient(Dictionary<float, float> map)
        {
            Keys = map.Keys.ToArray();
            Values = map.Values.ToArray();
        }

        public float GetValue(float ratio)
        {
            for (var i = 0; i < Keys.Length - 1; i++)
            {
                var min = Keys[i];
                var max = Keys[i + 1];

                if (!(max > ratio)) continue;
                
                var minV = Values[i];
                var maxV = Values[i + 1];
                var r = (ratio - min) / (max - min);

                return minV + (maxV - minV) * r;
            }

            return 0.0f;
        }
    }
}