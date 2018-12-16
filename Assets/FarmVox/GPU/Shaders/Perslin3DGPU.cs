using System;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class Perlin3DGpu : IDisposable
    {
        private const int WorkGroups = 8;
        private readonly int _size;
        private readonly Noise _noise;
        private readonly ComputeShader _shader;
        private readonly Vector3 _origin;
        private readonly int _dataSize;
        
        public ComputeBuffer Results { get; private set; }

        public Perlin3DGpu(Noise noise, int size, Vector3 origin)
        {
            _noise = noise;
            _size = size;
            _origin = origin;
            _shader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
            _dataSize = size;
            Results = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float));
            Dispatch();
        }

        void Dispatch()
        {
            var frequency = _noise.Frequency;
            var amplitude = _noise.Amplitude;
            var seed = _noise.Seed;
            var lacunarity = _noise.Lacunarity;
            var persistence = _noise.Persistence;
            var octaves = _noise.Octaves;
            var yScale = _noise.YScale;
            var xzScale = _noise.XzScale;

            _shader.SetBuffer(0, "_Results", Results);
            _shader.SetFloat("_Persistence", persistence);
            _shader.SetVector("_Origin", _origin);
            _shader.SetInt("_Size", _size);
            _shader.SetInt("_Seed", seed);
            _shader.SetFloat("_Frequency", frequency);
            _shader.SetFloat("_Lacunarity", lacunarity);
            _shader.SetInt("_Octaves", octaves);
            _shader.SetFloat("_YScale", yScale);
            _shader.SetFloat("_XZScale", xzScale);
            _shader.SetFloat("_Amplitude", amplitude);
            _shader.SetInt("_DataSize", _dataSize);
            _shader.SetInt("_Type", (int)_noise.Type);

            var dispatchNum = Mathf.CeilToInt(_size / (float)WorkGroups);
            _shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
        }

        public void Dispose()
        {
            Results.Release();
        }

        public float[] Read()
        {
            var data = new float[_size * _size * _size];
            Results.GetData(data);
            return data;
        }
    }
}