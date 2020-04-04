﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    public class MesherGpu : IDisposable
    {
        private readonly ComputeShader _shader;
        private readonly int _size;
        private readonly BoundsInt _bounds;
        private readonly Vector3Int _origin;
        private readonly int[] _workGroups = {8, 8, 4};

        public bool IsWater = false;

        private readonly ComputeBuffer _voxelBuffer;
        private readonly ComputeBuffer _colorsBuffer;
        private readonly ComputeBuffer _normalsBuffer;
        private readonly ComputeBuffer _quadsBuffer;
        private readonly bool _useBounds;
        public float AoStrength = 0.0f;
        private readonly WaterConfig _waterConfig;

        public MesherGpu(
            int size,
            BoundsInt bounds,
            Vector3Int origin,
            bool useBounds,
            WaterConfig waterConfig)
        {
            _size = size;
            _bounds = bounds;
            _origin = origin;
            _shader = Resources.Load<ComputeShader>("Shaders/Mesher");
            _useBounds = useBounds;
            _waterConfig = waterConfig;

            _quadsBuffer =
                new ComputeBuffer(_size * _size * _size, Quad.Size, ComputeBufferType.Append);
            _voxelBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float));
            _colorsBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float) * 4);
            _normalsBuffer = new ComputeBuffer(_size * _size * _size, sizeof(float) * 3);
        }

        public void Dispatch()
        {
            _shader.SetInt("_Size", _size);
            _shader.SetBuffer(0, "_VoxelBuffer", _voxelBuffer);
            _shader.SetBuffer(0, "_NormalsBuffer", _normalsBuffer);
            _shader.SetBuffer(0, "_ColorsBuffer", _colorsBuffer);

            _shader.SetBuffer(0, "_QuadsBuffer", _quadsBuffer);

            _shader.SetInt("_IsWater", IsWater ? 1 : 0);
            _shader.SetFloat("_AoStrength", AoStrength);
            _shader.SetInts("_Bounds", _bounds.min.x, _bounds.max.x, _bounds.min.z, _bounds.max.z);
            _shader.SetInts("_Origin", _origin.x, _origin.y, _origin.z);
            _shader.SetInt("_UseBounds", _useBounds ? 1 : 0);
            _shader.SetInt("_WaterLevel", _waterConfig.waterLevel);
            _shader.SetFloat("_WaterOpacity", _waterConfig.opacity);

            var slices = _size + 1;
            _shader.Dispatch(0,
                Mathf.CeilToInt(slices / (float) _workGroups[0]),
                Mathf.CeilToInt(slices / (float) _workGroups[1]),
                Mathf.CeilToInt(slices / (float) _workGroups[2]));
        }

        public IEnumerable<Quad> ReadQuads()
        {
            var count = AppendBufferCounter.Count(_quadsBuffer);
            var quads = new Quad[count];

            _quadsBuffer.GetData(quads);

            return quads;
        }

        public void SetData(float[] data)
        {
            _voxelBuffer.SetData(data);
        }

        public void SetColors(Color[] colors)
        {
            _colorsBuffer.SetData(colors);
        }

        public void SetNormals(Vector3[] normals)
        {
            _normalsBuffer.SetData(normals);
        }

        public void Dispose()
        {
            _voxelBuffer?.Dispose();
            _colorsBuffer?.Dispose();
            _normalsBuffer?.Dispose();
            _quadsBuffer?.Dispose();
        }
    }
}