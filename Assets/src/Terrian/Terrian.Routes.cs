using System.Threading;
using UnityEngine;

namespace FarmVox
{
    class RouteWorker : IWorker
    {
        private bool done;
        public bool IsDone()
        {
            return done;
        }

        RoutesMap routesMap;
        Vector3Int origin;
        Chunks defaultLayer;
        TerrianConfig config;
        TerrianChunk terrianChunk;
        public RouteWorker(RoutesMap routesMap, Vector3Int origin, Chunks defaultLayer, TerrianChunk terrianChunk, TerrianConfig config) {
            this.routesMap = routesMap;
            this.origin = origin;
            this.defaultLayer = defaultLayer;
            this.config = config;
            this.terrianChunk = terrianChunk;
        }

        public void Start()
        {
            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work() {
            var routes = routesMap.GetOrCreateRoutes(origin);
            var chunk = defaultLayer.GetChunk(origin);
            routes.LoadChunk(chunk, terrianChunk, config);
            done = true;
        }
    }

    public partial class Terrian
    {
        public void GenerateRoutes(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.routesNeedsUpdate)
            {
                return;
            }

            var routesMap = Finder.FindRoutesMap();
            var worker = new RouteWorker(routesMap, terrianChunk.Origin, defaultLayer, terrianChunk, config);
            WorkerQueues.routingQueue.Add(worker);

            terrianChunk.routesNeedsUpdate = false;
        }
    }
}
