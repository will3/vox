using UnityEngine;

namespace FarmVox
{
    public partial class Terrian
    {
        // TODO
        private void GenerateEnemies(TerrianChunk terrianChunk) {
        //    if (!terrianChunk.enemiesNeedsUpdate) {
        //        return;
        //    }

        //    return;

        //    var chunk = terrianChunk.Chunk;
        //    var origin = terrianChunk.Origin;

        //    var noise = new Perlin3DGPU(config.monsterNoise, chunk.dataSize, origin);
        //    noise.Dispatch();
        //    var data = noise.Read();
        //    noise.Dispose();

        //    foreach(var coord in chunk.surfaceCoordsUp) {
        //        var index = chunk.GetIndex(coord);

        //        var v = data[index];

        //        var r = config.monsterRandom.NextDouble();
        //        if (r > 0.01) {
        //            continue;
        //        }

        //        if (v > 0) {
        //            var go = new GameObject("monster");
        //            var actor = go.AddComponent<Actor>();
        //            actor.spriteSheetName = "monster";
        //            actor.SetNode(coord + origin);
        //            actor.terrian = this;
        //            actors.Add(actor);
        //        }
        //    }

        //    terrianChunk.enemiesNeedsUpdate = false;
        }
    }
}
