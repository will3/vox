using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        bool ShouldGenerateWaterFalls(TerrianChunk terrianChunk)
        {
            var key = terrianChunk.key;

            for (var i = -1; i <= 1; i++)
            {
                for (var k = -1; k <= 1; k++)
                {
                    for (var j = 0; j < config.MaxChunksY; j++)
                    {
                        var origin = new Vector3Int(i + key.x, j, k + key.z) * Size;
                        var tc = GetTerrianChunk(origin);
                        if (tc == null || tc.rockNeedsUpdate)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void GenerateWaterfalls(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.waterfallsNeedsUpdate)
            {
                return;
            }

            if (!ShouldGenerateWaterFalls(terrianChunk))
            {
                return;
            }

            var origin = terrianChunk.Origin;

            var chunk = DefaultLayer.GetChunk(origin);
            var dataSize = chunk.DataSize;
            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.SurfaceCoords)
            {
                var r = config.WaterfallRandom.NextDouble();

                var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;

                var absY = coord.y + origin.y;

                var height = (absY - config.GroundHeight) / config.MaxHeight;

                var heightValue = config.WaterfallHeightFilter.GetValue(height);

                var v = r / heightValue;

                if (v < 0 || v > config.WaterfallChance)
                {
                    continue;
                }

                GenerateWaterfall(coord + origin);
            }

            terrianChunk.waterfallsNeedsUpdate = false;
        }

        class WaterTracker
        {
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

            TerrianConfig config;

            public WaterTracker(TerrianConfig config)
            {
                this.config = config;
            }

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

            public void Start(Vector3Int coord)
            {
                coords.Add(coord);
                costs.Add(1.0f);
            }

            public void Freefall(Vector3Int coord)
            {
                speed += 1f;
                freefall = true;
                lastCost = 1 / speed;

                speed *= friction;

                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }

                coords.Add(coord);
                costs.Add(costs[costs.Count - 1] + lastCost);

                emptyCoords.Add(coord);
            }

            public void Flow(Vector3Int from, Vector3Int to)
            {
                if (freefall)
                {
                    speed *= freefallFriction;
                    freefall = false;
                }

                speed += 0.5f;

                speed *= friction;

                lastCost = (from - to).magnitude / speed;

                coords.Add(to);
                costs.Add(costs[costs.Count - 1] + lastCost);
            }

            public void ReachedWater()
            {
                didReachedWater = true;
            }

            public void Apply(Chunks chunks)
            {
                for (var i = 0; i < coords.Count; i++)
                {
                    var coord = coords[i];
                    var cost = costs[i];
                    chunks.SetColor(coord.x, coord.y, coord.z, config.Colors.WaterColor);
                    chunks.SetWaterfall(coord, cost);
                }
                foreach (var coord in emptyCoords)
                {
                    chunks.Set(coord, 1);
                }
            }
        }

        private void GenerateWaterfall(Vector3Int coord)
        {

            Vector3Int? nextPoint = coord;

            var cost = (float)config.WaterfallRandom.NextDouble() * 1000.0f;

            var waterTracker = new WaterTracker(config);
            waterTracker.Start(coord);

            while (true)
            {
                if (nextPoint == null)
                {
                    break;
                }

                nextPoint = ProcessNextWater(nextPoint.Value, waterTracker);

                if (nextPoint == null)
                {
                    break;
                }

                cost += waterTracker.LastCost;

                if (cost > 1000)
                {
                    break;
                }
            }

            //if (waterTracker.DidReachedWater)
            //{
                waterTracker.Apply(DefaultLayer);
            //}
        }

        private Vector3Int? ProcessNextWater(Vector3Int coord, WaterTracker waterTracker)
        {
            if (coord.y < config.WaterLevel + config.GroundHeight)
            {
                waterTracker.ReachedWater();
                return null;
            }

            var down = new Vector3Int(coord.x, coord.y - 1, coord.z);

            if (DefaultLayer.Get(down) < 0)
            {
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

            for (var i = 0; i < coords.Length; i++)
            {
                var c = coords[i];
                var v = DefaultLayer.Get(c);
                if (v > 0)
                {
                    continue;
                }

                var coordBelow = new Vector3Int(c.x, c.y - 1, c.z);
                var v2 = DefaultLayer.Get(coordBelow);
                if (v2 > 0)
                {
                    coordsBelow.Add(coordBelow);
                }
                else
                {
                    // From the sides
                    if (i < 4)
                    {
                        waterTracker.Flow(coord, down);
                        return down;
                    }
                }
            }

            Vector3Int[] listToUse;
            if (coordsBelow.Count > 0)
            {
                listToUse = coordsBelow.ToArray();
            }
            else
            {
                listToUse = coords;
            }

            var minValue = Mathf.Infinity;
            Vector3Int? minCoord = null;

            for (var i = 0; i < listToUse.Length; i++)
            {
                var value = DefaultLayer.Get(listToUse[i]);
                if (value < 0)
                {
                    continue;
                }

                if (DefaultLayer.GetWaterfall(listToUse[i]))
                {
                    continue;
                }

                if (value < minValue)
                {
                    minValue = value;
                    minCoord = listToUse[i];
                }
            }

            if (minCoord.HasValue)
            {
                waterTracker.Flow(coord, minCoord.Value);
            }

            return minCoord;
        }
    }
}
