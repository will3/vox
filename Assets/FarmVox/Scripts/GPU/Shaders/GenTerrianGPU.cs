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
            using (var noisesBuffer = NoisePacker.PackNoises(new[]
            {
                _groundConfig.heightNoise,
                _groundConfig.rockColorNoise,
                _groundConfig.grassNoise,
                _stoneConfig.noise,
                _groundConfig.edgeNoise
            }))
            {
                _shader.SetBuffer(0, "_NoisesBuffer", noisesBuffer);

                _shader.SetBuffer(0, "_VoxelBuffer", voxelBuffer);
                _shader.SetBuffer(0, "_ColorBuffer", colorBuffer);

                _shader.SetFloat("_MaxHeight", _groundConfig.maxHeight);

                _shader.SetInt("_Size", _size);
                _shader.SetVector("_Origin", (Vector3) _origin);
                _shader.SetFloat("_HillHeight", _groundConfig.hillHeight);
                _shader.SetFloat("_GroundHeight", _groundConfig.groundHeight);
                _shader.SetInt("_WaterLevel", _waterConfig.waterLevel);

                _shader.SetInt("_DataSize", _dataSize);

                _shader.SetValueGradient("_GrassNormalCurve", _groundConfig.grassNormalFilter);

                _shader.SetValueGradient("_GrassHeightCurve", _groundConfig.grassHeightFilter);

                _shader.SetColorGradient(_groundConfig.rockColor, "_Rock");
                _shader.SetColorGradient(_groundConfig.grassColor, "_Grass");
                _shader.SetColorGradient(_stoneConfig.color, "_Stone");

                _shader.SetValueGradient("_HeightCurve", _groundConfig.heightFilter);

                _shader.SetFloat("_GrassValue", _groundConfig.grassValue);

                _shader.SetValueGradient("_StoneHeightCurve", _stoneConfig.heightCurve);
                _shader.SetValueGradient("_EdgeHeightCurve", _groundConfig.edgeCurve);
                _shader.SetFloat("_EdgeDistance", _groundConfig.edgeDistance);
                _shader.SetInt("_UseEdges", _groundConfig.useEdges ? 1 : 0);

                _shader.SetInts("_Bounds", _bounds.min.x, _bounds.min.z, _bounds.max.x, _bounds.max.z);

                _shader.Dispatch(0,
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[0]),
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[1]),
                    Mathf.CeilToInt(_dataSize / (float) _workGroups[2]));
            }
        }
    }
}