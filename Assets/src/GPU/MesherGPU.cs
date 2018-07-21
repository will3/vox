﻿using UnityEngine;

namespace FarmVox
{
    public struct Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Color colorA;
        public Color colorB;
        public Color colorC;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 3 + sizeof(float) * 4 * 3;
            }
        }
    }

    public class MesherGPU
    {
        private readonly ComputeShader shader;
        private readonly int size;
        private readonly int dataSize;
        private readonly int workGroups = 8;
        public int normalBranding = 6;
        public float normalStrength = 0.4f;
        private float shadowStrength = 0.5f;
        public bool useNormals = true;
        public bool isWater = false;

        public MesherGPU(int size)
        {
            this.size = size;
            this.dataSize = size + 3;
            shader = Resources.Load<ComputeShader>("Shaders/Mesher");
        }

        public ComputeBuffer CreateTrianglesBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, Triangle.Size, ComputeBufferType.Append);
        }

        public ComputeBuffer CreateVoxelBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float) * 3);
        }

        public ComputeBuffer CreateColorBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float) * 4);
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorsBuffer, ComputeBuffer trianglesBuffer, TerrianChunk terrianChunk, Chunk chunk)
        {
            shader.SetInt("_DataSize", dataSize);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_TrianglesBuffer", trianglesBuffer);
            shader.SetBuffer(0, "_ColorsBuffer", colorsBuffer);
            shader.SetInt("_NormalBranding", normalBranding);
            shader.SetInt("_UseNormals", useNormals ? 1 : 0);
            shader.SetInt("_IsWater", isWater ? 1 : 0);
            shader.SetFloat("_NormalStrength", normalStrength);

            var shadowBuffer = new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float));
            var waterfallBuffer = new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float));

            var shadowBufferData = new float[dataSize * dataSize * dataSize];
            foreach(var coord in chunk.surfaceCoords) {
                var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;
                var v = chunk.GetShadow(coord);
                shadowBufferData[index] = v * shadowStrength;
            }
            shadowBuffer.SetData(shadowBufferData);
            shader.SetBuffer(0, "_ShadowBuffer", shadowBuffer);

            if (chunk.Waterfalls.Count > 0) {
                shader.SetInt("_HasWaterfalls", 1);
                var waterfallData = new float[dataSize * dataSize * dataSize];
                foreach(var kv in chunk.Waterfalls) {
                    var coord = kv.Key;
                    var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;
                    var value = kv.Value;
                    waterfallData[index] = value;
                }
                waterfallBuffer.SetData(waterfallData);
                shader.SetBuffer(0, "_WaterfallBuffer", waterfallBuffer);
            } else {
                shader.SetInt("_HasWaterfalls", 0);
            }

            var disptachNumber = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, 3 * disptachNumber, disptachNumber, disptachNumber);

            shadowBuffer.Dispose();
            waterfallBuffer.Dispose();
        }

        public int Count(ComputeBuffer from) {
            var countBuffer = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);

            ComputeBuffer.CopyCount(from, countBuffer, 0);
            int[] counter = new int[] { 0, 0, 0, 0 };
            countBuffer.GetData(counter);
            int count = counter[0];

            countBuffer.Dispose();

            return count;
        }

        public Triangle[] ReadTriangles(ComputeBuffer trianglesBuffer) {
            var count = Count(trianglesBuffer);
            var triangles = new Triangle[count];

            trianglesBuffer.GetData(triangles);

            return triangles;
        }
    }
}