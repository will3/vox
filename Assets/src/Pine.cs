using UnityEngine;
using System.Collections;

namespace FarmVox
{
    public class Pine
    {
        readonly float r;
        readonly float h;
        readonly Vector3Int offset;
        readonly int trunkHeight;

        public Pine(float r, float h, int trunkHeight)
        {
            this.r = r;
            this.h = h;
            this.trunkHeight = trunkHeight;

            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            offset = new Vector3Int(-radius, 1, -radius);
        }

        public void Place(Terrian terrian, Chunks layer, Vector3Int position, TerrianConfig config)
        {
            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            var width = radius * 2 + 1;
            var height = Mathf.CeilToInt(h) + trunkHeight;

            terrian.SetTree(position);

            for (var j = 0; j < height; j++)
            {
                var currentR = r * (1 - (j - trunkHeight) / h);

                for (var i = 0; i < width; i++)
                {
                    for (var k = 0; k < width; k++)
                    {
                        var coord = new Vector3Int(i, j, k) + position + offset;
                        if (j < trunkHeight + 2 && i == mid && k == mid)
                        {
                            layer.Set(coord, 1);
                            layer.SetColor(coord, Colors.trunk);
                        } else if (j >= trunkHeight) {
                            float diffI = Mathf.Abs(mid - i);
                            float diffK = Mathf.Abs(mid - k);
                            float distance = Mathf.Sqrt(diffI * diffI + diffK * diffK);
                            float density = currentR - distance;

                            if (j == height - 1)
                            {
                                density = 0f;
                            }

                            if (density > 0)
                            {
                                var value = density - (float)config.treeRandom.NextDouble() * 1.0f;
                                layer.Set(coord, value);
                                layer.SetColor(coord, Colors.leaf);
                            }
                        }
                    }
                }
            }
        }
    }
}