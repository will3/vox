using System;
using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    public class GenTerrianResults : IDisposable
    {
        public ComputeBuffer VoxelBuffer { get; }
        public ComputeBuffer ColorBuffer { get; }
        public ComputeBuffer NormalBuffer { get; }

        public GenTerrianResults(int dataSize)
        {
            var count = dataSize * dataSize * dataSize;
            VoxelBuffer = new ComputeBuffer(count, sizeof(float));
            ColorBuffer = new ComputeBuffer(count, sizeof(float) * 4);
            NormalBuffer = new ComputeBuffer(count, sizeof(float) * 3);
        }

        public float[] ReadData()
        {
            var data = new float[VoxelBuffer.count];
            VoxelBuffer.GetData(data);
            return data;
        }

        public Color[] ReadColors()
        {
            var colors = new Color[ColorBuffer.count];
            ColorBuffer.GetData(colors);
            return colors;
        }

        public Vector3[] ReadNormals()
        {
            var normals = new Vector3[NormalBuffer.count];
            NormalBuffer.GetData(normals);
            return normals;
        }

        public void Dispose()
        {
            VoxelBuffer.Dispose();
            ColorBuffer.Dispose();
            NormalBuffer.Dispose();
        }
    }
}