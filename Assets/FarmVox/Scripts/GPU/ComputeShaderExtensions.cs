using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.GPU
{
    public static class ComputeShaderExtensions
    {
        public static void SetColorGradient(this ComputeShader computeShader, string name, ColorGradient colorGradient)
        {
            var results = ColorGradientPacker.PackColorGradient(colorGradient).Select(x => x.ToVector4()).ToArray();
            computeShader.SetVectorArray(name, results);
        }

        public static void SetValueGradient(this ComputeShader computeShader, string name, ValueGradient valueGradient)
        {
            var results = ValueGradientPacker.PackValueGradient(valueGradient);
            computeShader.SetFloats(name, PackFloats(results));
        }

        private static float[] PackFloats(IList<float> floats)
        {
            var array = new float[floats.Count * 4];

            for (var i = 0; i < floats.Count; i++)
            {
                array[i * 4] = floats[i];
            }

            return array;
        }
    }
}