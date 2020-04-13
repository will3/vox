using UnityEngine;

namespace FarmVox.Scripts
{
    public class ShadowEvents
    {
        private ShadowEvents()
        {
        }

        private static ShadowEvents _instance;

        public static ShadowEvents Instance => _instance ?? (_instance = new ShadowEvents());

        public delegate void ChunkUpdatedHandler(Vector3Int origin);

        public delegate void ShadowMapUpdatedHandler(Vector3Int origin);

        public event ChunkUpdatedHandler ChunkUpdated;
        public event ShadowMapUpdatedHandler ShadowMapUpdated;

        public void PublishChunkUpdated(Vector3Int origin)
        {
            ChunkUpdated?.Invoke(origin);
        }

        public void PublishShadowMapUpdated(Vector3Int origin)
        {
            ShadowMapUpdated?.Invoke(origin);
        }
    }
}