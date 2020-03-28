﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.GPU
{
    public static class ComputeShaderExtensions
    {
        public static void SetColorGradient(this ComputeShader computeShader, ColorGradient colorGradient,
            string prefix)
        {
            var colors = colorGradient.gradient.colorKeys.Select(u => u.color.ToVector4()).ToArray();
            var keys = colorGradient.gradient.colorKeys.Select(u => u.time).ToArray();

            if (colorGradient.useSolidColor)
            {
                colors = new[] {colorGradient.solidColor.ToVector4()};
                keys = new[] {0.0f};
            }

            computeShader.SetVectorArray(prefix + "Gradient", colors);
            computeShader.SetFloats(prefix + "GradientIntervals", PackFloats(keys));
            computeShader.SetInt(prefix + "GradientSize", keys.Length);
            computeShader.SetFloat(prefix + "GradientBanding", colorGradient.banding);
        }

        public static void SetValueGradient(this ComputeShader computeShader, ValueGradient valueGradient,
            string prefix)
        {
            var keys = valueGradient.Curve.keys.Select(u => u.time).ToArray();
            var values = valueGradient.Curve.keys.Select(u => u.value).ToArray();

            computeShader.SetFloats(prefix + "Keys", PackFloats(keys));
            computeShader.SetFloats(prefix + "Values", PackFloats(values));
            computeShader.SetInt(prefix + "Size", keys.Length);
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