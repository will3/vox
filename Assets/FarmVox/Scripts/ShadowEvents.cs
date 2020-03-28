using UnityEngine;

namespace FarmVox.Scripts
{
    public class TerrainEvents
    {
        private TerrainEvents()
        {
            
        }

        private static TerrainEvents _instance;
        public static TerrainEvents Instance => _instance ?? (_instance = new TerrainEvents());

        public delegate void GroundGeneratedHandler(Vector3Int origin);

        public event GroundGeneratedHandler GroundGenerated;
        
        public void PublishGroundGenerated(Vector3Int origin)
        {
            GroundGenerated?.Invoke(origin);
        }
    }
    
    public class ShadowEvents
    {
        private ShadowEvents()
        {
        }

        private static ShadowEvents _instance;

        public static ShadowEvents Instance => _instance ?? (_instance = new ShadowEvents());

        public delegate void ChunkUpdatedHandler(Vector3Int origin);

        public delegate void ShadowMapUpdatedHandler(Vector3Int origin, int dataSize, ComputeBuffer[] buffers);

        public event ChunkUpdatedHandler ChunkUpdated;
        public event ShadowMapUpdatedHandler ShadowMapUpdated;

        public void PublishChunkUpdated(Vector3Int origin)
        {
            ChunkUpdated?.Invoke(origin);
        }

        public void PublishShadowMapUpdated(Vector3Int origin, int dataSize, ComputeBuffer[] buffers)
        {
            ShadowMapUpdated?.Invoke(origin, dataSize, buffers);
        }
    }
}