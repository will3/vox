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

        public RouteWorker(RoutesMap routesMap, Vector3Int origin, Chunks defaultLayer, TerrianConfig config) {
            this.routesMap = routesMap;
            this.origin = origin;
            this.defaultLayer = defaultLayer;
            this.config = config;
        }

        public void Start()
        {
            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work() {
            var routes = routesMap.GetOrCreateRoutes(origin);
            var chunk = defaultLayer.GetChunk(origin);
            routes.LoadChunk(chunk, config);
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

            var worker = new RouteWorker(routesMap, terrianChunk.Origin, defaultLayer, config);
            WorkerQueues.routingQueue.Add(worker);

            terrianChunk.routesNeedsUpdate = false;
        }
    }
}
