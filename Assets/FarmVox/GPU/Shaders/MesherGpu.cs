﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class MesherGpu : IDisposable
    {
        private readonly ComputeShader _shader;
        private readonly int _size;
        private readonly MesherSettings _settings;
        private readonly int[] _workGroups = {8, 8, 4};
        
        public int NormalBanding = 6;
        public bool UseNormals = true;
        public bool IsWater = false;

        private readonly ComputeBuffer _voxelBuffer;
        private readonly ComputeBuffer _colorsBuffer;
        private readonly ComputeBuffer _trianglesBuffer;

        public float NormalStrength = 0.0f;
        
        public MesherGpu(int size, MesherSettings settings)
        {
            _size = size;
            _settings = settings;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");

            _trianglesBuffer =
                new ComputeBuffer(_size * _size * _size, Quad.Size, ComputeBufferType.Append);
            _voxelBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float));
            _colorsBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float) * 4);
        }

        public void Dispatch()
        {
            _shader.SetInt("_Size", _size);
            _shader.SetBuffer(0, "_VoxelBuffer", _voxelBuffer);
            _shader.SetBuffer(0, "_TrianglesBuffer", _trianglesBuffer);

            _shader.SetBuffer(0, "_ColorsBuffer", _colorsBuffer);

            _shader.SetFloat("_NormalBanding", NormalBanding);
            _shader.SetInt("_UseNormals", UseNormals ? 1 : 0);
            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_NormalStrength", NormalStrength);
            _shader.SetFloat("_AoStrength", _settings.AoStrength);

            _shader.Dispatch(0, 
                3 * Mathf.CeilToInt(_size / (float) _workGroups[0]),
                Mathf.CeilToInt(_size / (float) _workGroups[1]),
                Mathf.CeilToInt(_size / (float) _workGroups[2]));
        }

        public IEnumerable<Quad> ReadTriangles() {
            var count = AppendBufferCounter.Count(_trianglesBuffer);
            var triangles = new Quad[count];

            _trianglesBuffer.GetData(triangles);

            return triangles;
        }

        public void SetData(float[] data)
        {
            _voxelBuffer.SetData(data);
        }

        public void SetColors(Color[] colors)
        {
            _colorsBuffer.SetData(colors);
        }

        public void Dispose()
        {
            _voxelBuffer?.Dispose();
            _colorsBuffer?.Dispose();
            _trianglesBuffer?.Dispose();
        }
    }
}