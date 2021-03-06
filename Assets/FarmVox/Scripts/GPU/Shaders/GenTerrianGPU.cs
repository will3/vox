﻿using UnityEngine;

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
            BoundsInt bounds,
            ComputeShader shader)
        {
            _size = size;
            _origin = origin;
            _groundConfig = groundConfig;
            _waterConfig = waterConfig;
            _stoneConfig = stoneConfig;
            _bounds = bounds;

            _dataSize = size;
            _shader = shader;
        }

        public GenTerrianResults Dispatch()
        {
            var results = new GenTerrianResults(_dataSize);

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

                _shader.SetBuffer(0, "_VoxelBuffer", results.VoxelBuffer);
                _shader.SetBuffer(0, "_ColorBuffer", results.ColorBuffer);
                _shader.SetBuffer(0, "_NormalBuffer", results.NormalBuffer);

                _shader.SetFloat("_MaxHeight", _groundConfig.maxHeight);

                _shader.SetInt("_Size", _size);
                _shader.SetVector("_Origin", (Vector3) _origin);
                _shader.SetFloat("_HillHeight", _groundConfig.hillHeight);
                _shader.SetFloat("_GroundHeight", _groundConfig.groundHeight);
                _shader.SetInt("_WaterLevel", _waterConfig.waterLevel);

                _shader.SetInt("_DataSize", _dataSize);

                _shader.SetValueGradient("_GrassNormalCurve", _groundConfig.grassNormalFilter);

                _shader.SetValueGradient("_GrassHeightCurve", _groundConfig.grassHeightFilter);

                _shader.SetColorGradient("_RockColorGradient", _groundConfig.rockColor);
                _shader.SetColorGradient("_GrassColorGradient", _groundConfig.grassColor);
                _shader.SetColorGradient("_StoneColorGradient", _stoneConfig.color);

                _shader.SetValueGradient("_HeightCurve", _groundConfig.heightFilter);

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

            return results;
        }
    }
}