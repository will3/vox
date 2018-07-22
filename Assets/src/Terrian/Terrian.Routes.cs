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

        public RouteWorker(TerrianChunk terrianChunk, Chunks defaultLayer) {
            this.terrianChunk = terrianChunk;
            this.defaultLayer = defaultLayer;
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
            routes.LoadChunk(chunk);
            done = true;
        }
    }

    public partial class Terrian
    {
        private bool ShouldUpdateRoutes(TerrianChunk terrianChunk) {
            var key = terrianChunk.key;
            for (var j = -1; j <= 1; j++)
            {
                for (var i = -1; i <= 1; i++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        var key2 = key + new Vector3Int(i, j, k);
                        if (key2.y < 0 || key2.y >= config.maxChunksY) {
                            continue;
                        }

                        var origin = key2 * size;
                        var tc = GetTerrianChunk(origin);
                        if (tc == null || tc.rockNeedsUpdate) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void GenerateRoutes(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.routesNeedsUpdate)
            {
                return;
            }

            if (!ShouldUpdateRoutes(terrianChunk)) {
                return;
            }

            var worker = new RouteWorker(terrianChunk, defaultLayer);
            WorkerQueues.routingQueue.Add(worker);

            terrianChunk.routesNeedsUpdate = false;
        }
    }
}
