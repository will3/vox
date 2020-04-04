using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.GPU
{
    public static class NoisePacker
    {
        public static ComputeBuffer PackNoises(IReadOnlyCollection<Noise> noises)
        {
            var computeBuffer = new ComputeBuffer(noises.Count, GpuNoise.Stride);
            computeBuffer.SetData(noises.Select(MapNoise).ToArray());
            return computeBuffer;
        }

        private static GpuNoise MapNoise(Noise noise)
        {
            return new GpuNoise
            {
                frequency = noise.frequency,
                amplitude = noise.amplitude,
                seed = noise.seed,
                lacunarity = noise.lacunarity,
                persistence = noise.persistence,
                octaves = noise.octaves,
                yScale = noise.yScale,
                xzScale = noise.xzScale,
                type = (int) noise.type,
                offset = noise.offset
            };
        }
    }
}