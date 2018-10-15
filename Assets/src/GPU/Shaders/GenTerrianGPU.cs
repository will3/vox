using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public partial class GenTerrianGPU
    {

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
            using(var rockGradientBuffers = SetColorGradient(config.colors.rockColorGradient, "_Rock"))
            using(var grassGradientBuffers = SetColorGradient(config.colors.grassColor, "_Grass"))
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

                shader.SetVector("_SoilColor", config.colors.soil);
                shader.SetVector("_WaterColor", config.colors.waterColor);

                shader.SetInt("_Size", size);
                shader.SetVector("_Origin", (Vector3)origin);
                shader.SetFloat("_HillHeight", config.hillHeight);
                shader.SetFloat("_PlainHeight", config.plainHeight);
                shader.SetFloat("_GroundHeight", config.groundHeight);
                shader.SetInt("_WaterLevel", config.waterLevel);

                shader.SetVector("_StoneColor", config.colors.stoneColor);
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