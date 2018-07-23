using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{

    public class RoutesMap
    {
        Terrian terrian;

        public RoutesMap(Terrian terrian) {
            this.terrian = terrian;    
        }

        private Dictionary<Vector3Int, Routes> map = new Dictionary<Vector3Int, Routes>();

        public Routes GetOrCreateRoutes(Vector3Int origin) {
            if (!map.ContainsKey(origin)) {
                map[origin] = new Routes(origin, terrian);
                map[origin].routesMap = this;
            }

            return map[origin];
        }

        public Routes GetRoutes(Vector3Int origin) {
            if (map.ContainsKey(origin)) {
                return map[origin];
            }
            return null;
        }

        public Vector3Int GetOrigin(Vector3 coord)
        {
            return terrian.GetOrigin(coord.x, coord.y, coord.z);
        }

        public Vector3Int GetOrigin(float i, float j, float k) {
            return terrian.GetOrigin(i, j, k);
        }

        public void DrawGizmos() {
            foreach(var kv in map) {
                kv.Value.DrawGizmos();
            }
        }
    }
}