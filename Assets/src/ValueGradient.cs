using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueGradient
{
    private readonly Dictionary<float, float> map = new Dictionary<float, float>();
    private readonly List<float> keys = new List<float>();

    public ValueGradient() {
        map[0.0f] = 0.0f;
        map[1.0f] = 1.0f;

        keys.Add(0.0f);
        keys.Add(1.0f);
    }

    public void Add(float position, float v) {
        map[position] = v;
        keys.Add(position);
        keys.Sort();
    }

    public float GetValue(float ratio) {
        for (int i = 0; i < keys.Count - 1; i++) {
            float min = keys[i];
            float max = keys[i + 1];

            if (max > ratio) {
                float minV = map[min];
                float maxV = map[max];
                var r = (ratio - min) / (max - min);

                return minV + (maxV - minV) * r;
            }
        }

        return 0.0f;
    }
}
