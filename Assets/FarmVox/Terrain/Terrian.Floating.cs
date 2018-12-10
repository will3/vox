using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmVox.Terrain
{
    // WIP

    public partial class Terrian
    {
        public void UpdateFloating(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.floatingNeedsUpdate)
            {
                return;
            }

            if (terrianChunk.Origin.y == 0)
            {
                Vector3Int startCoord = new Vector3Int(Size / 2, config.WaterLevel - 1, Size / 2) + terrianChunk.Origin;

                var leads = new HashSet<Vector3Int>() { startCoord };
                var visited = new HashSet<Vector3Int>() { startCoord };

                var start = terrianChunk.Origin;
                var end = start + new Vector3Int(Size, Size, Size);
                start.y = config.WaterLevel - 1;

                var startTime = System.DateTime.Now;

                while (leads.Count > 0)
                {
                    var current = leads.ElementAt(0);
                    Visit(current, -1, 0, 0, leads, visited, start, end);
                    Visit(current, 1, 0, 0, leads, visited, start, end);
                    Visit(current, 0, -1, 0, leads, visited, start, end);
                    Visit(current, 0, 1, 0, leads, visited, start, end);
                    Visit(current, 0, 0, -1, leads, visited, start, end);
                    Visit(current, 0, 0, 1, leads, visited, start, end);

                    terrianChunk.SetFloating(current);
                    leads.Remove(current);
                    visited.Add(current);
                }

                var endTime = System.DateTime.Now;

                Debug.Log((endTime - startTime).Milliseconds);
            }
            else
            {
                //var left = GetNeighbourTerrianChunk(terrianChunk, -1, 0, 0);
                //var right = GetNeighbourTerrianChunk(terrianChunk, 1, 0, 0);
                //var bot = GetNeighbourTerrianChunk(terrianChunk, 0, -1, 0);
                //var top = GetNeighbourTerrianChunk(terrianChunk, 0, 1, 0);
                //var back = GetNeighbourTerrianChunk(terrianChunk, 0, 0, -1);
                //var forward = GetNeighbourTerrianChunk(terrianChunk, 0, 0, 1);

                //// No neighbour hanging completed
                //if (left.hangingNeedsUpdate && right.hangingNeedsUpdate &&
                //    bot.hangingNeedsUpdate && top.hangingNeedsUpdate &&
                //    back.hangingNeedsUpdate && forward.hangingNeedsUpdate)
                //{
                //    return;
                //}    
            }

            terrianChunk.floatingNeedsUpdate = false;
        }

        private TerrianChunk GetNeighbourTerrianChunk(TerrianChunk terrianChunk, int di, int dj, int dk)
        {
            var key = terrianChunk.key;
            key.x += di;
            key.y += dj;
            key.z += dk;
            var origin = key * Size;
            return GetTerrianChunk(origin);
        }

        //HashSet<Vector3Int> floatingUpdated = new HashSet<Vector3Int>();

        //bool ShouldUpdateHanging(Vector3Int coord) {
        //    var key = new Vector3Int(coord.x / size, coord.y / size, coord.z / size);
        //    for (var i = -1; i <= 1; i++) 
        //    {
        //        for (var j = 0; j < config.maxChunksY; j++)
        //        {
        //            for (var k = -1; k <= 1; k++)
        //            {
        //                var o = (new Vector3Int(i, j, k) + key) * size;
        //                var tc = GetTerrianChunk(o);
        //                if (tc == null || tc.rockNeedsUpdate) {
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        //public void UpdateFloating(Vector3Int xz) {
        //    if (floatingUpdated.Contains(xz)) {
        //        return;
        //    }

        //    if (!ShouldUpdateHanging(xz)) {
        //        return;
        //    }

        //    var j = config.waterLevel - 1;
        //    var i = size / 2;
        //    var k = size / 2;
        //    var startCoord = new Vector3Int(i, j, k) + xz;
        //    var chunksGroup = new ChunksGroup(new Chunks[] { waterLayer, defaultLayer });

        //    Assert.IsTrue(chunksGroup.Any(startCoord));
        //    floatingUpdated.Add(xz);
        //}

        void Visit(Vector3Int coord, int di, int dj, int dk,
                    HashSet<Vector3Int> leads, HashSet<Vector3Int> visited, Vector3Int start, Vector3Int end)
        {
            var c = new Vector3Int(coord.x + di, coord.y + dj, coord.z + dk);

            if (c.x < start.x || c.x >= end.x ||
                c.y < start.y || c.y >= end.y ||
                c.z < start.z || c.z >= end.z)
            {
                return;
            }

            var any = coord.y < config.WaterLevel || DefaultLayer.Get(c.x, c.y, c.z) > 0;
            if (any)
            {
                if (!visited.Contains(c))
                {
                    leads.Add(c);
                }
            }
            else
            {
                visited.Add(c);
            }
        }
    }
}
