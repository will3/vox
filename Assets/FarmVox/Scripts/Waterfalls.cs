using System.Collections.Generic;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;
using Random = System.Random;

namespace FarmVox.Scripts
{
    public class Waterfalls : MonoBehaviour
    {
        public float shadowStrength = 0.2f;
        public float speed = 2.0f;
        public float width = 10.0f;
        public float min = 0.9f;
        public float variance = 0.7f;
        public Random random = new Random();
        public float chance = 0.01f;

        public ValueGradient heightFilter = new ValueGradient(new Dictionary<float, float>
        {
            {0, 0},
            {0.3f, 0},
            {0.5f, 1},
            {1.0f, 1}
        });

        private readonly Dictionary<Vector3Int, Dictionary<Vector3Int, float>> _data =
            new Dictionary<Vector3Int, Dictionary<Vector3Int, float>>();

        public Chunks groundChunks;
        public Water water;
        public Ground ground;
        public int size = 32;

        public float GetWaterfall(Vector3Int coord)
        {
            var origin = coord.GetOrigin(size);

            if (!_data.TryGetValue(origin, out var chunk))
            {
                return 0;
            }

            return chunk.TryGetValue(coord - origin, out var value) ? value : 0;
        }

        public Dictionary<Vector3Int, float> GetWaterfallChunk(Vector3Int origin)
        {
            return _data.TryGetValue(origin, out var chunk) ? chunk : null;
        }

        public void SetWaterfall(Vector3Int coord, float value)
        {
            var origin = coord.GetOrigin(size);

            if (!_data.TryGetValue(origin, out var chunk))
            {
                chunk = new Dictionary<Vector3Int, float>();
                _data[origin] = chunk;
            }

            chunk[coord - origin] = value;
        }

        public void GenerateWaterfalls(TerrianChunk terrianChunk)
        {
            var origin = terrianChunk.Origin;

            var chunk = groundChunks.GetChunk(origin);
            chunk.UpdateSurfaceCoords();

            var groundConfig = ground.config;

            foreach (var coord in chunk.SurfaceCoords)
            {
                var r = random.NextDouble();

                var absY = coord.y + origin.y;

                var height = (absY - groundConfig.groundHeight) / groundConfig.maxHeight;

                var heightValue = heightFilter.GetValue(height);

                var v = r / heightValue;

                if (v < 0 || v > chance)
                {
                    continue;
                }

                GenerateWaterfall(coord + origin);
            }
        }

        private void GenerateWaterfall(Vector3Int coord)
        {
            var nextPoint = coord;

            var waterTracker = new WaterTracker(water.config, this);
            waterTracker.Start(coord);

            var count = 0;
            while (true)
            {
                var point = ProcessNextWater(nextPoint, waterTracker);

                if (point == null)
                {
                    break;
                }

                if (nextPoint == point.Value)
                {
                    break;
                }

                nextPoint = point.Value;

                count++;
                if (count > 128)
                {
                    break;
                }
            }

            if (waterTracker.ReachedWater)
            {
                waterTracker.Apply(groundChunks);
            }
        }


        private Vector3Int? ProcessNextWater(Vector3Int coord, WaterTracker waterTracker)
        {
            if (coord.y < water.config.waterLevel)
            {
                waterTracker.ReachedWater = true;
                return null;
            }

            var down = new Vector3Int(coord.x, coord.y - 1, coord.z);

            if (groundChunks.Get(down) < 0)
            {
                waterTracker.FreeFall(down);
                return down;
            }

            Vector3Int[] coords =
            {
                new Vector3Int(coord.x, coord.y, coord.z - 1),
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
                var v = groundChunks.Get(c);
                if (v > 0)
                {
                    continue;
                }

                var coordBelow = new Vector3Int(c.x, c.y - 1, c.z);
                var v2 = groundChunks.Get(coordBelow);
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

            var listToUse = coordsBelow.Count > 0 ? coordsBelow.ToArray() : coords;

            var minValue = Mathf.Infinity;
            Vector3Int? minCoord = null;

            foreach (var t in listToUse)
            {
                var value = groundChunks.Get(t);
                if (value < 0)
                {
                    continue;
                }

                if (GetWaterfall(t) > 0)
                {
                    continue;
                }

                if (value >= minValue) continue;

                minValue = value;
                minCoord = t;
            }

            if (minCoord.HasValue)
            {
                waterTracker.Flow(coord, minCoord.Value);
            }

            return minCoord;
        }
    }
}