using UnityEngine;

namespace FarmVox.Scripts.GPU.Shaders
{
    public class GenTerrianGpu
    {
        private readonly int _size;
        private readonly int _dataSize;
        private readonly int[] _workGroups = {8, 8, 4};
        private readonly ComputeShader _shader;
        private readonly GroundConfig _groundConfig;
        private readonly WaterConfig _waterConfig;
        private readonly StoneConfig _stoneConfig;
        private readonly BoundsInt _bounds;
        private readonly Vector3Int _origin;
        private ComputeBuffer _voxelBuffer;

        public GenTerrianGpu(
            int size,
            Vector3Int origin,
            GroundConfig groundConfig,
            WaterConfig waterConfig,
            StoneConfig stoneConfig,
            BoundsInt bounds)
        {
            _size = size;
            _origin = origin;
            _groundConfig = groundConfig;
            _waterConfig = waterConfig;
            _stoneConfig = stoneConfig;
            _bounds = bounds;

            _dataSize = size + 3;
            _shader = Resources.Load<ComputeShader>("Shaders/GenTerrian");
        }

        public ComputeBuffer CreateVoxelBuffer()
        {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float));
        }

        public ComputeBuffer CreateColorBuffer()
        {
            return new ComputeBuffer(_dataSize * _dataSize * _dataSize, sizeof(float) * 4);
        }

        public void Dispatch(ComputeBuffer voxelBuffer, ComputeBuffer colorBuffer)
        {
            using (var rockColorBuffer = new Perlin3DGpu(_groundConfig.rockColorNoise, _dataSize, _origin))
            using (var heightBuffer = new Perlin3DGpu(_groundConfig.heightNoise, _dataSize, _origin))
            using (var grassBuffer = new Perlin3DGpu(_groundConfig.grassNoise, _dataSize, _origin))
            using (var stoneBuffer = new Perlin3DGpu(_stoneConfig.noise, _dataSize, _origin))
            using (var edgeNoiseBuffer = new Perlin3DGpu(_groundConfig.edgeNoise, _dataSize, _origin))
            {
                _shader.SetBuffer(0, "_RockColorBuffer", rockColorBuffer.Results);
                _shader.SetBuffer(0, "_HeightBuffer", heightBuffer.Results);
                _shader.SetBuffer(0, "_GrassBuffer", grassBuffer.Results);
                _shader.SetBuffer(0, "_StoneBuffer", stoneBuffer.Results);
                _shader.SetBuffer(0, "_EdgeBuffer", edgeNoiseBuffer.Results);

                _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
                _shader.SetBuffer(0, "_ColorBuffer", colorBuffer);

                _shader.SetFloat("_MaxHeight", _groundConfig.maxHeight);

                _shader.SetInt("_Size", _size);
                _shader.SetVector("_Origin", (Vector3) _origin);
                _shader.SetFloat("_HillHeight", _groundConfig.hillHeight);
                _shader.SetFloat("_GroundHeight", _groundConfig.groundHeight);
                _shader.SetInt("_WaterLevel", _waterConfig.waterLevel);

                _shader.SetInt("_DataSize", _dataSize);

                _shader.SetValueGradient(_groundConfig.grassNormalFilter, "_GrassNormal");
                _shader.SetValueGradient(_groundConfig.grassHeightFilter, "_GrassHeight");

                _shader.SetColorGradient(_groundConfig.rockColor, "_Rock");
                _shader.SetColorGradient(_groundConfig.grassColor, "_Grass");
                _shader.SetColorGradient(_stoneConfig.color, "_Stone");

                _shader.SetValueGradient(_groundConfig.heightFilter, "_Height");

                _shader.SetFloat("_GrassValue", _groundConfig.grassValue);

                _shader.SetValueGradient(_stoneConfig.heightCurve, "_StoneHeight");
                _shader.SetValueGradient(_groundConfig.edgeCurve, "_Edge");
                _shader.SetFloat("_EdgeDistance", _groundConfig.edgeDistance);

                _shader.SetInts("_Bounds", _bounds.min.x, _bounds.min.z, _bounds.max.x, _bounds.max.z);

                _shader.Dispatch(0,
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[0]),
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[1]),
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[2]));
            }
        }
    }
}