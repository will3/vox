using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Priority_Queue;

namespace FarmVox
{
    class RouteNavMesh {
                   
    }

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

        public void LoadChunk(Chunk chunk, TerrianChunk terrianChunk, TerrianConfig config)
        {
            editingMutex.WaitOne();

            nodes.Clear();

            var rf = (float)resolution;

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.surfaceCoordsUp)
            {
                var node = new Vector3Int(
                    Mathf.FloorToInt(coord.x / rf) * resolution,
                    (Mathf.FloorToInt(coord.y / rf)) * resolution,
                    Mathf.FloorToInt(coord.z / rf) * resolution) + origin;
                
                nodes.Add(node);
                if (!coordsByNodes.ContainsKey(node)) {
                    coordsByNodes[node] = new HashSet<Vector3Int>();
                }

                coordsByNodes[node].Add(coord + origin);
            }

            editingMutex.ReleaseMutex();
        }

        public void DrawGizmos()
        {
            editingMutex.WaitOne();

            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);

            foreach(var coord in nodes) {
                var position = coord + new Vector3(1, 1, 1);
                position.y += resolution;
                Gizmos.DrawCube(position, new Vector3(1.0f, 1.0f, 1.0f));
            }

            editingMutex.ReleaseMutex();
        }

        readonly HashSet<Vector3Int> nodes = new HashSet<Vector3Int>();
        readonly Dictionary<Vector3Int, HashSet<Vector3Int>> coordsByNodes = new Dictionary<Vector3Int, HashSet<Vector3Int>>();

        public bool HasNode(Vector3Int node) {
            return nodes.Contains(node);
        }

        public HashSet<Vector3Int> GetCoordsInNode(Vector3Int node) {
            if (!nodes.Contains(node)) {
                return new HashSet<Vector3Int>();
            }
            return coordsByNodes[node];
        }
    }
}