using System.Collections.Generic;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain
{   
    class WaterTracker
    {
        private const float Friction = 0.9f;
        private const float FreeFallFriction = 0f;
        private const float MaxSpeed = 5;
        private const float Gravity = 0.5f;
        
        private float _speed;
        private bool _freeFall;
        private float _lastCost;
        public bool ReachedWater { get; set; }

        private readonly List<Vector3Int> _coords = new List<Vector3Int>();
        private readonly List<Vector3Int> _emptyCoords = new List<Vector3Int>();
        private readonly List<float> _costs = new List<float>();

        readonly TerrianConfig _config;

        public WaterTracker(TerrianConfig config)
        {
            _config = config;
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

        public void FreeFall(Vector3Int coord)
        {
            _speed += Gravity;
            _freeFall = true;
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
            if (_freeFall)
            {
                _speed *= FreeFallFriction;
                _freeFall = false;
            }

            _speed += 0.5f;

            _speed *= Friction;

            _lastCost = (from - to).magnitude / _speed;

            _coords.Add(to);
            _costs.Add(_costs[_costs.Count - 1] + _lastCost);
        }

        public void Apply(Chunks chunks)
        {
            for (var i = 0; i < _coords.Count; i++)
            {
                var coord = _coords[i];
                var cost = _costs[i];
                chunks.SetColor(coord.x, coord.y, coord.z, _config.Biome.WaterColor);
                Terrian.Instance.SetWaterfall(coord, cost);
            }
            foreach (var coord in _emptyCoords)
            {
                chunks.Set(coord, 1);
            }
        }
    }
}