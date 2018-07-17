using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        class TownGenerator : FieldGenerator
        {
            private readonly TerrianConfig config;

            public TownGenerator(TerrianConfig config)
            {
                this.config = config;
            }

            public float GetValue(int i, int j, int k)
            {
                var noise = config.townNoise;
                return (float)noise.GetValue(new Vector3(i, j, k));
            }
        }
    }
}
