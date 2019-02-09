using FarmVox.Terrain.Routing;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class GenRoutesWorker : IWorker
    {
        private readonly Vector3Int _origin;
        private readonly RouteChunks _routes;
        private readonly Chunks _chunks;

        public GenRoutesWorker(Vector3Int origin, RouteChunks routes, Chunks chunks)
        {
            _origin = origin;
            _routes = routes;
            _chunks = chunks;
        }
        
        public void Start()
        {
            var chunk = _chunks.GetChunk(_origin);
            if (chunk != null)
            {
                _routes.LoadChunk(chunk);    
            }
        }
    }
}