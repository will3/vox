using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class ChunkShadowMap
    {
        private Dictionary<Vector3Int, int> results = new Dictionary<Vector3Int, int>();
        private static int lightY = 100;
        private static int minY = -100;

        public bool Get(Vector3Int coord)
        {
            return false;
        }

        public float GetShadow(Chunk chunk, Vector3Int c) {
            var coord = c + chunk.Origin;
            var key = liftVector(coord, 0);
            if (!results.ContainsKey(key))
            {
                return 0.0f;
            }

            var shadow = coord.y < results[key];
            return shadow ? 1.0f : 0.0f;
        }

        public float CalcShadow(Chunk chunk, Vector3Int c, IList<Chunks> chunksList)
        {
            var coord = c + chunk.Origin;
            var key = liftVector(coord, 0);

            if (!results.ContainsKey(key)) {
                var start = liftVector(coord, lightY);

                Vector3Int? result = Raycast4545.Trace(start, chunksList, lightY);
                 
                if (result.HasValue)
                {
                    results[key] = result.Value.y;
                }
                else
                {
                    results[key] = minY;
                }
            } 

            var hit = coord.y >= results[key];

            return hit ? 1.0f : 0.0f;
        }

        public void Clear() {
            results.Clear();    
        }

        private Vector3Int liftVector(Vector3Int coord, int height)
        {
            var diff = height - coord.y;
            return new Vector3Int(coord.x + diff, coord.y + diff, coord.z + diff);
        }

        private Vector2Int flattenVector(Vector3Int coord) {
            var v = liftVector(coord, 0);
            return new Vector2Int(v.x, v.z);
        }
    }
}