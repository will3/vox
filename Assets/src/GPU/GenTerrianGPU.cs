using System.Collections.Generic;
using UnityEngine;
using IDisposable = System.IDisposable;

namespace FarmVox
{
    struct GenTerrianNoise {
        public float height;
        public float rockColor;
        public float grass;
        public float river;
        public float stone;
        public float stone2;

        public static int Stride {
            get {
                return sizeof(float) * 6;
            }
        }
    }

    public class GenTerrianNoiseGPU : IDisposable
    {
        int dataSize;
        Vector3Int origin;
        TerrianConfig config;
        ComputeShader shader;
        readonly int workGroups = 8;

        ComputeBuffer results;

        public ComputeBuffer Results
        {
            get
            {
                return results;
            }
        }

        public GenTerrianNoiseGPU(int dataSize, Vector3Int origin, TerrianConfig config) {
            this.dataSize = dataSize;
            this.origin = origin;
            this.config = config;
            shader = Resources.Load<ComputeShader>("Shaders/GenTerrianNoise");
            results = new ComputeBuffer(dataSize * dataSize * dataSize, GenTerrianNoise.Stride);

            Dispatch();
        }

        void Dispatch() {
            using (var heightNoise = new Perlin3DGPU(config.heightNoise, dataSize, origin))
            using (var rockColorNoise = new Perlin3DGPU(config.rockColorNoise, dataSize, origin))
            using (var grassNoise = new Perlin3DGPU(config.grassNoise, dataSize, origin))
            using (var riverNoise = new Perlin3DGPU(config.riverNoise, dataSize, origin))
            using (var stoneNoise = new Perlin3DGPU(config.stoneNoise, dataSize, origin))
            using (var stoneNoise2 = new Perlin3DGPU(config.stoneNoise2, dataSize, origin))
            {
                shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
                shader.SetBuffer(0, "_GrassBuffer", grassNoise.Results);
                shader.SetBuffer(0, "_RiverBuffer", riverNoise.Results);
                shader.SetBuffer(0, "_RockColorBuffer", rockColorNoise.Results);
                shader.SetBuffer(0, "_StoneBuffer", stoneNoise.Results);
                shader.SetBuffer(0, "_StoneBuffer2", stoneNoise2.Results);

                shader.SetBuffer(0, "_NoiseBuffer", results);

                shader.SetInt("_DataSize", dataSize);

                var dispatchNum = Mathf.CeilToInt(dataSize / (float)workGroups);
                shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
            }
        }

        public void Dispose()
        {
            results.Dispose();
        }
    }

    public class GenTerrianGPU
    {
        class ColorGradientBuffers : System.IDisposable
        {
            public readonly ComputeBuffer intervalsBuffer;
            public readonly ComputeBuffer colorsBuffer;

            public ColorGradientBuffers(ComputeBuffer intervalsBuffer, ComputeBuffer colorsBuffer)
            {
                this.intervalsBuffer = intervalsBuffer;
                this.colorsBuffer = colorsBuffer;
            }

            public void Dispose()
            {
                intervalsBuffer.Dispose();
                colorsBuffer.Dispose();
            }
        }

        int size;
        int dataSize;
        int workGroups = 8;
        ComputeShader shader;
        TerrianConfig config;
        Vector3Int origin;
        ComputeBuffer voxelBuffer;

        public GenTerrianGPU(int size, Vector3Int origin, TerrianConfig config) {
            this.size = size;
            this.origin = origin;
            this.config = config;

            dataSize = size + 3;
            shader = Resources.Load<ComputeShader>("Shaders/GenTerrian");
        }

        public ComputeBuffer CreateVoxelBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float));
        }

        public ComputeBuffer CreateColorBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(float) * 4);
        }

        public ComputeBuffer CreateTypeBuffer() {
            return new ComputeBuffer(dataSize * dataSize * dataSize, sizeof(int));
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorBuffer, ComputeBuffer typeBuffer, TerrianChunk terrianChunk) {
            using (var noises = new GenTerrianNoiseGPU(dataSize, origin, config))
            using(var rockGradientBuffers = SetColorGradient(config.rockColorGradient, "_Rock"))
            using(var grassGradientBuffers = SetColorGradient(config.grassColor, "_Grass"))
            using(var grassNormalBuffers = config.grassNormalFilter.CreateBuffers(shader, "_GrassNormal"))
            using(var grassHeightBuffers = config.grassHeightFilter.CreateBuffers(shader, "_GrassHeight"))
            using(config.riverNoiseFilter.CreateBuffers(shader, "_River"))
            using(config.stoneHeightFilter.CreateBuffers(shader, "_StoneHeight"))
            {
                shader.SetBuffer(0, "_NoiseBuffer", noises.Results);

                shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
                shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
                shader.SetBuffer(0, "_TypeBuffer", typeBuffer);

                shader.SetFloat("_MaxHeight", config.maxHeight);

                shader.SetVector("_SoilColor", Colors.soil);
                shader.SetVector("_WaterColor", config.waterColor);

                shader.SetInt("_Size", size);
                shader.SetVector("_Origin", (Vector3)origin);
                shader.SetFloat("_HillHeight", config.hillHeight);
                shader.SetFloat("_PlainHeight", config.plainHeight);
                shader.SetFloat("_GroundHeight", config.groundHeight);
                shader.SetInt("_WaterLevel", config.waterLevel);

                shader.SetVector("_StoneColor", config.stoneColor);
                shader.SetFloat("_StoneThreshold", config.stoneThreshold);

                shader.SetInt("_DataSize", dataSize);

                var dispatchNum = Mathf.CeilToInt(dataSize / (float)workGroups);
                shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
            }
        }

        ColorGradientBuffers SetColorGradient(ColorGradient colorGradient, string prefix) {
            var intervalsBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float));
            intervalsBuffer.SetData(colorGradient.GetKeys());

            var gradientBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float) * 4);
            gradientBuffer.SetData(colorGradient.GetValues());

            shader.SetBuffer(0, prefix + "Gradient", gradientBuffer);
            shader.SetBuffer(0, prefix + "GradientIntervals", intervalsBuffer);
            shader.SetInt(prefix + "GradientSize", colorGradient.Count);
            shader.SetFloat(prefix + "GradientBanding", colorGradient.banding);

            return new ColorGradientBuffers(intervalsBuffer, gradientBuffer);
        }
    }
}