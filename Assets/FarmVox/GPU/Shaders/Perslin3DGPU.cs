using System;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class Perlin3DGPU : IDisposable
    {
        readonly int workGroups = 8;
        readonly int size;
        Noise noise;

        ComputeShader shader;

        public ComputeBuffer Results { get; private set; }

        ValueGradient.ValueGradientBuffers buffers;

        Vector3 origin;

        public int DataSize { get; private set; }

        public Perlin3DGPU(Noise noise, int size, Vector3 origin)
        {
            this.noise = noise;
            this.size = size;
            this.origin = origin;
            shader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
            DataSize = size;
            Results = new ComputeBuffer(DataSize * DataSize * DataSize, sizeof(float));
            Dispatch();
        }

        void Dispatch()
        {
            var frequency = noise.Frequency;
            var amplitude = noise.Amplitude;
            var seed = noise.Seed;
            var lacunarity = noise.Lacunarity;
            var persistence = noise.Persistence;
            var octaves = noise.Octaves;
            var yScale = noise.YScale;
            var xzScale = noise.XzScale;

            shader.SetBuffer(0, "_Results", Results);
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
            shader.SetInt("_DataSize", DataSize);
            shader.SetInt("_Type", (int)noise.Type);

            buffers = noise.Filter.CreateBuffers(shader, "_Filter");

            var dispatchNum = Mathf.CeilToInt(size / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
        }

        public void Dispose()
        {
            if (buffers != null) {
                buffers.Dispose();
            }
            Results.Release();
        }

        public float[] Read()
        {
            var data = new float[size * size * size];
            Results.GetData(data);
            return data;
        }
    }
}