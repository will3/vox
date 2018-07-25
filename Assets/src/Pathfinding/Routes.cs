using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Priority_Queue;

namespace FarmVox
{
    public class Routes
    {
        Vector3Int origin;
        Terrian terrian;
        int size;
        public RoutesMap routesMap;
        public int resolution = 2;

        public Routes(Vector3Int origin, Terrian terrian)
        {
            this.terrian = terrian;
            this.size = terrian.Size;
            this.origin = origin;
            var halfSize = this.size / resolution;
        }

        Mutex editingMutex = new Mutex();

        public void LoadChunk(Chunk chunk, TerrianConfig config)
        {
            editingMutex.WaitOne();

            LoadConnections(chunk, config);

            editingMutex.ReleaseMutex();
        }

        public Vector3Int? GetExistingNode(Vector3 vector) {
            var rf = (float)resolution;
            var node = new Vector3Int(
                Mathf.FloorToInt(vector.x / rf) * resolution,
                Mathf.FloorToInt(vector.y / rf) * resolution,
                Mathf.FloorToInt(vector.z / rf) * resolution);

            if (HasNode(node)) {
                return node;    
            }
            return null;
        }

        private bool HasNode(Vector3Int node) {
            var o = routesMap.GetOrigin(node.x, node.y, node.z);
            if (o != this.origin) {
                var routes = routesMap.GetRoutes(o);    
                if (routes == null) {
                    return false;
                }
                return routes.HasNode(node);
            } else {
                return nodes.Contains(node);
            }
        }

        public Vector3Int GetNode(Vector3 vector) {
            var rf = (float)resolution;
            return new Vector3Int(
                Mathf.FloorToInt(vector.x / rf) * resolution,
                Mathf.FloorToInt(vector.y / rf) * resolution,
                Mathf.FloorToInt(vector.z / rf) * resolution);
        }

        public Vector3Int? SteppingNode(Vector3Int node) {
            var up = new Vector3Int(node.x, node.y + resolution, node.z);
            if (HasNode(up)) {
                return up;
            }

            if (HasNode(node)) {
                return node;
            }

            var down = new Vector3Int(node.x, node.y - resolution, node.z);
            if (HasNode(down)) {
                return down;
            }

            return null;
        }

        public Vector3? Drag(Vector3 now, Vector3 to) {
            var nodeNullable = GetExistingNode(now);
            if (nodeNullable == null) {
                Debug.Log("invalid position");
                return null;
            }
            var node = nodeNullable.Value;

            var diff = (to - now);
            diff.y = 0;

            var maxMag = 0.4f;
            if (diff.magnitude > maxMag) {
                diff = diff.normalized * maxMag;
            }

            var projected = now + diff;

            if (diff.x > 0) {
                var nextNode = SteppingNode(node + new Vector3Int(resolution, 0, 0));
                if (nextNode == null) {
                    if (projected.x > node.x + 1) {
                        projected.x = node.x + 1;
                    }
                }
            }

            if (diff.x < 0) {
                var nextNode = SteppingNode(node + new Vector3Int(-resolution, 0, 0));
                if (nextNode == null) {
                    if (projected.x < node.x + 1) {
                        projected.x = node.x + 1;
                    }
                }
            }

            if (diff.z > 0)
            {
                var nextNode = SteppingNode(node + new Vector3Int(0, 0, resolution));
                if (nextNode == null)
                {
                    if (projected.z > node.z + 1)
                    {
                        projected.z = node.z + 1;
                    }
                }
            }

            if (diff.z < 0)
            {
                var nextNode = SteppingNode(node + new Vector3Int(0, 0, -resolution));
                if (nextNode == null)
                {
                    if (projected.z < node.z + 1)
                    {
                        projected.z = node.z + 1;
                    }
                }
            }

            ;
            var projectedNode = GetNode(projected);
            var projectedSteppingNode = SteppingNode(projectedNode);

            if (projectedSteppingNode == null) {
                // Huuhhhh?
                return null;
            }

            projected.y = projectedSteppingNode.Value.y;
            return projected;
        }

        public void DrawGizmos()
        {
            editingMutex.WaitOne();

            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);

            foreach(var coord in nodes) {
                var position = coord + new Vector3(1, 1, 1);
                Gizmos.DrawCube(position, new Vector3(1.0f, 1.0f, 1.0f));
            }

            editingMutex.ReleaseMutex();
        }

        readonly HashSet<Vector3Int> nodes = new HashSet<Vector3Int>();
        readonly HashSet<Vector3Int> coords = new HashSet<Vector3Int>();

        void LoadConnections(Chunk chunk, TerrianConfig config)
        {
            nodes.Clear();

            var rf = (float)resolution;

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.surfaceCoordsUp) {
                var node = new Vector3Int(
                    Mathf.FloorToInt(coord.x / rf) * resolution,
                    (Mathf.FloorToInt(coord.y / rf) + 1) * resolution,
                    Mathf.FloorToInt(coord.z / rf) * resolution) + origin;
                nodes.Add(node);
                coords.Add(coord);
            }
        }

        public HashSet<Vector3Int> GetConnections(Vector3Int node) {
            var set = new HashSet<Vector3Int>();
            for (var i = -1; i <= 1; i++) {
                for (var j = -1; j <= 1; j++) {
                    for (var k = -1; k <= 1; k++)
                    { 
                        if (i == 0 && k == 0 && (j == -1 || j == 1)) {
                            continue;
                        }

                        var next = node + new Vector3Int(i, j, k) * resolution;
                        if (HasNode(next)) {
                            set.Add(next);
                        }
                    }    
                }
            }
            return set;
        }
    }
}