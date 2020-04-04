using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

namespace FarmVox.Scripts
{
    public static class ModuleBuilder
    {
        public static ModuleBase Build(Noise noise)
        {
            return new ScaleBias(noise.amplitude, 0,
                new Scale(noise.xzScale, noise.yScale, noise.xzScale,
                    new ScaleBias(0.5f, 0.5f, new Perlin(noise.frequency, noise.lacunarity, noise.persistence,
                        noise.octaves, noise.seed,
                        QualityMode.Medium))));
        }
    }
}