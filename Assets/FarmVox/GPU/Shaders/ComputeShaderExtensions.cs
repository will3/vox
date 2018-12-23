using System.Linq;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public static class ComputeShaderExtensions {
        public static ColorGradientBuffers SetColorGradient(this ComputeShader computeShader, Gradient colorGradient, int banding, string prefix)
        {
            var intervalsBuffer = new ComputeBuffer(colorGradient.colorKeys.Length, sizeof(float));
            intervalsBuffer.SetData(colorGradient.colorKeys.Select(u => u.time).ToArray());

            var gradientBuffer = new ComputeBuffer(colorGradient.colorKeys.Length, sizeof(float) * 4);
            gradientBuffer.SetData(colorGradient.colorKeys.Select(u => u.color).ToArray());

            computeShader.SetBuffer(0, prefix + "Gradient", gradientBuffer);
            computeShader.SetBuffer(0, prefix + "GradientIntervals", intervalsBuffer);
            computeShader.SetInt(prefix + "GradientSize", colorGradient.colorKeys.Length);
            computeShader.SetFloat(prefix + "GradientBanding", banding);

            return new ColorGradientBuffers(intervalsBuffer, gradientBuffer);
        }

        public static void SetColorGradient2(this ComputeShader computeShader, ColorGradient2 colorGradient2, string prefix)
        {
            computeShader.SetVectorArray(prefix + "Gradient", colorGradient2.Colors);
            computeShader.SetFloats(prefix + "GradientIntervals", colorGradient2.Keys);
            computeShader.SetInt(prefix + "GradientSize", colorGradient2.Keys.Length);
            computeShader.SetFloat(prefix + "GradientBanding", colorGradient2.Banding);
        }
    }
}