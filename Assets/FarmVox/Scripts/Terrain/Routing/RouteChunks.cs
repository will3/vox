using System;
using System.Collections.Generic;
using FarmVox.Voxel;
using JetBrains.Annotations;
using UnityEngine;

namespace FarmVox.Terrain.Routing
{
    public class RouteChunks
    {
        private readonly int _size;
        private readonly Dictionary<Vector3Int, RouteChunk> _map = new Dictionary<Vector3Int, RouteChunk>();

        public RouteChunks(int size)
        {
            _size = size;
        }

        private RouteChunk GetChunk(Vector3Int origin)
        {
            RouteChunk chunk;
            _map.TryGetValue(origin, out chunk);
            return chunk;
        }
        
        private RouteChunk GetOrCreateChunk(Vector3Int origin)
        {
            RouteChunk chunk;
            _map.TryGetValue(origin, out chunk);
            if (chunk != null)
            {
                return chunk;
            }
            
            chunk = new RouteChunk(_size, origin);

            _map[origin] = chunk;
            
            return chunk;
        }

        public void LoadChunk([NotNull] Chunk chunk)
        {
            if (chunk == null) throw new ArgumentNullException("chunk");
            var routeChunk = GetOrCreateChunk(chunk.origin);
            routeChunk.LoadChunk(chunk);
        }
        
        public Vector3Int? FindClosestLandingSpot(Vector3Int target, int searchDistance)
        {
            if (searchDistance == 0)
            {
                throw new ArgumentException("searchDistance cannot be 0");
            }

            var origin = GetOrigin(target.x, target.y, target.z);
            var chunk = GetChunk(origin);

            if (chunk == null)
            {
                return null;
            }
            
            var coords = chunk.Connections.Keys;

            var minDistance = Mathf.Infinity;
            Vector3Int? minCoord = null;
            
            foreach (var coord in coords)
            {
                var distance = GetDistance(target, coord);
                if (distance > searchDistance)
                {
                    continue;
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minCoord = coord;
                }
            }

            return minCoord;
        }

        private int GetDistance(Vector3Int a, Vector3Int b)
        {
            return Math.Abs(a.x - b.x) +
                   Math.Abs(a.y - b.y) +
                   Math.Abs(a.z - b.z);
        }
        
        private Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float)_size) * _size,
                Mathf.FloorToInt(j / (float)_size) * _size,
                Mathf.FloorToInt(k / (float)_size) * _size
            );
        }
    }
}