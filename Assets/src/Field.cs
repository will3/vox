using UnityEngine;
using System.Collections;

public class Field
{
    private float[] data;
    private int size;

    public Field(int size) {
        this.size = size;
        data = new float[size * size * size];
    }

    public void Set(int i, int j, int k, float v) {
        var index = i * size * size + j * size + k;
        data[index] = v;
    }

    public float Get(int i, int j, int k) {
        var index = i * size * size + j * size + k;
        return data[index];
    }

    public float Sample(float i, float j, float k) {
        int min_i = Mathf.FloorToInt(i);
        int min_j = Mathf.FloorToInt(j);
        int min_k = Mathf.FloorToInt(k);

        int max_i = min_i + 1;
        int max_j = min_j + 1;
        int max_k = min_k + 1;

        float ri = 1 - (i - min_i);
        float rj = 1 - (j - min_j);
        float rk = 1 - (k - min_k);
        float ri2 = 1 - ri;
        float rj2 = 1 - rj;
        float rk2 = 1 - rk;

        float a = Get(min_i, min_j, min_k);
        float b = Get(max_i, min_j, min_k);
        float c = Get(min_i, max_j, min_k);
        float d = Get(max_i, max_j, min_k);
        float e = Get(min_i, min_j, max_k);
        float f = Get(max_i, min_j, max_k);
        float g = Get(min_i, max_j, max_k);
        float h = Get(max_i, max_j, max_k);

        float v =
            ((a * ri + b * ri2) * rj + (c * ri + d * ri2) * rj2) * rk +
            ((e * ri + f * ri2) * rj + (g * ri + h * ri2) * rj2) * rk2;

        return v;
    }
}
