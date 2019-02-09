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
            var routeChunk = GetOrCreateChunk(chunk.Origin);
            routeChunk.LoadChunk(chunk);
        }
    }
}