using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class HeightMap
    {
        int size;
        public HeightMap(int size) {
            this.size = size;
        }

        Dictionary<Vector2Int, Dictionary<Vector2Int, int>> map = 
            new Dictionary<Vector2Int, Dictionary<Vector2Int, int>>();

        public void LoadColumn(TerrianColumn column) {
            float startY = 200;
            var origin = Vectors.GetXZ(column.Origin);
            if (!map.ContainsKey(origin)) {
                map[origin] = new Dictionary<Vector2Int, int>();
            }
            var m = map[origin];

            for (var i = 0; i < size; i ++) {
                for (var k = 0; k < size; k ++) {
                    var coord = new Vector2Int(i + column.Origin.x, 
                                               k + column.Origin.z);
                    var pos = new Vector3(i + column.Origin.x, startY, 
                                          k + column.Origin.z);
                    var ray = new Ray(pos, Vector3.down);

                    var result = VoxelRaycast.TraceRay(ray, 1 << UserLayers.terrian);

                    if (result == null) {
                        m[coord] = 999;
                    } else {
                        var y = result.GetCoord().y;
                        m[coord] = y;
                    }
                }
            }
        }
    }
}