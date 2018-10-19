using UnityEngine;
using IDisposable = System.IDisposable;

namespace FarmVox
{
    struct GenTerrianNoise
    {
        public float height;
        public float rockColor;
        public float grass;
        public float river;
        public float stone;
        public float stone2;

        public static int Stride
        {
            get
            {
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

        public GenTerrianNoiseGPU(int dataSize, Vector3Int origin, TerrianConfig config)
        {
            this.dataSize = dataSize;
            this.origin = origin;
            this.config = config;
            shader = Resources.Load<ComputeShader>("Shaders/GenTerrianNoise");
            results = new ComputeBuffer(dataSize * dataSize * dataSize, GenTerrianNoise.Stride);

            Dispatch();
        }

        void Dispatch()
        {
            using (var heightNoise = new Perlin3DGPU(config.HeightNoise, dataSize, origin))
            using (var rockColorNoise = new Perlin3DGPU(config.RockColorNoise, dataSize, origin))
            using (var grassNoise = new Perlin3DGPU(config.GrassNoise, dataSize, origin))
            using (var riverNoise = new Perlin3DGPU(config.RiverNoise, dataSize, origin))
            using (var stoneNoise = new Perlin3DGPU(config.StoneNoise, dataSize, origin))
            using (var stoneNoise2 = new Perlin3DGPU(config.StoneNoise2, dataSize, origin))
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
}