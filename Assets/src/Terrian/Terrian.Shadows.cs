using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        //private bool CheckShadow(Vector3Int key, int i, int j, int k)
        //{
        //    key.x += i;
        //    key.y += j;
        //    key.z += k;
        //    var origin = key * size;

        //    var terrianChunk = getOrCreateTerrianChunk(origin);
        //    terrianChunk.generatedForShadows = true;
        //    var ready = !terrianChunk.rockNeedsUpdate && !terrianChunk.treesNeedsUpdate;
        //    return ready;
        //}

        //private bool CheckShadow(TerrianChunk terrianChunk)
        //{
        //    var key = terrianChunk.key;

        //    var ready = true;
        //    for (var j = 0; j < config.maxChunksY - key.y; j++)
        //    {
        //        ready &= CheckShadow(key, 0 + j, j, 0 + j);
        //        ready &= CheckShadow(key, 1 + j, j, 0 + j);
        //        ready &= CheckShadow(key, 0 + j, j, 1 + j);
        //        ready &= CheckShadow(key, 1 + j, j, 1 + j);
        //    }
        //    return ready;
        //}

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

            var origin = terrianChunk.Origin;
            var chunk = defaultLayer.GetChunk(origin);
            var treeChunk = treeLayer.GetChunk(origin);

            foreach (var chunks in chunksReceivingShadows)
            {
                var c = chunks.GetChunk(origin);
                if (c != null)
                {
                    c.UpdateSurfaceCoords();
                    c.UpdateShadows(chunksCastingShadows);
                }
            }

            terrianChunk.shadowsNeedsUpdate = false;
        }
    }
}
