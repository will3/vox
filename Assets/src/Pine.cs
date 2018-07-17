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
        private int trunkHeight;

        public Pine(float r, float h, int trunkHeight)
        {
            this.r = r;
            this.h = h;
            this.trunkHeight = trunkHeight;
            calcShape();
        }

        public Array3<Voxel> GetShape() {
            var copy = new Array3<Voxel>(shape.Width, shape.Height, shape.Depth);
            for (var i = 0; i < shape.Width; i++) {
                for (var j = 0; j < shape.Height; j++) {
                    for (var k = 0; k < shape.Depth; k++) {
                        var v = shape.Get(i, j, k);
                        if (v != null) {
                            v.value -= Random.Range(0.0f, 1.0f) * 0.5f;
                            copy.Set(i, j, k, v);        
                        }
                    }
                }
            }
            return copy;
        }

        private void calcShape()
        {
            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            width = radius * 2 + 1;
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