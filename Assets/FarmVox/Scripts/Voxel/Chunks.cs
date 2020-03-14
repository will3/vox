using System.Collections.Generic;
using FarmVox.Scripts;
using FarmVox.Scripts.Voxel;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class Chunks : MonoBehaviour
    {
        public int size = 32;
        public bool useNormals = true;
        public bool isWater;
        public float normalStrength = 0.4f;
        public int normalBanding = 6;
        public float shadowStrength = 0.5f;
        public bool transparent;
        public bool isWalkable;
        public GameObject chunkPrefab;

        private readonly Dictionary<Vector3Int, Chunk> _map = new Dictionary<Vector3Int, Chunk>();
        private GameObject _root;

        public IEnumerable<Chunk> ChunkList => _map.Values;

        private Vector3Int GetKey(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float) size),
                Mathf.FloorToInt(j / (float) size),
                Mathf.FloorToInt(k / (float) size)
            );
        }

        private Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float) size) * size,
                Mathf.FloorToInt(j / (float) size) * size,
                Mathf.FloorToInt(k / (float) size) * size
            );
        }

        public float Get(Vector3Int coord)
        {
            return Get(coord.x, coord.y, coord.z);
        }

        private float Get(int i, int j, int k)
        {
            var origin = GetOrigin(i, j, k);
            if (!_map.ContainsKey(origin))
            {
                return 0;
            }

            return _map[origin].Get(i - origin.x, j - origin.y, k - origin.z);
        }

        public Color GetColor(Vector3Int coord)
        {
            return GetColor(coord.x, coord.y, coord.z);
        }

        private Color GetColor(int i, int j, int k)
        {
            var origin = GetOrigin(i, j, k);
            return !_map.ContainsKey(origin)
                ? default(Color)
                : _map[origin].GetColor(i - origin.x, j - origin.y, k - origin.z);
        }

        public Chunk GetOrCreateChunk(Vector3Int origin)
        {
            if (_root == null)
            {
                _root = new GameObject("Chunks");
                _root.transform.parent = transform;
            }

            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            var chunkGo = Instantiate(chunkPrefab, _root.transform);

            chunkGo.name = "Chunk" + origin;
            chunkGo.transform.localPosition = origin;
            chunkGo.layer = gameObject.layer;
            var chunk = chunkGo.GetComponent<Chunk>();
            chunk.Chunks = this;
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

        private IEnumerable<Vector3Int> GetKeys(int i, int j, int k)
        {
            var key = GetKey(i, j, k);
            var list = new List<Vector3Int>();

            var origin = key * size;
            var ri = i - origin.x;
            var rj = j - origin.y;
            var rk = k - origin.z;

            var iList = new List<int> {0};
            var jList = new List<int> {0};
            var kList = new List<int> {0};

            if (ri == 0 || ri == 1)
            {
                iList.Add(-1);
            }

            if (rj == 0 || rj == 1)
            {
                jList.Add(-1);
            }

            if (rk == 0 || rk == 1)
            {
                kList.Add(-1);
            }

            if (ri >= size)
            {
                iList.Add(1);
            }

            if (rj >= size)
            {
                jList.Add(1);
            }

            if (rk >= size)
            {
                kList.Add(1);
            }

            foreach (var di in iList)
            {
                foreach (var dj in jList)
                {
                    foreach (var dk in kList)
                    {
                        list.Add(new Vector3Int(key.x + di, key.y + dj, key.z + dk));
                    }
                }
            }

            return list;
        }

        public void Set(Vector3Int coord, float v)
        {
            Set(coord.x, coord.y, coord.z, v);
        }

        private void Set(int i, int j, int k, float v)
        {
            var keys = GetKeys(i, j, k);
            foreach (var key in keys)
            {
                var origin = key * size;
                var chunk = GetOrCreateChunk(origin);
                chunk.Set(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var keys = GetKeys(i, j, k);
            foreach (var key in keys)
            {
                var origin = key * size;
                var chunk = GetOrCreateChunk(origin);
                chunk.SetColor(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(Vector3Int coord, Color v)
        {
            SetColor(coord.x, coord.y, coord.z, v);
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