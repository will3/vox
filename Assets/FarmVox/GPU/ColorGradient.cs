using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.GPU
{
    public class ColorGradient
    {
        private readonly Dictionary<float, Color> _map = new Dictionary<float, Color>();
        private readonly List<float> _keys = new List<float>();
        
        public int Banding { get; set; }

        public int Count {
            get { return _map.Count; }
        }

        public ColorGradient(Color a, Color b)
        {
            _keys.Add(0);
            _keys.Add(1);
            _map[0] = a;
            _map[1] = b;
        }

        public ColorGradient(Dictionary<float, Color> map) {
            foreach(var kv in map) {
                Add(kv.Key, kv.Value);
            }
        }

        private void Add(float position, Color v)
        {
            _map[position] = v;
            _keys.Add(position);
            _keys.Sort();
        }

        public Color GetValue(float ratio)
        {
            if (ratio < 0.0) { ratio = 0.0f; }
            if (ratio > 1.0) { ratio = 1.0f; }

            if (Banding > 0)
            {
                ratio = Mathf.Floor(ratio * Banding) / Banding;
            }

            for (var i = 0; i < _keys.Count - 1; i++)
            {
                var min = _keys[i];
                var max = _keys[i + 1];

                if (!(max >= ratio)) continue;
                
                var minV = _map[min];
                var maxV = _map[max];
                var r = (ratio - min) / (max - min);

                return Color.Lerp(minV, maxV, r);
            }

            throw new Exception("ratio must be between 0 and 1");
        }

        public Color[] GetValues() {
            var list = new List<Color>();
            foreach(var key in _keys) {
                list.Add(_map[key]);
            }
            return list.ToArray();
        }

        public float[] GetKeys() {
            return _keys.ToArray();
        }
	}
}