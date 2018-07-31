using UnityEngine;

namespace FarmVox
{
    public class GenTerrianGPU
    {
        class ValueGradientBuffers : System.IDisposable
        {
            public readonly ComputeBuffer keysBuffer;
            public readonly ComputeBuffer valuesBuffer;

            public ValueGradientBuffers(ComputeBuffer keysBuffer, ComputeBuffer valuesBuffer)
            {
                this.keysBuffer = keysBuffer;
                this.valuesBuffer = valuesBuffer;
            }

            public void Dispose()
            {
                keysBuffer.Dispose();
                valuesBuffer.Dispose();
            }
        }

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
            var heightNoise = new Perlin3DGPU(config.heightNoise, dataSize, origin);
            var rockColorNoise = new Perlin3DGPU(config.rockColorNoise, dataSize, origin);
            var grassNoise = new Perlin3DGPU(config.grassNoise, dataSize, origin);
            var canyonNoise = new Perlin3DGPU(config.canyonNoise, dataSize, origin);

            heightNoise.Dispatch();
            rockColorNoise.Dispatch();
            grassNoise.Dispatch();
            canyonNoise.Dispatch();

            shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
            shader.SetBuffer(0, "_CanyonBuffer", canyonNoise.Results);
            shader.SetBuffer(0, "_GrassBuffer", grassNoise.Results);

            shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
            shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
            shader.SetBuffer(0, "_TypeBuffer", typeBuffer);

            shader.SetFloat("_MaxHeight", config.maxHeight);

            shader.SetVector("_SoilColor", Colors.soil);
            shader.SetVector("_WaterColor", Colors.water);

            shader.SetInt("_Size", size);
            shader.SetVector("_Origin", (Vector3)origin);
            shader.SetFloat("_HillHeight", config.hillHeight);
            shader.SetFloat("_PlainHeight", config.plainHeight);
            shader.SetFloat("_GroundHeight", config.groundHeight);


            //var rockGradientIntervalsBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float));
            //rockGradientIntervalsBuffer.SetData(Colors.rockColorGradient.GetKeys());
            //var rockGradientBuffer = new ComputeBuffer(Colors.rockColorGradient.Count, sizeof(float) * 4);
            //rockGradientBuffer.SetData(Colors.rockColorGradient.GetValues());
            //shader.SetBuffer(0, "_RockGradient", rockGradientBuffer);
            //shader.SetBuffer(0, "_RockGradientIntervals", rockGradientIntervalsBuffer);
            //shader.SetInt("_RockGradientSize", Colors.rockColorGradient.Count);
            //shader.SetFloat("_RockGradientBanding", Colors.rockColorGradient.banding);

            shader.SetBuffer(0, "_RockColorNoise", rockColorNoise.Results);

            var rockGradientBuffers = SetColorGradient(config.rockColorGradient, "_Rock");
            var grassGradientBuffers = SetColorGradient(config.grassGradient, "_Grass");
            var grassNormalBuffers = SetValueGradient(config.grassNormalFilter, "_GrassNormal");
            var grassHeightBuffers = SetValueGradient(config.grassHeightFilter, "_GrassHeight");

            shader.SetInt("_DataSize", heightNoise.DataSize);
            shader.SetInt("_Resolution", heightNoise.Resolution);

            var dispatchNum = Mathf.CeilToInt(dataSize / (float)workGroups);
            shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);

            heightNoise.Dispose();
            rockColorNoise.Dispose();
            grassNoise.Dispose();
            rockGradientBuffers.Dispose();
            grassGradientBuffers.Dispose();
            grassNormalBuffers.Dispose();
            grassHeightBuffers.Dispose();
            canyonNoise.Dispose();
        }

        ValueGradientBuffers SetValueGradient(ValueGradient valueGradient, string prefix) {
            var keysBuffer = new ComputeBuffer(valueGradient.Keys.Count, sizeof(float));
            keysBuffer.SetData(valueGradient.Keys);

            var valuesBuffer = new ComputeBuffer(valueGradient.Keys.Count, sizeof(float));
            valuesBuffer.SetData(valueGradient.Values);

            shader.SetBuffer(0, prefix + "Keys", keysBuffer);
            shader.SetBuffer(0, prefix + "Values", valuesBuffer);
            shader.SetInt(prefix + "Size", valueGradient.Keys.Count);

            return new ValueGradientBuffers(keysBuffer, valuesBuffer);
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