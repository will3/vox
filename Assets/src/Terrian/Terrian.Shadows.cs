using UnityEngine;

namespace FarmVox
{

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
            WorkerQueues.meshQueue.Add(worker);

            terrianChunk.shadowsNeedsUpdate = false;
        }
    }
}
