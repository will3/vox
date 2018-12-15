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
        public int Banding = 0;

        public ValueGradient(float min, float max)
        {
            Keys = new[] {0.0f, 1.0f};
            Values = new[] {min, max};
        }

        public ValueGradient()
        {
            Keys = new[] {0.0f, 1.0f};
            Values = new[] {0.0f, 1.0f};
        }
        
        public ValueGradient(Dictionary<float, float> map)
        {
            Keys = map.Keys.ToArray();
            Values = map.Values.ToArray();
        }

        public float GetValue(float ratio)
        {
            if (Banding > 0)
            {
                ratio = Mathf.Floor(ratio * Banding) / Banding;
            }

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

        public ValueGradientBuffers CreateBuffers(ComputeShader shader, string prefix)
        {
            var keysBuffer = new ComputeBuffer(Keys.Length, sizeof(float));
            keysBuffer.SetData(Keys);

            var valuesBuffer = new ComputeBuffer(Keys.Length, sizeof(float));
            valuesBuffer.SetData(Values);

            shader.SetBuffer(0, prefix + "Keys", keysBuffer);
            shader.SetBuffer(0, prefix + "Values", valuesBuffer);
            shader.SetInt(prefix + "Size", Keys.Length);

            return new ValueGradientBuffers(keysBuffer, valuesBuffer);
        }

        public class ValueGradientBuffers : System.IDisposable
        {
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
}