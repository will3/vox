using System.Collections.Generic;
using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class GenWaterfallWorker : IWorker
    {
        private readonly TerrianChunk _terrianChunk;
        private readonly Chunks _defaultLayer;
        private readonly TerrianConfig _config;

        public GenWaterfallWorker(TerrianChunk terrianChunk, Chunks defaultLayer, TerrianConfig config)
        {
            _terrianChunk = terrianChunk;
            _defaultLayer = defaultLayer;
            _config = config;
        }
            
        public void Start()
        {
            var origin = _terrianChunk.Origin;

            var chunk = _defaultLayer.GetChunk(origin);
            var dataSize = chunk.DataSize;
            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.SurfaceCoords)
            {
                var r = _config.Biome.WaterfallRandom.NextDouble();

                var index = coord.x * dataSize * dataSize + coord.y * dataSize + coord.z;

                var absY = coord.y + origin.y;

                var height = (absY - _config.GroundHeight) / _config.MaxHeight;

                var heightValue = _config.Biome.WaterfallHeightFilter.GetValue(height);

                var v = r / heightValue;

                if (v < 0 || v > _config.Biome.WaterfallChance)
                {
                    continue;
                }

                GenerateWaterfall(coord + origin);
            }
        }
            
        private void GenerateWaterfall(Vector3Int coord)
        {

            Vector3Int? nextPoint = coord;

            var cost = (float)_config.Biome.WaterfallRandom.NextDouble() * 1000.0f;

            var waterTracker = new WaterTracker(_config);
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
            waterTracker.Apply(_defaultLayer);
            //}
        }
            
        private Vector3Int? ProcessNextWater(Vector3Int coord, WaterTracker waterTracker)
        {
            if (coord.y < _config.WaterLevel + _config.GroundHeight)
            {
                waterTracker.ReachedWater();
                return null;
            }

            var down = new Vector3Int(coord.x, coord.y - 1, coord.z);

            if (_defaultLayer.Get(down) < 0)
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
                var v = _defaultLayer.Get(c);
                if (v > 0)
                {
                    continue;
                }

                var coordBelow = new Vector3Int(c.x, c.y - 1, c.z);
                var v2 = _defaultLayer.Get(coordBelow);
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
                var value = _defaultLayer.Get(listToUse[i]);
                if (value < 0)
                {
                    continue;
                }

                if (_defaultLayer.GetWaterfall(listToUse[i]))
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