using UnityEngine;
using System.Collections;
using LibNoise.Generator;
using System;

namespace FarmVox
{
    public interface FieldGenerator {
        float GetValue(int i, int j, int k);    
    }

    public class Field
    {
        private float[] data;
        private int size;
        private int fullSize;
        private float resolution;

        public Field(int fullSize, float resolution = 2.0f)
        {
            this.resolution = resolution;
            this.fullSize = fullSize;
            this.size = fullSize / 2 + 2;
            data = new float[size * size * size];
        }

        public void Generate(FieldGenerator generator, Vector3Int origin) {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    for (var k = 0; k < size; k++)
                    {
                        var r = (int)resolution;
                        float v = generator.GetValue(i * r + origin.x, j * r + origin.y, k * r + origin.z);
                        Set(i, j, k, v);
                    }
                }
            }
        }

        private int getIndex(int i, int j, int k) {
            return i * size * size + j * size + k;
        }

        private void Set(int i, int j, int k, float v)
        {
            var index = getIndex(i, j, k);
            data[index] = v;
        }

        private float Get(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            return data[index];
        }

        public float Sample(float i, float j, float k) {
            return _Sample(i / resolution, j / resolution, k / resolution);
        }

        private float _Sample(float i, float j, float k)
        {
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

        public bool OutOfBounds(Vector3Int coord)
        {
            return OutOfBounds(coord.x, coord.y, coord.z);
        }

        public bool OutOfBounds(int i, int j, int k) {
            return i < 0 || i > fullSize - 1 || j < 0 || j > fullSize - 1 || k < 0 || k > fullSize - 1;
        }
    }
}