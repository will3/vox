using UnityEngine;
using System.Collections;

namespace FarmVox
{

    public class Pine
    {
        private float r;
        private float h;
        float[] profile;
        int width;
        int height;
        public Vector3Int Offset;
        private int trunkHeight;

        public Pine(float r, float h, int trunkHeight)
        {
            this.r = r;
            this.h = h;
            this.trunkHeight = trunkHeight;

            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            Offset = new Vector3Int(-mid, 1, -mid);
        }

        public Array3<Voxel> GetShape()
        {
            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            width = radius * 2 + 1;
            height = Mathf.CeilToInt(h) + trunkHeight;
            var shape = new Array3<Voxel>(width, height, width);

            for (var j = 0; j < height; j++)
            {
                var currentR = r * (1 - (j - trunkHeight) / h);

                for (var i = 0; i < width; i++)
                {
                    for (var k = 0; k < width; k++)
                    {
                        if (j < trunkHeight + 2 && i == mid && k == mid)
                        {
                            var v = new Voxel(1, Colors.trunk);
                            shape.Set(i, j, k, v);
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
                                var v = new Voxel(density, Colors.leaf);
                                v.value -= Random.Range(0.0f, 1.0f) * 1.0f;
                                shape.Set(i, j, k, v);
                            }    
                        }
                    }
                }
            }

            return shape;
        }
    }
}