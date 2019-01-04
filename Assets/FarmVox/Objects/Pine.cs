using System.Collections.Generic;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;
using Tree = FarmVox.Terrain.Tree;

namespace FarmVox.Objects
{
    public class Pine
    {
        readonly float r;
        readonly float h;
        readonly Vector3Int offset;
        readonly int trunkHeight;

        public Pine(float r, float h, int trunkHeight)
        {
            this.r = r;
            this.h = h;
            this.trunkHeight = trunkHeight;

            int radius = Mathf.CeilToInt(r);
            offset = new Vector3Int(-radius, 0, -radius);
        }

        public Tree Place(Terrain.Terrian terrian, Chunks layer, Vector3Int position, TreeConfig config)
        {
            int radius = Mathf.CeilToInt(r);
            int mid = radius + 1;
            var width = radius * 2 + 1;
            var height = Mathf.CeilToInt(h) + trunkHeight;

            var treeCoords = new HashSet<Vector3Int>();
            var stumpCoords = new HashSet<Vector3Int>();

            for (var j = 0; j < height; j++)
            {
                var currentR = r * (1 - (j - trunkHeight) / h);

                for (var i = 0; i < width; i++)
                {
                    for (var k = 0; k < width; k++)
                    {
                        var coord = new Vector3Int(i, j, k) + position + offset;
                        if (j < trunkHeight + 2 && i == mid && k == mid)
                        {
                            layer.Set(coord, 1);
                            layer.SetColor(coord, config.TrunkColor);

                            //if (j == 1) {
                            //    var stump1 = coord + new Vector3Int(0, 0, 1);
                            //    var stump2 = coord + new Vector3Int(0, 0, -1);
                            //    var stump3 = coord + new Vector3Int(1, 0, 0);
                            //    var stump4 = coord + new Vector3Int(-1, 0, 0);

                            //    layer.Set(stump1, 1);
                            //    layer.SetColor(stump1, Colors.trunk);

                            //    layer.Set(stump2, 1);
                            //    layer.SetColor(stump2, Colors.trunk);

                            //    layer.Set(stump3, 1);
                            //    layer.SetColor(stump3, Colors.trunk);

                            //    layer.Set(stump4, 1);
                            //    layer.SetColor(stump4, Colors.trunk);
                            //}

                            if (j == 0) {
                                stumpCoords.Add(coord);
                            } else {
                                treeCoords.Add(coord);    
                            }
                        } else if (j >= trunkHeight) {
                            float diffI = Mathf.Abs(mid - i);
                            float diffK = Mathf.Abs(mid - k);
                            float distance = Mathf.Sqrt(diffI * diffI + diffK * diffK);
                            float density = currentR - distance;

                            if (j == height - 1)
                            {
                                density = 0f;
                            }

                            if (density > 0)
                            {
                                var value = density - (float)config.TreeRandom.NextDouble() * 1.0f;
                                layer.Set(coord, value);
                                layer.SetColor(coord, config.LeafColor);
                                treeCoords.Add(coord);
                            }
                        }
                    }
                }
            }

            var tree = new Tree(stumpCoords, treeCoords, position);
            return tree;
        }
    }
}