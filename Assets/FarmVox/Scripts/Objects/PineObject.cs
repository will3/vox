using System.Collections.Generic;
using FarmVox.Scripts.Voxel;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Scripts.Objects
{
    public class PineObject
    {
        private readonly float _r;
        private readonly float _h;
        private readonly int _trunkHeight;
        private readonly TreeConfig _config;
        private readonly Dictionary<Vector3Int, float> _cache = new Dictionary<Vector3Int, float>();

        public PineObject(float r, float h, int trunkHeight, TreeConfig config)
        {
            _r = r;
            _h = h;
            _trunkHeight = trunkHeight;
            _config = config;
        }

        public void Place(Chunks layer, Vector3Int position)
        {
            var radius = Mathf.CeilToInt(_r);
            var height = Mathf.CeilToInt(_h) + _trunkHeight;

            for (var i = -radius; i <= radius; i++)
            {
                for (var k = 0 - radius; k <= radius; k++)
                {
                    for (var j = 0; j < height; j++)
                    {
                        var localCoord = new Vector3Int(i, j, k);
                        var v = GetValue(localCoord);
                        if (!(v > 0)) continue;

                        var worldCoord = localCoord + position;
                        var color = GetColor(localCoord);
                        layer.Set(worldCoord, v);
                        layer.SetColor(worldCoord, color);
                        layer.SetNormal(worldCoord, GetNormal(localCoord));
                    }
                }
            }
        }

        private float GetCone(Vector3 localCoord)
        {
            var radius = _r * (1 - (localCoord.y - _trunkHeight) / _h);

            var dis = Mathf.Sqrt(localCoord.x * localCoord.x + localCoord.z * localCoord.z);
            var density = radius - dis;
            density -= (float) _config.random.NextDouble() * _config.randomAmount;

            return density;
        }

        private Color GetColor(Vector3Int localCoord)
        {
            return localCoord.y < _trunkHeight
                ? _config.trunkColor
                : _config.leafColor;
        }

        private float GetValue(Vector3Int localCoord)
        {
            if (_cache.TryGetValue(localCoord, out var v))
            {
                return v;
            }

            v = CalcValue(localCoord);
            _cache[localCoord] = v;

            return v;
        }

        private Vector3 GetNormal(Vector3Int localCoord)
        {
            var x = GetValue(localCoord + new Vector3Int(1, 0, 0)) - GetValue(localCoord - new Vector3Int(1, 0, 0));
            var y = GetValue(localCoord + new Vector3Int(0, 1, 0)) - GetValue(localCoord - new Vector3Int(0, 1, 0));
            var z = GetValue(localCoord + new Vector3Int(0, 0, 1)) - GetValue(localCoord - new Vector3Int(0, 0, 1));

            return new Vector3(x, y, z).normalized * -1;
        }

        private float CalcValue(Vector3Int localCoord)
        {
            if (localCoord.y >= _trunkHeight)
            {
                return GetCone(localCoord);
            }

            if (localCoord.x == 0 && localCoord.z == 0)
            {
                return 1.0f;
            }

            return 0.0f;
        }
    }
}