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
            var heightNoise = new Perlin3DGPU(config.heightNoise, size, origin);
            var canyonNoise = new Perlin3DGPU(config.canyonNoise, size, origin);
            heightNoise.Dispatch();
            canyonNoise.Dispatch();

            shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
            shader.SetBuffer(0, "_CanyonBuffer", canyonNoise.Results);
            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_ColorBuffer", colorBuffer);

            shader.SetVector("_RockColor", Colors.red);
            shader.SetInt("_Size", size);
            shader.SetVector("_Origin", (Vector3)origin);
            shader.SetFloat("_HillHeight", config.hillHeight);
            shader.SetFloat("_PlainHeight", config.plainHeight);

            var dispatchNum = Mathf.CeilToInt(32 / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);

            heightNoise.Dispose();
            canyonNoise.Dispose();
        }
    }
}