using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        public void GenerateWaterfalls(TerrianColumn column)
        {
            if (column.generatedWaterfalls)
            {
                return;
            }

            foreach (var terrianChunk in column.TerrianChunks)
            {
                GenerateWaterfalls(terrianChunk);
            }

            column.generatedWaterfalls = true;
        }

        bool ShouldGenerateWaterFalls(TerrianChunk terrianChunk) {
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
            var dataSize = chunk.dataSize;
            chunk.UpdateSurfaceCoords();
            // var waterFallNoise = new Perlin3DGPU(config.waterfallNoise, chunk.dataSize, origin);
            // waterFallNoise.Dispatch();
            // var waterFallNoiseData = waterFallNoise.Read();
            //  // / waterFallNoiseData[index];
            
            foreach(var coord in chunk.surfaceCoords) {
                var r = config.waterfallRandom.NextDouble();
                var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;
                var absY = coord.y + origin.y;
                var height = absY / config.maxHeight;
                var heightValue = config.waterfallHeightFilter.GetValue(height);
                var v = r / heightValue;
                if (v < 0 || v > 0.01f)
                {
                    continue;
                }

                GenerateWaterfall(coord + origin);
            }

            terrianChunk.waterfallsNeedsUpdate = false;

            // waterFallNoise.Dispose();
        }

        class WaterTracker {
            private float speed = 0.0f;
            private bool freefall = false;
            private float freefallFriction = 0f;
            private float friction = 0.9f;
            private float lastCost = 0.0f;
            private float maxSpeed = 5;
            private bool didReachedWater = false;
            private List<Vector3Int> coords = new List<Vector3Int>();
            private List<Vector3Int> emptyCoords = new List<Vector3Int>();
            private List<float> costs = new List<float>();

            public bool DidReachedWater
            {
                get
                {
                    return didReachedWater;
                }
            }

            public float LastCost
            {
                get
                {
                    return lastCost;
                }
            }

            public void Start(Vector3Int coord) {
                coords.Add(coord);
                costs.Add(1.0f);
            }

            public void Freefall(Vector3Int coord) {
                speed += 1f;
                freefall = true;
                lastCost = 1 / speed;

                speed *= friction;

                if (speed > maxSpeed) {
                    speed = maxSpeed;
                }

                coords.Add(coord);
                costs.Add(costs[costs.Count - 1] + lastCost);

                emptyCoords.Add(coord);
            }

            public void Flow(Vector3Int from, Vector3Int to) {
                if (freefall) {
                    speed *= freefallFriction;
                    freefall = false;
                }

                speed += 0.5f;

                speed *= friction;

                lastCost = (from - to).magnitude / speed;

                coords.Add(to);
                costs.Add(costs[costs.Count - 1] + lastCost);
            }

            public void ReachedWater() {
                didReachedWater = true;
            }

            public void Apply(Chunks chunks) {
                for (var i = 0; i < coords.Count; i++) {
                    var coord = coords[i];
                    var cost = costs[i];
                    chunks.SetColor(coord.x, coord.y, coord.z, Colors.water);
                    chunks.SetWaterfall(coord, cost);
                }
                foreach(var coord in emptyCoords) {
                    chunks.Set(coord, 1);
                }
            }
        }

        private void GenerateWaterfall(Vector3Int coord) {

            Vector3Int? nextPoint = coord;

            var cost = (float)config.waterfallRandom.NextDouble() * 1000.0f;

            var waterTracker = new WaterTracker();
            waterTracker.Start(coord);

            while (true) {
                if (nextPoint == null) {
                    break;
                }

                nextPoint = ProcessNextWater(nextPoint.Value, waterTracker);

                if (nextPoint == null)
                {
                    break;
                }

                cost += waterTracker.LastCost;

                if (cost > 1000) {
                    break;
                }
            }

            //if (waterTracker.DidReachedWater) {
                waterTracker.Apply(defaultLayer);    
            //}
        }

        private Vector3Int? ProcessNextWater(Vector3Int coord, WaterTracker waterTracker) {
            if (coord.y < config.waterLevel) {
                waterTracker.ReachedWater();
                return null;    
            }

            var down = new Vector3Int(coord.x, coord.y - 1, coord.z);

            if (defaultLayer.Get(down) < 0) {
                waterTracker.Freefall(down);
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
                        waterTracker.Flow(coord, down);
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

            if (minCoord.HasValue) {
                waterTracker.Flow(coord, minCoord.Value);    
            }

            return minCoord;
        }
    }
}
