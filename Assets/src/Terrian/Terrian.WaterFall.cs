using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        private bool ShouldGenerateWaterFalls(TerrianChunk terrianChunk) {
            var key = terrianChunk.key;

            for (var i = -1; i <= 1; i ++) {
                for (var k = -1; k <= 1; k ++) {
                    for (var j = 0; j < config.maxChunksY; j++) {
                        var origin = new Vector3Int(i + key.x, j, k + key.z) * size;
                        var tc = GetTerrianChunk(origin);
                        if (tc == null || tc.rockNeedsUpdate) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void GenerateWaterfalls(TerrianChunk terrianChunk) {
            if (!terrianChunk.waterfallsNeedsUpdate) {
                return;
            }

            if (!ShouldGenerateWaterFalls(terrianChunk)) {
                return;
            }

            var origin = terrianChunk.Origin;

            var chunk = defaultLayer.GetChunk(origin);

            chunk.UpdateSurfaceCoords();

            foreach(var coord in chunk.surfaceCoords) {
                var r = config.waterfallRandom.NextDouble();

                var absY = coord.y + origin.y;
                var height = absY / config.maxHeight;
                var heightValue = config.waterfallHeightFilter.GetValue(height);

                if (r / heightValue > 0.03f)
                {
                    continue;
                }

                GenerateWaterfall(coord + origin);
            }

            terrianChunk.waterfallsNeedsUpdate = false;
        }

        private void GenerateWaterfall(Vector3Int coord) {
            defaultLayer.SetColor(coord.x, coord.y, coord.z, Colors.special);

            Vector3Int? nextPoint = coord;

            var count = 0;

            while (true) {
                if (nextPoint == null) {
                    break;
                }
                if (nextPoint.Value.y <= config.waterLevel) {
                    break;
                }
                nextPoint = GetNextWaterPoint(nextPoint.Value);
                defaultLayer.SetColor(nextPoint.Value.x, nextPoint.Value.y, nextPoint.Value.z, Colors.water);
                defaultLayer.SetWaterfall(nextPoint.Value, count);
                count++;
                if (count > 1000) {
                    break;
                }
            }
        }

        private Vector3Int? GetNextWaterPoint(Vector3Int coord) {
            if (coord.y <= config.waterLevel) {
                return null;    
            }

            var down = new Vector3Int(coord.x, coord.y - 1, coord.z);

            if (defaultLayer.Get(down) < 0) {
                defaultLayer.Set(down, 1);
                return down;
            }

            Vector3Int[] coords = {
                new Vector3Int(coord.x, coord.y, coord.z  - 1),
                new Vector3Int(coord.x - 1, coord.y, coord.z),
                new Vector3Int(coord.x + 1, coord.y, coord.z),
                new Vector3Int(coord.x, coord.y, coord.z + 1),

                new Vector3Int(coord.x - 1, coord.y, coord.z - 1),
                new Vector3Int(coord.x + 1, coord.y, coord.z - 1),
                new Vector3Int(coord.x - 1, coord.y, coord.z + 1),
                new Vector3Int(coord.x + 1, coord.y, coord.z + 1),
            };

            var coordsBelow = new List<Vector3Int>();

            for (var i = 0; i < coords.Length; i ++) {
                var c = coords[i];
                var v = defaultLayer.Get(c);
                if (v > 0) {
                    continue;
                }

                var coordBelow = new Vector3Int(c.x, c.y - 1, c.z);
                var v2 = defaultLayer.Get(coordBelow);
                if (v2 > 0) {
                    coordsBelow.Add(coordBelow);
                } else {
                    // From the sides
                    if (i < 4) {
                        return down;      
                    }
                }
            }

            Vector3Int[] listToUse;
            if (coordsBelow.Count > 0) {
                listToUse = coordsBelow.ToArray();
            } else {
                listToUse = coords;
            }
                
            var minValue = Mathf.Infinity;
            Vector3Int? minCoord = null;

            for (var i = 0; i < listToUse.Length; i++) {
                var value = defaultLayer.Get(listToUse[i]);
                if (value < 0) {
                    continue;
                }

                if (defaultLayer.GetWaterfall(listToUse[i])) {
                    continue;
                }

                if (value < minValue) {
                    minValue = value;
                    minCoord = listToUse[i];
                }
            }

            return minCoord;
        }
    }
}
