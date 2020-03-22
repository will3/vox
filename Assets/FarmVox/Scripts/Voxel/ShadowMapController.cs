using System.Linq;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class ShadowMapController : MonoBehaviour
    {
        public ShadowMap[] shadowMaps;
        public LightController lightController;
        public LightDir lightDir;
        public bool debugLog;

        private ShadowMap ActiveShadowMap => shadowMaps.Single(s => s.lightDir == lightDir);
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
            shadowMaps = GetComponents<ShadowMap>();
            if (shadowMaps.Length != 4)
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

            foreach (var shadowMap in shadowMaps)
            {
                shadowMap.debugLog = debugLog;
            }
        }

        private void OnDestroy()
        {
            ShadowEvents.Instance.ChunkUpdated -= OnChunkUpdated;
        }

        private void OnChunkUpdated(Vector3Int origin)
        {
            foreach (var shadowMap in shadowMaps)
            {
                var origins = shadowMap.CalcChunksToRedraw(origin);

                foreach (var o in origins)
                {
                    shadowMap.SetDirty(o);
                }
            }
        }
    }
}