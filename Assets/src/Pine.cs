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
        private Array3<Voxel> shape;
        public Vector3Int Offset;

        public Array3<Voxel> Shape
        {
            get
            {
                return shape;
            }
        }

        public Pine(float r, float h)
        {
            this.r = r;
            this.h = h;
            calcShape();
        }

        private void calcShape()
        {
            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            width = radius * 2 + 1;
            int trunkHeight = 2;
            height = Mathf.CeilToInt(h) + trunkHeight;
            shape = new Array3<Voxel>(width, height, width);

            for (var j = 0; j < height; j++)
            {
                var currentR = r * (1 - (j - trunkHeight) / h);

                for (var i = 0; i < width; i++)
                {
                    for (var k = 0; k < width; k++)
                    {
                        if (j < trunkHeight)
                        {
                            if (i == mid && k == mid)
                            {
                                var v = new Voxel(1, Colors.trunk);
                                shape.Set(i, j, k, v);
                            }
                        }
                        else
                        {
                            float diffI = Mathf.Abs(mid - i);
                            float diffK = Mathf.Abs(mid - k);
                            float distance = Mathf.Sqrt(diffI * diffI + diffK * diffK);
                            float density = currentR - distance;

                            if (j == height - 1) {
                                density = 0f;
                            }

                            if (density > 0)
                            {
                                var v = new Voxel(density, Colors.leaf);
                                shape.Set(i, j, k, v);
                            }
                        }
                    }
                }
            }

            Offset = new Vector3Int(-mid, 1, -mid);
        }
    }
}