using UnityEngine;
using System.Collections;

namespace FarmVox
{
    public class Perlin3DGPU
    {
        private int workGroups = 8;
        private int size;
        private Noise noise;

        private ComputeShader computeShader;
        private ComputeBuffer results;
        private Vector3 origin;

        public Perlin3DGPU(Noise noise, int size, Vector3 origin)
        {
            this.noise = noise;
            this.size = size;
            this.origin = origin;
            this.computeShader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
            this.results = new ComputeBuffer(size * size * size, sizeof(float));
        }

        public void Dispatch()
        {
            var frequency = noise.frequency;
            var amplitude = noise.amplitude;
            var seed = noise.seed;
            var lacunarity = noise.lacunarity;
            var persistence = noise.persistence;
            var octaves = noise.octaves;
            var yScale = noise.yScale;

            computeShader.SetBuffer(0, "_Results", results);
            computeShader.SetFloat("_Persistence", persistence);
            computeShader.SetVector("_Origin", origin);
            computeShader.SetInt("_Size", size);
            computeShader.SetInt("_Seed", seed);
            computeShader.SetFloat("_Frequency", frequency);
            computeShader.SetFloat("_Lacunarity", lacunarity);
            computeShader.SetInt("_Octaves", octaves);
            computeShader.SetFloat("_YScale", yScale);
            var dispatchNum = Mathf.CeilToInt(size / (float)workGroups);
            computeShader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
        }

        public void Dispose()
        {
            results.Release();
        }

        public float[] Read()
        {
            var data = new float[size * size * size];
            results.GetData(data);

            var min = Mathf.Infinity;
            var max = -Mathf.Infinity;

            for (var i = 0; i < data.Length; i++)
            {
                var v = data[i];
                if (v > max)
                {
                    max = v;
                }
                if (v < min)
                {
                    min = v;
                }
            }
            Debug.Log(min);
            Debug.Log(max);
            return data;
        }
    }
}