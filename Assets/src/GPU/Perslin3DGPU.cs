using UnityEngine;
using System.Collections;
using System;

namespace FarmVox
{
    public class Perlin3DGPU : IDisposable
    {
        int workGroups = 8;
        int size;
        Noise noise;

        ComputeShader shader;
        ComputeBuffer results;

        public ComputeBuffer Results
        {
            get
            {
                return results;
            }
        }

        Vector3 origin;
        int dataSize;

        public int DataSize
        {
            get
            {
                return dataSize;
            }
        }

        public Perlin3DGPU(Noise noise, int size, Vector3 origin)
        {
            this.noise = noise;
            this.size = size;
            this.origin = origin;
            shader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
            dataSize = size;
            results = new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float));
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
            var xzScale = noise.xzScale;

            shader.SetBuffer(0, "_Results", results);
            shader.SetFloat("_Persistence", persistence);
            shader.SetVector("_Origin", origin);
            shader.SetInt("_Size", size);
            shader.SetInt("_Seed", seed);
            shader.SetFloat("_Frequency", frequency);
            shader.SetFloat("_Lacunarity", lacunarity);
            shader.SetInt("_Octaves", octaves);
            shader.SetFloat("_YScale", yScale);
            shader.SetFloat("_XZScale", xzScale);
            shader.SetFloat("_Amplitude", amplitude);
            shader.SetInt("_DataSize", dataSize);
            shader.SetInt("_Type", (int)noise.type);

            var dispatchNum = Mathf.CeilToInt(size / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
        }

        public void Dispose()
        {
            results.Release();
        }

        public float[] Read()
        {
            var data = new float[size * size * size];
            results.GetData(data);
            return data;
        }
    }
}