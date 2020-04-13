using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public class Chunks : MonoBehaviour
    {
        public int size = 32;
        public ChunkOptions chunkOptions;
        public bool isWalkable;
        public GameObject chunkPrefab;

        private readonly Dictionary<Vector3Int, Chunk> _map = new Dictionary<Vector3Int, Chunk>();

        public IEnumerable<Chunk> ChunkList => _map.Values;

        private Vector3Int GetKey(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float) size),
                Mathf.FloorToInt(j / (float) size),
                Mathf.FloorToInt(k / (float) size)
            );
        }

        private Vector3Int GetOrigin(Vector3Int coord)
        {
            var i = coord.x;
            var j = coord.y;
            var k = coord.z;
            return new Vector3Int(
                Mathf.FloorToInt(i / (float) size) * size,
                Mathf.FloorToInt(j / (float) size) * size,
                Mathf.FloorToInt(k / (float) size) * size
            );
        }

        public float Get(Vector3Int coord)
        {
            var origin = GetOrigin(coord);
            if (!_map.ContainsKey(origin))
            {
                return 0;
            }

            return _map[origin].Get(coord - origin);
        }

        public Color GetColor(Vector3Int coord)
        {
            var origin = GetOrigin(coord);
            return !_map.ContainsKey(origin)
                ? default(Color)
                : _map[origin].GetColor(coord - origin);
        }

        public Vector3 GetNormal(Vector3Int worldCoord)
        {
            var origin = GetOrigin(worldCoord);
            return _map.TryGetValue(origin, out var chunk)
                ? chunk.GetNormal(worldCoord - chunk.origin)
                : Vector3.zero;
        }

        public Chunk GetOrCreateChunk(Vector3Int origin)
        {
            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            var chunkGo = Instantiate(chunkPrefab, transform);

            chunkGo.name = "Chunk" + origin;
            chunkGo.transform.localPosition = origin;
            chunkGo.layer = gameObject.layer;
            var chunk = chunkGo.GetComponent<Chunk>();

            chunk.options = chunkOptions;
            chunk.chunks = this;

            chunk.origin = origin;

            if (isWalkable)
            {
                chunkGo.GetComponent<NavMeshSourceTag>().enabled = true;
            }

            _map[origin] = chunk;

            return chunk;
        }

        public Chunk GetChunk(Vector3Int origin)
        {
            return _map.TryGetValue(origin, out var chunk) ? chunk : null;
        }

        public void Set(Vector3Int worldCoord, float v)
        {
            var origin = GetOrigin(worldCoord);
            var chunk = GetOrCreateChunk(origin);
            chunk.Set(worldCoord - origin, v);
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            SetColor(new Vector3Int(i, j, k), v);
        }

        public void SetColor(Vector3Int coord, Color v)
        {
            var origin = GetOrigin(coord);
            var chunk = GetOrCreateChunk(origin);
            chunk.SetColor(coord - origin, v);
        }

        public void SetNormal(Vector3Int worldCoord, Vector3 normal)
        {
            var origin = GetOrigin(worldCoord);
            var chunk = GetOrCreateChunk(origin);
            chunk.SetNormal(worldCoord - origin, normal);
        }

        public void UnloadChunk(Vector3Int origin)
        {
            if (_map.TryGetValue(origin, out var chunk))
            {
                Destroy(chunk.gameObject);
                _map.Remove(origin);
            }
        }
    }
}