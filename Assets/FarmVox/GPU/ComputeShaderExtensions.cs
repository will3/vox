using System.Linq;
using UnityEngine;

namespace FarmVox.GPU
{
    public static class ComputeShaderExtensions {
        public static void SetColorGradient(this ComputeShader computeShader, ColorGradient colorGradient, string prefix)
        {
            computeShader.SetVectorArray(prefix + "Gradient", colorGradient.Colors.Select(u => u.ToVector4()).ToArray());
            computeShader.SetFloats(prefix + "GradientIntervals", colorGradient.Keys);
            computeShader.SetInt(prefix + "GradientSize", colorGradient.Keys.Length);
            computeShader.SetFloat(prefix + "GradientBanding", colorGradient.Banding);
        }

        public static void SetValueGradient(this ComputeShader computeShader, ValueGradient valueGradient, string prefix)
        {            
            computeShader.SetFloats(prefix + "Keys", PackFloats(valueGradient.Keys));
            computeShader.SetFloats(prefix + "Values", PackFloats(valueGradient.Values));
            computeShader.SetInt(prefix + "Size", valueGradient.Keys.Length);
        }
        
        private static float[] PackFloats(float[] floats)
        {
            var array = new float[floats.Length * 4];

            for (var i = 0; i < floats.Length; i++)
            {
                array[i * 4] = floats[i];
            }

            return array;
        }
    }
}