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
            var grassNoise = new Perlin3DGPU(config.grassNoise, dataSize, origin);

            heightNoise.Dispatch();
            canyonNoise.Dispatch();
            rockColorNoise.Dispatch();
            grassNoise.Dispatch();

            shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
            shader.SetBuffer(0, "_CanyonBuffer", canyonNoise.Results);
            shader.SetBuffer(0, "_GrassBuffer", grassNoise.Results);

            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
            shader.SetFloat("_MaxHeight", config.maxHeight);

            shader.SetVector("_SoilColor", Colors.soil);
            shader.SetVector("_WaterColor", Colors.water);

            shader.SetInt("_Size", size);
            shader.SetVector("_Origin", (Vector3)origin);
            shader.SetFloat("_HillHeight", config.hillHeight);
            shader.SetFloat("_PlainHeight", config.plainHeight);

            //var rockGradientIntervalsBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float));
            //rockGradientIntervalsBuffer.SetData(Colors.rockColorGradient.GetKeys());
            //var rockGradientBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float) * 4);
            //rockGradientBuffer.SetData(Colors.rockColorGradient.GetValues());
            //shader.SetBuffer(0, "_RockGradient", rockGradientBuffer);
            //shader.SetBuffer(0, "_RockGradientIntervals", rockGradientIntervalsBuffer);
            //shader.SetInt("_RockGradientSize", Colors.rockColorGradient.Count);
            //shader.SetFloat("_RockGradientBanding", Colors.rockColorGradient.banding);

            ComputeBuffer rockGradientBuffer;
            ComputeBuffer rockGradientIntervalsBuffer;
            LoadColorGradient(Colors.rockColorGradient, "_Rock", 
                              out rockGradientIntervalsBuffer, out rockGradientBuffer);

            shader.SetBuffer(0, "_RockColorNoise", rockColorNoise.Results);


            ComputeBuffer grassGradientBuffer;
            ComputeBuffer grassGradientIntervalsBuffer;

            LoadColorGradient(Colors.grassGradient, "_Grass",
                              out grassGradientIntervalsBuffer, out grassGradientBuffer);


            var grassNormalKeysBuffer = new ComputeBuffer(config.grassNormalFilter.Keys.Count, sizeof(float));
            grassNormalKeysBuffer.SetData(config.grassNormalFilter.Keys);
            var grassNormalValuesBuffer = new ComputeBuffer(config.grassNormalFilter.Keys.Count, sizeof(float));
            grassNormalValuesBuffer.SetData(config.grassNormalFilter.Values);
            shader.SetBuffer(0, "_GrassNormalKeys", grassNormalKeysBuffer);
            shader.SetBuffer(0, "_GrassNormalValues", grassNormalValuesBuffer);
            shader.SetInt("_GrassNormalSize", config.grassNormalFilter.Keys.Count);

            var grassHeightKeysBuffer = new ComputeBuffer(config.grassHeightFilter.Keys.Count, sizeof(float));
            grassHeightKeysBuffer.SetData(config.grassHeightFilter.Keys);
            var grassHeightValuesBuffer = new ComputeBuffer(config.grassHeightFilter.Keys.Count, sizeof(float));
            grassHeightValuesBuffer.SetData(config.grassHeightFilter.Values);
            shader.SetBuffer(0, "_GrassHeightKeys", grassHeightKeysBuffer);
            shader.SetBuffer(0, "_GrassHeightValues", grassHeightValuesBuffer);
            shader.SetInt("_GrassHeightSize", config.grassHeightFilter.Keys.Count);

            shader.SetInt("_DataSize", heightNoise.DataSize);
            shader.SetInt("_Resolution", heightNoise.Resolution);

            var dispatchNum = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);

            heightNoise.Dispose();
            canyonNoise.Dispose();
            rockGradientBuffer.Dispose();
            rockGradientIntervalsBuffer.Dispose();
            rockColorNoise.Dispose();
            grassNoise.Dispose();
            grassNormalKeysBuffer.Dispose();
            grassNormalValuesBuffer.Dispose();
            grassHeightKeysBuffer.Dispose();
            grassHeightValuesBuffer.Dispose();
            grassGradientBuffer.Dispose();
            grassGradientIntervalsBuffer.Dispose();
        }

        void LoadColorGradient(ColorGradient colorGradient, string prefix, 
                               out ComputeBuffer intervalsBuffer, 
                               out ComputeBuffer gradientBuffer) {
            intervalsBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float));
            intervalsBuffer.SetData(colorGradient.GetKeys());

            gradientBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float) * 4);
            gradientBuffer.SetData(colorGradient.GetValues());

            shader.SetBuffer(0, prefix + "Gradient", gradientBuffer);
            shader.SetBuffer(0, prefix + "GradientIntervals", intervalsBuffer);
            shader.SetInt(prefix + "GradientSize", colorGradient.Count);
            shader.SetFloat(prefix + "GradientBanding", colorGradient.banding);
        }
    }
}