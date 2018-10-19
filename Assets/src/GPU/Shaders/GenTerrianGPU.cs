using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public partial class GenTerrianGpu
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

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorBuffer, ComputeBuffer typeBuffer, TerrianChunk terrianChunk) {
            using (var noises = new GenTerrianNoiseGPU(_dataSize, _origin, _config))
            using (SetColorGradient(_config.Colors.rockColorGradient, "_Rock"))
            using (SetColorGradient(_config.Colors.grassColor, "_Grass"))
            using (_config.GrassNormalFilter.CreateBuffers(_shader, "_GrassNormal"))
            using (_config.GrassHeightFilter.CreateBuffers(_shader, "_GrassHeight"))
            using(_config.RiverNoiseFilter.CreateBuffers(_shader, "_River"))
            using(_config.StoneHeightFilter.CreateBuffers(_shader, "_StoneHeight"))
            {
                _shader.SetBuffer(0, "_NoiseBuffer", noises.Results);

                _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
                _shader.SetBuffer(0, "_ColorBuffer", colorBuffer);
                _shader.SetBuffer(0, "_TypeBuffer", typeBuffer);

                _shader.SetFloat("_MaxHeight", _config.MaxHeight);

                _shader.SetVector("_SoilColor", _config.Colors.soil);
                _shader.SetVector("_WaterColor", _config.Colors.waterColor);

                _shader.SetInt("_Size", _size);
                _shader.SetVector("_Origin", (Vector3)_origin);
                _shader.SetFloat("_HillHeight", _config.HillHeight);
                _shader.SetFloat("_PlainHeight", _config.PlainHeight);
                _shader.SetFloat("_GroundHeight", _config.GroundHeight);
                _shader.SetInt("_WaterLevel", _config.WaterLevel);

                _shader.SetVector("_StoneColor", _config.Colors.stoneColor);
                _shader.SetFloat("_StoneThreshold", _config.StoneThreshold);

                _shader.SetInt("_DataSize", _dataSize);

                var dispatchNum = Mathf.CeilToInt(_dataSize / (float)_workGroups);
                _shader.Dispatch(0, dispatchNum, dispatchNum, dispatchNum);
            }
        }

        ColorGradientBuffers SetColorGradient(ColorGradient colorGradient, string prefix) {
            var intervalsBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float));
            intervalsBuffer.SetData(colorGradient.GetKeys());

            var gradientBuffer = new ComputeBuffer(colorGradient.Count, sizeof(float) * 4);
            gradientBuffer.SetData(colorGradient.GetValues());

            _shader.SetBuffer(0, prefix + "Gradient", gradientBuffer);
            _shader.SetBuffer(0, prefix + "GradientIntervals", intervalsBuffer);
            _shader.SetInt(prefix + "GradientSize", colorGradient.Count);
            _shader.SetFloat(prefix + "GradientBanding", colorGradient.Banding);

            return new ColorGradientBuffers(intervalsBuffer, gradientBuffer);
        }
    }
}