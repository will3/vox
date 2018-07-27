using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueGradient
{
    private readonly Dictionary<float, float> map = new Dictionary<float, float>();
    private readonly List<float> keys = new List<float>();

    public List<float> Keys
    {
        get
        {
            return keys;
        }
    }

    public List<float> Values {
        get 
        {
            var list = new List<float>();
            foreach (var key in keys) {
                list.Add(map[key]);
            }
            return list;
        }
    }

    public int banding = 0;

    public ValueGradient(float min, float max) {
        map[0.0f] = min;
        map[1.0f] = max;

        keys.Add(0.0f);
        keys.Add(1.0f);
    }

    public ValueGradient(Dictionary<float, float> map) {
        foreach(var kv in map) {
            Set(kv.Key, kv.Value);
        }
    }

    public void Set(float position, float v) {
        bool existing = map.ContainsKey(position);
        map[position] = v;
        if (!existing) {
            keys.Add(position);
            keys.Sort();    
        }
    }

    public float GetValue(float ratio) {
        if (banding > 0) {
            ratio = Mathf.Floor(ratio * (float)banding) / (float)banding;
        }
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
