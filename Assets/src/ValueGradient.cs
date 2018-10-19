using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueGradient
{
    private readonly Dictionary<float, float> _map = new Dictionary<float, float>();
    private readonly List<float> _keys = new List<float>();

    public List<float> Keys
    {
        get
        {
            return _keys;
        }
    }

    public List<float> Values {
        get 
        {
            var list = new List<float>();
            foreach (var key in _keys) {
                list.Add(_map[key]);
            }
            return list;
        }
    }

    public int banding = 0;

    public ValueGradient(float min, float max) {
        _map[0.0f] = min;
        _map[1.0f] = max;

        _keys.Add(0.0f);
        _keys.Add(1.0f);
    }

    public ValueGradient(Dictionary<float, float> map) {
        foreach(var kv in map) {
            Set(kv.Key, kv.Value);
        }
    }

    public void Set(float position, float v) {
        bool existing = _map.ContainsKey(position);
        _map[position] = v;
        if (!existing) {
            _keys.Add(position);
            _keys.Sort();    
        }
    }

    public float GetValue(float ratio) {
        if (banding > 0) {
            ratio = Mathf.Floor(ratio * (float)banding) / (float)banding;
        }
        for (int i = 0; i < _keys.Count - 1; i++) {
            float min = _keys[i];
            float max = _keys[i + 1];

            if (max > ratio) {
                float minV = _map[min];
                float maxV = _map[max];
                var r = (ratio - min) / (max - min);

                return minV + (maxV - minV) * r;
            }
        }

        return 0.0f;
    }

    public ValueGradientBuffers CreateBuffers(ComputeShader shader, string prefix) {
        var keysBuffer = new ComputeBuffer(Keys.Count, sizeof(float));
        keysBuffer.SetData(Keys);

        var valuesBuffer = new ComputeBuffer(Keys.Count, sizeof(float));
        valuesBuffer.SetData(Values);

        shader.SetBuffer(0, prefix + "Keys", keysBuffer);
        shader.SetBuffer(0, prefix + "Values", valuesBuffer);
        shader.SetInt(prefix + "Size", Keys.Count);

        return new ValueGradientBuffers(keysBuffer, valuesBuffer);
    }

    public class ValueGradientBuffers : System.IDisposable {
        public readonly ComputeBuffer KeysBuffer;
        public readonly ComputeBuffer ValuesBuffer;

        public ValueGradientBuffers(ComputeBuffer keysBuffer, ComputeBuffer valuesBuffer)
        {
            KeysBuffer = keysBuffer;
            ValuesBuffer = valuesBuffer;
        }

        public void Dispose()
        {
            KeysBuffer.Dispose();
            ValuesBuffer.Dispose();
        }
    }
}
