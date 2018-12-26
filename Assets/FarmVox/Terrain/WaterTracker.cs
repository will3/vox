using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain
{
    class WaterTracker
    {
        private const float Friction = 0.9f;
        private const float FreefallFriction = 0f;
        private const float MaxSpeed = 5;
        
        private float _speed;
        private bool _freefall;
        private float _lastCost;
        private bool _didReachedWater;
        
        private readonly List<Vector3Int> _coords = new List<Vector3Int>();
        private readonly List<Vector3Int> _emptyCoords = new List<Vector3Int>();
        private readonly List<float> _costs = new List<float>();

        readonly TerrianConfig _config;

        public WaterTracker(TerrianConfig config)
        {
            _config = config;
        }

        public bool DidReachedWater
        {
            get
            {
                return _didReachedWater;
            }
        }

        public float LastCost
        {
            get
            {
                return _lastCost;
            }
        }

        public void Start(Vector3Int coord)
        {
            _coords.Add(coord);
            _costs.Add(1.0f);
        }

        public void Freefall(Vector3Int coord)
        {
            _speed += 1f;
            _freefall = true;
            _lastCost = 1 / _speed;

            _speed *= Friction;

            if (_speed > MaxSpeed)
            {
                _speed = MaxSpeed;
            }

            _coords.Add(coord);
            _costs.Add(_costs[_costs.Count - 1] + _lastCost);

            _emptyCoords.Add(coord);
        }

        public void Flow(Vector3Int from, Vector3Int to)
        {
            if (_freefall)
            {
                _speed *= FreefallFriction;
                _freefall = false;
            }

            _speed += 0.5f;

            _speed *= Friction;

            _lastCost = (from - to).magnitude / _speed;

            _coords.Add(to);
            _costs.Add(_costs[_costs.Count - 1] + _lastCost);
        }

        public void ReachedWater()
        {
            _didReachedWater = true;
        }

        public void Apply(Chunks chunks)
        {
            for (var i = 0; i < _coords.Count; i++)
            {
                var coord = _coords[i];
                var cost = _costs[i];
                chunks.SetColor(coord.x, coord.y, coord.z, _config.Biome.Colors.WaterColor);
                chunks.SetWaterfall(coord, cost);
            }
            foreach (var coord in _emptyCoords)
            {
                chunks.Set(coord, 1);
            }
        }
    }
}