using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        class HeightGenerator : FieldGenerator
        {
            private TerrianConfig config;
            public HeightGenerator(TerrianConfig config)
            {
                this.config = config;
            }
            public float GetValue(int i, int j, int k)
            {
                var plainHeight = config.plainHeight;
                var hillHeight = config.hillHeight;
                var heightNoise = config.heightNoise;
                var heightNoise2 = config.heightNoise2;

                var canyonNoise = config.canyonNoise;
                var biome = (float)canyonNoise.GetValue(new Vector3(i, j * 0.4f, k));

                float terrainHeight;
                if (biome < 0.1 && biome > -0.1)
                {
                    var ratio = (float)(biome + 0.1f) / 0.2f;
                    terrainHeight = plainHeight + (hillHeight - plainHeight) * ratio;
                }
                else if (biome > 0)
                {
                    terrainHeight = hillHeight;
                }
                else
                {
                    terrainHeight = plainHeight;
                }

                var height = (1f - j / (float)terrainHeight) - 0.5f;
                var value = height;
                var n1 = (float)heightNoise.GetValue(new Vector3(i, j * 0.4f, k) * 0.015f);
                var n2 = (float)heightNoise2.GetValue(new Vector3(i, j * 0.4f, k) * 0.015f) * 0.5f;
                return value + n1;
            }
        }
    }
}
