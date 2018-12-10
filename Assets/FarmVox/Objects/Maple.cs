using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Objects
{
    public class Maple {
        readonly float size;
        readonly int trunkHeight;

        public Maple(float size, int trunkHeight) {
            this.size = size;
            this.trunkHeight = trunkHeight;
        }

        public void Place(Terrain.Terrian terrian, Chunks layer, Vector3Int position, TerrianConfig config) {
            var height = trunkHeight + size;
            var centerX = Mathf.FloorToInt(size / 2);
            var offset = new Vector3Int(centerX - 1, 0, centerX - 1);
            var center = new Vector3(centerX, trunkHeight + size / 2.0f, centerX);
            for (var j = 0; j < height; j++) {
                for (var i = 0; i < size; i++) {
                    for (var k = 0; k < size; k++) {
                        var coord = position + new Vector3Int(i, j, k) - offset;

                        var c = new Vector3(i, j, k);

                        if (j <= trunkHeight) {
                            if (i == centerX && k == centerX) {
                                layer.Set(coord, 1.0f);
                                layer.SetColor(coord, config.Colors.TrunkColor);
                            }
                        } else {
                            var dis = (c - center).magnitude;
                            float density = size / 2.0f - dis;
                            density -= (float)config.TreeRandom.NextDouble() * 1.0f;

                            layer.Set(coord, density);
                            layer.SetColor(coord, config.Colors.LeafColor);
                        }
                    }
                }
            }
        }
    }
}