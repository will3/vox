using System;
using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    public class Perlin3DGpu : IDisposable
    {
        private readonly int[] _workGroups = {8, 8, 4};
        private readonly Noise _noise;
        private readonly ComputeShader _shader;
        private readonly Vector3 _origin;
        private readonly int _dataSize;
        
        public ComputeBuffer Results { get; private set; }

        public Perlin3DGpu(Noise noise, int size, Vector3 origin)
        {
            _noise = noise;
            _origin = origin;
            _shader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
            _dataSize = size;
            Results = new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float));
            Dispatch();
        }

        void Dispatch()
        {
            var frequency = _noise.frequency;
            var amplitude = _noise.amplitude;
            var seed = _noise.seed;
            var lacunarity = _noise.lacunarity;
            var persistence = _noise.persistence;
            var octaves = _noise.octaves;
            var yScale = _noise.yScale;
            var xzScale = _noise.xzScale;

            _shader.SetBuffer(0, "_Results", Results);
            _shader.SetFloat("_Persistence", persistence);
            _shader.SetVector("_Origin", _origin);
            _shader.SetInt("_Seed", seed);
            _shader.SetFloat("_Frequency", frequency);
            _shader.SetFloat("_Lacunarity", lacunarity);
            _shader.SetInt("_Octaves", octaves);
            _shader.SetFloat("_YScale", yScale);
            _shader.SetFloat("_XZScale", xzScale);
            _shader.SetFloat("_Amplitude", amplitude);
            _shader.SetInt("_DataSize", _dataSize);
            _shader.SetInt("_Type", (int)_noise.type);

            _shader.Dispatch(0, 
                Mathf.CeilToInt(_dataSize / (float) _workGroups[0]),
                Mathf.CeilToInt(_dataSize / (float) _workGroups[1]),
                Mathf.CeilToInt(_dataSize / (float) _workGroups[2]));
        }

        public void Dispose()
        {
            Results.Release();
        }

        public float[] Read()
        {
            var data = new float[_dataSize * _dataSize * _dataSize];
            Results.GetData(data);
            return data;
        }
    }
}