﻿using UnityEngine;
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
    }
}
