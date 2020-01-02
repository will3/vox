using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;
using Tree = FarmVox.Terrain.Tree;

namespace FarmVox.Objects
{
    public class PineObject
    {
        private readonly float _r;
        private readonly float _h;
        private readonly Vector3Int _pivot;
        private readonly int _trunkHeight;
        
        public PineObject(float r, float h, int trunkHeight)
        {
            _r = r;
            _h = h;
            _trunkHeight = trunkHeight;

            var radius = Mathf.CeilToInt(r);
            _pivot = new Vector3Int(-radius, 0, -radius);
        }

        public Tree Place(Chunks layer, Vector3Int offset, TreeConfig config)
        {
            var radius = Mathf.CeilToInt(_r);
            var mid = radius + 1;
            var width = radius * 2 + 1;
            var height = Mathf.CeilToInt(_h) + _trunkHeight;

            for (var j = 0; j < height; j++)
            {
                var currentR = _r * (1 - (j - _trunkHeight) / _h);

                for (var i = 0; i < width; i++)
                {
                    for (var k = 0; k < width; k++)
                    {
                        var coord = new Vector3Int(i, j, k) + offset + _pivot;
                        if (j < _trunkHeight + 2 && i == mid && k == mid)
                        {
                            layer.Set(coord, 1);
                            layer.SetColor(coord, config.trunkColor);
                        } else if (j >= _trunkHeight) {
                            var diffI = Mathf.Abs(mid - i);
                            var diffK = Mathf.Abs(mid - k);
                            var distance = Mathf.Sqrt(diffI * diffI + diffK * diffK);
                            var density = currentR - distance;

                            if (j == height - 1)
                            {
                                density = 0f;
                            }

                            if (!(density > 0)) continue;
                            var value = density - (float)config.random.NextDouble() * 1.0f;
                            layer.Set(coord, value);
                            layer.SetColor(coord, config.leafColor);
                        }
                    }
                }
            }

            var tree = new Tree(offset);
            return tree;
        }
    }
}