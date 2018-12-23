using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.GPU.Shaders
{
    public class GenTerrianGpu
    {
        private readonly int _size;
        private readonly int _dataSize;
        private readonly int _workGroups = 8;
        private readonly ComputeShader _shader;
        private readonly TerrianConfig _config;
        private readonly Vector3Int _origin;
        private ComputeBuffer _voxelBuffer;

        public GenTerrianGpu(int size, Vector3Int origin, TerrianConfig config) {
            _size = size;
            _origin = origin;
            _config = config;

            _dataSize = size + 3;
            _shader = Resources.Load<ComputeShader>("Shaders/GenTerrian");
        }

        public ComputeBuffer CreateVoxelBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float));
        }

        public ComputeBuffer CreateColorBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 4);
        }

        public ComputeBuffer CreateTypeBuffer() {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(int));
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorBuffer, ComputeBuffer typeBuffer) {
            using (var noises = new GenTerrianNoiseGpu(_dataSize, _origin, _config))
            {
                _shader.SetBuffer(0, "_NoiseBuffer", noises.Results);

                _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
                _shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
                _shader.SetBuffer(0, "_TypeBuffer", typeBuffer);

                _shader.SetFloat("_MaxHeight", _config.MaxHeight);

                _shader.SetVector("_WaterColor", _config.Biome.Colors.WaterColor);

                _shader.SetInt("_Size", _size);
                _shader.SetVector("_Origin", (Vector3)_origin);
                _shader.SetFloat("_HillHeight", _config.Biome.HillHeight);
                _shader.SetFloat("_PlainHeight", _config.PlainHeight);
                _shader.SetFloat("_GroundHeight", _config.GroundHeight);
                _shader.SetInt("_WaterLevel", _config.WaterLevel);

                _shader.SetVector("_StoneColor", _config.Biome.Colors.StoneColor);

                _shader.SetInt("_DataSize", _dataSize);
                
                _shader.SetValueGradient(_config.Biome.GrassNormalFilter, "_GrassNormal");
                _shader.SetValueGradient(_config.Biome.GrassHeightFilter, "_GrassHeight");
                
                _shader.SetColorGradient(_config.Biome.Colors.RockColorGradient, "_Rock");
                _shader.SetColorGradient(_config.Biome.Colors.GrassColor, "_Grass");
                
                _shader.SetValueGradient(_config.Biome.HeightFilter, "" +
                                                                     "_Height" +
                                                                     "");

                var dispatchNum = Mathf.CeilToInt(_dataSize / (float)_workGroups);
                _shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
            }
        }
    }
}