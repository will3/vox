using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace FarmVox
{

    public partial class Terrian
    {
        public bool Draw(Chunks chunks, Vector3Int origin, Transform transform, Material material, TerrianChunk terrianChunk)
        {
            if (!chunks.HasChunk(origin))
            {
                return false;
            }

            var chunk = chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return false;
            }

            var worker = new MeshWorker(chunk, terrianChunk, transform, material);
            WorkerQueues.meshQueue.Add(worker);

            chunk.Dirty = false;
            return true;
        }

        public void Draw(HashSet<Vector3Int> updatedTerrianChunks) {
            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < config.drawDis)
                {
                    foreach (var chunks in chunksToDraw)
                    {
                        if (Draw(chunks, terrianChunk.Origin, Transform, material, terrianChunk))
                        {
                            updatedTerrianChunks.Add(terrianChunk.Origin);
                        }
                    }
                }
            }
        }
    }
}
