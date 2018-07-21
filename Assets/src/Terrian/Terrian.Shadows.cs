using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace FarmVox
{
    public class ShadowWorker : IWorker
    {
        private bool done = false;

        TerrianChunk terrianChunk;
        IList<Chunks> chunksReceivingShadows;
        IList<Chunks> chunksCastingShadows;
        Terrian terrian;

        public ShadowWorker(TerrianChunk terrianChunk, IList<Chunks> chunksReceivingShadows, IList<Chunks> chunksCastingShadows, Terrian terrian) {
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
            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work() {
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

    public partial class Terrian
    {
        public void UpdateShadowNeedsUpdate(Vector3Int from) {
            var key = new Vector3Int(from.x / size, from.y / size, from.z / size);
            for (var offset = 0; offset <= key.y; offset++) {
                SetShadowNeedsUpdate((key + new Vector3Int(-offset, -offset, -offset)) * size);
                SetShadowNeedsUpdate((key + new Vector3Int(-offset - 1, -offset, -offset)) * size);
                SetShadowNeedsUpdate((key + new Vector3Int(-offset - 1, -offset, -offset - 1)) * size);
                SetShadowNeedsUpdate((key + new Vector3Int(-offset, -offset, -offset - 1)) * size);
            }
        }

        private void SetShadowNeedsUpdate(Vector3Int origin) {
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk != null)
            {
                terrianChunk.shadowsNeedsUpdate = true;
            }
        }

        private void GenerateShadows(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.shadowsNeedsUpdate)
            {
                return;
            }

            var worker = new ShadowWorker(terrianChunk, chunksReceivingShadows, chunksCastingShadows, this);
            WorkerQueues.shadowQueue.Add(worker);

            terrianChunk.shadowsNeedsUpdate = false;
        }
    }
}
