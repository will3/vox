using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        void GenerateShadows() {
            foreach (var column in columns.Values) {
                if (column.generatedShadows) {
                    continue;
                }
                if (!ShouldGenerateShadow(column)) {
                    continue;
                }

                foreach (var terrianChunk in column.TerrianChunks) {
                    GenerateShadow(terrianChunk);
                }

                column.generatedShadows = true;
            }
        }

        void GenerateShadow(TerrianChunk terrianChunk)
        {
            var origin = terrianChunk.Origin;

            foreach (var chunks in chunksReceivingShadows)
            {
                var c = chunks.GetChunk(origin);
                if (c != null)
                {
                    c.UpdateShadows(chunksCastingShadows);
                }
            }
        }

        bool ShouldGenerateShadow(TerrianColumn column) {
            var shadowLength = 3;
            for (var i = 0; i < shadowLength; i++) {
                for (var k = 0; k < shadowLength; k++) {
                    var origin = column.Origin + new Vector3Int(i, 0, k) * size;
                    var c = GetColumn(origin);
                    if (c == null || !c.generatedColliders) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
