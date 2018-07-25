using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class ChunkShadowMap
    {
        Dictionary<Vector3Int, int> results = new Dictionary<Vector3Int, int>();
        static int lightY = 100;
        static int minY = -100;

        public bool Get(Vector3Int coord)
        {
            return false;
        }

        public float GetShadow(Chunk chunk, Vector3Int c)
        {
            var coord = c + chunk.Origin;
            var key = liftVector(coord, 0);
            if (!results.ContainsKey(key))
            {
                return 0.0f;
            }

            var shadow = coord.y < results[key];
            return shadow ? 1.0f : 0.0f;
        }

        public void CalcShadow(Chunk chunk, Vector3Int c, IList<Chunks> chunksList)
        {
            var coord = c + chunk.Origin;
            var key = liftVector(coord, 0);

            if (!results.ContainsKey(key))
            {
                var start = liftVector(coord, lightY) + new Vector3(0.5f, 0, 0.5f);
                var ray = new Ray(start, new Vector3(-1, -1, -1));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    results[key] = GetCoord(hit).y;
                }
                else
                {
                    results[key] = minY;
                }
            }
        }

        public void Clear()
        {
            results.Clear();
        }

        private Vector3Int liftVector(Vector3Int coord, int height)
        {
            var diff = height - coord.y;
            return new Vector3Int(coord.x + diff, coord.y + diff, coord.z + diff);
        }

        private Vector2Int flattenVector(Vector3Int coord)
        {
            var v = liftVector(coord, 0);
            return new Vector2Int(v.x, v.z);
        }

        public Vector3Int GetCoord(RaycastHit hit)
        {
            var point = hit.point - hit.normal * 0.5f;
            return new Vector3Int(Mathf.FloorToInt(point.x),
                                  Mathf.FloorToInt(point.y),
                                  Mathf.FloorToInt(point.z));
        }
    }
}