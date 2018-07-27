using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        //public void UpdateShadowNeedsUpdate(Vector3Int from) {
        //    var key = new Vector3Int(from.x / size, from.y / size, from.z / size);
        //    for (var offset = 0; offset <= key.y; offset++) {
        //        SetShadowNeedsUpdate((key + new Vector3Int(-offset, -offset, -offset)) * size);
        //        SetShadowNeedsUpdate((key + new Vector3Int(-offset - 1, -offset, -offset)) * size);
        //        SetShadowNeedsUpdate((key + new Vector3Int(-offset - 1, -offset, -offset - 1)) * size);
        //        SetShadowNeedsUpdate((key + new Vector3Int(-offset, -offset, -offset - 1)) * size);
        //    }
        //}

        //void SetShadowNeedsUpdate(Vector3Int origin) {
        //    var terrianChunk = GetTerrianChunk(origin);
        //    if (terrianChunk != null)
        //    {
        //        terrianChunk.shadowsNeedsUpdate = true;
        //    }
        //}

        void GenerateShadows() {
            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < config.drawDis)
                {
                    GenerateShadow(terrianChunk);
                }
            }
        }

        void GenerateShadow(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.shadowsNeedsUpdate)
            {
                return;
            }

            var origin = terrianChunk.Origin;

            foreach (var chunks in chunksReceivingShadows)
            {
                var c = chunks.GetChunk(origin);
                if (c != null)
                {
                    c.UpdateShadows(chunksCastingShadows);
                }
            }

            terrianChunk.shadowsNeedsUpdate = false;
        }
    }
}
