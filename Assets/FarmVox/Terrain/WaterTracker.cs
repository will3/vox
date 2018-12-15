using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain
{
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
                chunks.SetColor(coord.x, coord.y, coord.z, config.Biome.Colors.WaterColor);
                chunks.SetWaterfall(coord, cost);
            }
            foreach (var coord in emptyCoords)
            {
                chunks.Set(coord, 1);
            }
        }
    }
}