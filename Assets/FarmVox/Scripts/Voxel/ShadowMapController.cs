using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class ShadowMapController : MonoBehaviour
    {
        private readonly ShadowMap[] _shadowMaps =
        {
            new ShadowMap(LightDir.NorthEast),
            new ShadowMap(LightDir.NorthWest),
            new ShadowMap(LightDir.SouthEast),
            new ShadowMap(LightDir.SouthWest),
        };

        public LightController lightController;
        public LightDir lightDir;
        public bool debugLog;

        private ShadowMap ActiveShadowMap => _shadowMaps.Single(s => s.LightDir == lightDir);
        private LightDir _lastLightDir;

        private void Start()
        {
            if (lightController == null)
            {
                lightController = FindObjectOfType<LightController>();
                if (lightController == null)
                {
                    Logger.LogComponentNotFound(typeof(LightController));
                }
            }

            ShadowEvents.Instance.ChunkUpdated += OnChunkUpdated;
            if (_shadowMaps.Length != 4)
            {
                Debug.LogError("Please configure 4 shadow maps, one of each direction");
            }
        }

        private void Update()
        {
            lightDir = lightController.lightDir;

            if (lightDir != _lastLightDir)
            {
                ActiveShadowMap.UpdateAllChunks();
            }

            ActiveShadowMap.UpdateBuffers();

            _lastLightDir = lightDir;

            foreach (var shadowMap in _shadowMaps)
            {
                shadowMap.DebugLog = debugLog;
            }
        }

        private void OnDestroy()
        {
            ShadowEvents.Instance.ChunkUpdated -= OnChunkUpdated;

            foreach (var shadowMap in _shadowMaps)
            {
                shadowMap.Dispose();
            }
        }

        private void OnChunkUpdated(Vector3Int origin)
        {
            foreach (var shadowMap in _shadowMaps)
            {
                var origins = shadowMap.CalcChunksToRedraw(origin);

                foreach (var o in origins)
                {
                    shadowMap.SetDirty(o);
                }
            }
        }

        public IEnumerable<ComputeBuffer> GetBuffers(Vector3Int origin)
        {
            return ActiveShadowMap.GetBuffers(origin);
        }
    }
}