using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class Chunks : IDisposable
    {
        GameObject _gameObject;
        
        public readonly int Size;
        public bool UseNormals = true;
        public bool IsWater = false;
        private readonly string _groupName = "chunks";
        public float NormalStrength = 0.0f;

        public bool Transparent;

        public GameObject GetGameObject() {
            if (_gameObject == null) {
                _gameObject = new GameObject(_groupName);
            }
            return _gameObject;
        }

        public readonly Dictionary<Vector3Int, Chunk> Map = new Dictionary<Vector3Int, Chunk>();

        public Chunks(int size)
        {
            Size = size;
        }

        private Vector3Int GetKey(int i, int j, int k) {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float)Size),
                Mathf.FloorToInt(j / (float)Size),
                Mathf.FloorToInt(k / (float)Size)
            );
        }

        private Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / (float)Size) * Size,
                Mathf.FloorToInt(j / (float)Size) * Size,
                Mathf.FloorToInt(k / (float)Size) * Size
            );
        }

        public float Get(Vector3Int coord) {
            return Get(coord.x, coord.y, coord.z);
        }

        public float Get(int i, int j, int k)
        {
            var origin = GetOrigin(i, j, k);
            if (!Map.ContainsKey(origin))
            {
                return 0;
            }

            return Map[origin].Get(i - origin.x, j - origin.y, k - origin.z);
        }

        public Color GetColor(Vector3Int coord) {
            return GetColor(coord.x, coord.y, coord.z);
        }

        public Color GetColor(int i, int j, int k)
        {
            var origin = GetOrigin(i, j, k);
            return !Map.ContainsKey(origin) ? 
                default(Color) : 
                Map[origin].GetColor(i - origin.x, j - origin.y, k - origin.z);
        }

        public Chunk GetOrCreateChunk(Vector3Int origin)
        {
            if (Map.ContainsKey(origin))
            {
                return Map[origin];
            }

            Map[origin] = new Chunk(Size, origin) {Chunks = this};
            return Map[origin];
        }

        public Chunk GetChunk(Vector3Int origin)
        {
            Chunk chunk;
            Map.TryGetValue(origin, out chunk);
            return chunk;
        }

        public bool HasChunk(Vector3Int origin)
        {
            return Map.ContainsKey(origin);
        }

        private IEnumerable<Vector3Int> GetKeys(int i, int j, int k) {
            var key = GetKey(i, j, k);
            var list = new List<Vector3Int>();

            var origin = key * Size;
            var ri = i - origin.x;
            var rj = j - origin.y;
            var rk = k - origin.z;

            var iList = new List<int> {0};
            var jList = new List<int> {0};
            var kList = new List<int> {0};

            if (ri == 0 || ri == 1) {
                iList.Add(-1);
            }

            if (rj == 0 || rj == 1) {
                jList.Add(-1);
            }

            if (rk == 0 || rk == 1)
            {
                kList.Add(-1);
            }

            if (ri >= Size) {
                iList.Add(1);
            }

            if (rj >= Size)
            {
                jList.Add(1);
            }

            if (rk >= Size) {
                kList.Add(1);
            }

            foreach(var di in iList) {
                foreach(var dj in jList) {
                    foreach(var dk in kList) {
                        list.Add(new Vector3Int(key.x + di, key.y + dj, key.z + dk));
                    }
                }
            }

            return list;
        }

        public void Set(Vector3Int coord, float v) {
            Set(coord.x, coord.y, coord.z, v);
        }

        public void Set(int i, int j, int k, float v)
        {
            var keys = GetKeys(i, j, k);
            foreach(var key in keys) {
                var origin = key * Size;
                var chunk = GetOrCreateChunk(origin);
                chunk.Set(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var keys = GetKeys(i, j, k);
            foreach(var key in keys) {
                var origin = key * Size;
                var chunk = GetOrCreateChunk(origin);
                chunk.SetColor(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(Vector3Int coord, Color v) {
            SetColor(coord.x, coord.y, coord.z, v);
        }

        public void Clear()
        {
            foreach (var chunk in Map.Values)
            {
                chunk.Clear();
            }
        }

        public void Dispose()
        {
            foreach (var chunk in Map.Values)
            {
                chunk.Dispose();
            }
        }
    }
}
