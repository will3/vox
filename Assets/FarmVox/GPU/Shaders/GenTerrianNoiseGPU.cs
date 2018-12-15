using FarmVox.Terrain;
using UnityEngine;
using IDisposable = System.IDisposable;

namespace FarmVox.GPU.Shaders
{
    public class GenTerrianNoiseGpu : IDisposable
    {
        private int DataSize { get; set; }
        private Vector3Int Origin { get; set; }
        private TerrianConfig Config { get; set; }
        private ComputeShader Shader { get; set; }

        private const int WorkGroups = 8;

        public ComputeBuffer Results { get; private set; }

        public GenTerrianNoiseGpu(int dataSize, Vector3Int origin, TerrianConfig config)
        {
            DataSize = dataSize;
            Origin = origin;
            Config = config;
            Shader = Resources.Load<ComputeShader>("Shaders/GenTerrianNoise");
            Results = new ComputeBuffer(dataSize * dataSize * dataSize, GenTerrianNoise.Stride);

            Dispatch();
        }

        void Dispatch()
        {
            using (var heightNoise = new Perlin3DGPU(Config.Biome.HeightNoise, DataSize, Origin))
            using (var rockColorNoise = new Perlin3DGPU(Config.Biome.RockColorNoise, DataSize, Origin))
            using (var grassNoise = new Perlin3DGPU(Config.Biome.GrassNoise, DataSize, Origin))
            using (var riverNoise = new Perlin3DGPU(Config.Biome.RiverNoise, DataSize, Origin))
            using (var stoneNoise = new Perlin3DGPU(Config.Biome.StoneNoise, DataSize, Origin))
            using (var stoneNoise2 = new Perlin3DGPU(Config.Biome.StoneNoise2, DataSize, Origin))
            {
                Shader.SetBuffer(0, "_HeightBuffer", heightNoise.Results);
                Shader.SetBuffer(0, "_GrassBuffer", grassNoise.Results);
                Shader.SetBuffer(0, "_RiverBuffer", riverNoise.Results);
                Shader.SetBuffer(0, "_RockColorBuffer", rockColorNoise.Results);
                Shader.SetBuffer(0, "_StoneBuffer", stoneNoise.Results);
                Shader.SetBuffer(0, "_StoneBuffer2", stoneNoise2.Results);

                Shader.SetBuffer(0, "_NoiseBuffer", Results);

                Shader.SetInt("_DataSize", DataSize);

                var dispatchNum = Mathf.CeilToInt(DataSize / (float)WorkGroups);
                Shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
            }
        }

        public void Dispose()
        {
            Results.Dispose();
        }
    }
}