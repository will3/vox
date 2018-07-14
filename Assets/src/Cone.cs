using UnityEngine;
using System.Collections;

public class Cone
{
    private float r;
    private float h;
    float[] profile;
    int width;
    int height;
    private Array3<float> shape;

    public Array3<float> Shape
    {
        get
        {
            return shape;
        }
    }

    public Cone(float r, float h) {
        this.r = r;
        this.h = h;
        calcShape();
    }

    private void calcShape() {
        int radius = Mathf.CeilToInt(r);
        int mid = radius + 1;
        width = radius * 2 + 1;
        height = Mathf.CeilToInt(h);

        shape = new Array3<float>(width, height, width);
        var minR = 1.0f;

        for (var j = 0; j < height; j++) {
            var currentR = r * (1 - j / h);

            for (var i = 0; i < width; i ++) {
                for (var k = 0; k < width; k ++) {
                
                    float diffI = Mathf.Abs(mid - i);
                    float diffK = Mathf.Abs(mid - k);
                    float distance = Mathf.Sqrt(diffI * diffI + diffK * diffK);
                    float density = currentR - distance;

                    // No more pointy tops
                    if (currentR < minR) {
                        density *= (currentR / minR);
                    }

                    shape.Set(i, j, k, density);
                }
            }
        }
    }
}