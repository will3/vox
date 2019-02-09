using System;
using System.Collections.Generic;
using System.Linq;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Terrain.Routing
{
    public class RouteChunk
    {
        private readonly int _size;
        private readonly Vector3Int _origin;

        private readonly Dictionary<Vector3Int, HashSet<Vector3Int>> _connections =
            new Dictionary<Vector3Int, HashSet<Vector3Int>>();

        public Dictionary<Vector3Int, HashSet<Vector3Int>> Connections
        {
            get { return _connections; }
        }

        public RouteChunk(int size, Vector3Int origin)
        {
            _size = size;
            _origin = origin;
        }

        public void LoadChunk(Chunk chunk)
        {
            if (chunk.Origin != _origin)
            {
                var message = string.Format("origin must match, expected {0}, but got {1}", _origin, chunk.Origin);
                throw new InvalidOperationException(message);    
            }
            
            chunk.UpdateSurfaceCoords();
            
            _connections.Clear();

            foreach (var coord in chunk.SurfaceCoordsUp)
            {
                var worldCoord = coord + _origin;

                _connections[worldCoord] = new HashSet<Vector3Int>();
            }

            var coords = _connections.Keys;

            foreach (var kv in _connections)
            {
                var coord = kv.Key;

                var coordsAround = GetCoordsAround(coord);

                foreach (var b in coordsAround)
                {
                    if (coords.Contains(b))
                    {
                        kv.Value.Add(b);
                    }   
                }
            }
        }

        private static IEnumerable<Vector3Int> GetCoordsAround(Vector3Int coord)
        {
            var set = new HashSet<Vector3Int>();

            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    for (var k = -1; k <= 1; k++)
                    {
                        if (i == 0 && k == 0)
                        {
                            continue;
                        }

                        set.Add(coord + new Vector3Int(i, j, k));
                    }
                }
            }

            return set;
        }
    }
}