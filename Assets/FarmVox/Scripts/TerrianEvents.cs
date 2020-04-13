using UnityEngine;

namespace FarmVox.Scripts
{
    public class TerrianEvents
    {
        private TerrianEvents()
        {
            
        }

        private static TerrianEvents _instance;
        public static TerrianEvents Instance => _instance ?? (_instance = new TerrianEvents());

        public delegate void GroundGeneratedHandler(Vector3Int origin);

        public delegate void ColumnGeneratedHandler(Vector3Int origin);

        public event GroundGeneratedHandler GroundGenerated;
        public event ColumnGeneratedHandler ColumnGenerated;
        
        public void PublishGroundGenerated(Vector3Int origin)
        {
            GroundGenerated?.Invoke(origin);
        }

        public void PublishColumnGenerated(Vector3Int origin)
        {
            ColumnGenerated?.Invoke(origin);
        }
    }
}