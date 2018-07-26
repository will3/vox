using UnityEngine;

namespace FarmVox
{
    public class GenTerrianGPU
    {
        private int size;
        private int dataSize;
        private int workGroups = 8;
        private ComputeShader shader;
        private TerrianConfig config;
        private Vector3Int origin;
        private ComputeBuffer voxelBuffer;

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

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorBuffer) {
            var heightNoise = new Perlin3DGPU(config.heightNoise, dataSize, origin);
            var canyonNoise = new Perlin3DGPU(config.canyonNoise, dataSize, origin);
            var rockColorNoise = new Perlin3DGPU(config.rockColorNoise, dataSize, origin);

            heightNoise.Dispatch();
            canyonNoise.Dispatch();
            rockColorNoise.Dispatch();

            shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
            shader.SetBuffer(0, "_CanyonBuffer", canyonNoise.Results);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
            shader.SetFloat("_MaxHeight", config.maxHeight);

            shader.SetVector("_SoilColor", Colors.soil);
            shader.SetVector("_WaterColor", Colors.water);

            shader.SetInt("_Size", size);
            shader.SetVector("_Origin", (Vector3)origin);
            shader.SetFloat("_HillHeight", config.hillHeight);
            shader.SetFloat("_PlainHeight", config.plainHeight);

            var rockGradientIntervalsBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float));
            rockGradientIntervalsBuffer.SetData(Colors.rockColorGradient.GetKeys());

            var rockGradientBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float) * 4);
            rockGradientBuffer.SetData(Colors.rockColorGradient.GetValues());

            shader.SetBuffer(0, "_RockGradient", rockGradientBuffer);
            shader.SetBuffer(0, "_RockGradientIntervals", rockGradientIntervalsBuffer);
            shader.SetInt("_RockGradientSize", Colors.rockColorGradient.Count);
            shader.SetFloat("_RockGradientBanding", Colors.rockColorGradient.banding);
            shader.SetBuffer(0, "_RockColorNoise", rockColorNoise.Results);

            shader.SetInt("_DataSize", heightNoise.DataSize);
            shader.SetInt("_Resolution", heightNoise.Resolution);

            var dispatchNum = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);

            heightNoise.Dispose();
            canyonNoise.Dispose();
            rockGradientBuffer.Dispose();
            rockGradientIntervalsBuffer.Dispose();
            rockColorNoise.Dispose();
        }
    }
}