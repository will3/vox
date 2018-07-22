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

        TerrianChunk terrianChunk;
        Chunks defaultLayer;
        TerrianConfig config;

        public RouteWorker(TerrianChunk terrianChunk, Chunks defaultLayer, TerrianConfig config) {
            this.terrianChunk = terrianChunk;
            this.defaultLayer = defaultLayer;
            this.config = config;
        }

        public void Start()
        {
            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work() {
            var routes = terrianChunk.Routes;
            var chunk = defaultLayer.GetChunk(terrianChunk.Origin);
            routes.Clear();
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

            var worker = new RouteWorker(terrianChunk, defaultLayer, config);
            WorkerQueues.routingQueue.Add(worker);

            terrianChunk.routesNeedsUpdate = false;
        }
    }
}
