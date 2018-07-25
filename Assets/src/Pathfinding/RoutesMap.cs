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

        readonly Dictionary<Vector3Int, Routes> map = new Dictionary<Vector3Int, Routes>();

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

        // TODO
        //void UpdatePhysics()
        //{
        //    for (var i = 0; i < actors.Count; i++)
        //    {
        //        for (var j = i; j < actors.Count; j++)
        //        {
        //            if (i == j)
        //            {
        //                continue;
        //            }

        //            var a = actors[i];
        //            var b = actors[j];
        //            var factor = 1.0f;

        //            var diff = (b.position - a.position);
        //            if (diff.magnitude == 0.0f)
        //            {
        //                diff = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)).normalized * 0.01f;
        //            }

        //            var dis = diff.magnitude;
        //            if (dis < a.radius + b.radius)
        //            {
        //                var dir = diff.normalized;
        //                var force = dir * (a.radius + b.radius - dis) * factor;
        //                b.Drag(b.position + force);
        //                a.Drag(a.position - force);
        //            }
        //        }
        //    }
        //}
    }
}