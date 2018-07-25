using System.Collections.Generic;
using System.Threading;

namespace FarmVox
{
    public class ShadowWorker : IWorker
    {
        bool done = false;

        TerrianChunk terrianChunk;
        IList<Chunks> chunksReceivingShadows;
        IList<Chunks> chunksCastingShadows;
        Terrian terrian;

        public ShadowWorker(TerrianChunk terrianChunk, IList<Chunks> chunksReceivingShadows, IList<Chunks> chunksCastingShadows, Terrian terrian)
        {
            this.terrianChunk = terrianChunk;
            this.chunksReceivingShadows = chunksReceivingShadows;
            this.chunksCastingShadows = chunksCastingShadows;
            this.terrian = terrian;
        }

        public bool IsDone()
        {
            return done;
        }

        public void Start()
        {
            Work();
        }

        void Work()
        {
            var origin = terrianChunk.Origin;

            foreach (var chunks in chunksReceivingShadows)
            {
                var c = chunks.GetChunk(origin);
                if (c != null)
                {
                    c.UpdateSurfaceCoords();
                    c.UpdateShadows(chunksCastingShadows);

                    var meshWorker = new MeshWorker(c, terrianChunk, terrian.Transform, terrian.Material);
                    WorkerQueues.meshQueue.Add(meshWorker);
                }
            }
            done = true;
        }
    }
}
