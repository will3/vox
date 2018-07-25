using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public class Chunks
    {
        float sizeF;
        int size;
        public bool useNormals = true;
        public bool isWater = false;
        public string groupName = "chunks";
        GameObject gameObject = new GameObject();

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
        }

        private Dictionary<Vector3Int, Chunk> map = new Dictionary<Vector3Int, Chunk>();

        public Dictionary<Vector3Int, Chunk> Map
        {
            get
            {
                return map;
            }
        }

        public Chunks(int size)
        {
            this.size = size;
            this.sizeF = size;
        }

        private Vector3Int getKey(int i, int j, int k) {
            return new Vector3Int(
                Mathf.FloorToInt(i / this.sizeF),
                Mathf.FloorToInt(j / this.sizeF),
                Mathf.FloorToInt(k / this.sizeF)
            );
        }

        private Vector3Int getOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / this.sizeF) * this.size,
                Mathf.FloorToInt(j / this.sizeF) * this.size,
                Mathf.FloorToInt(k / this.sizeF) * this.size
            );
        }

        public void SetActive(bool active) {
            gameObject.SetActive(active);
        }

        public float Get(Vector3Int coord) {
            return Get(coord.x, coord.y, coord.z);
        }

        public float Get(int i, int j, int k)
        {
            var origin = getOrigin(i, j, k);
            if (!map.ContainsKey(origin))
            {
                return 0;
            }

            return map[origin].Get(i - origin.x, j - origin.y, k - origin.z);
        }

        public float GetLighting(int i, int j, int k)
        {
            var origin = getOrigin(i, j, k);
            if (!map.ContainsKey(origin))
            {
                return 1.0f;
            }

            return map[origin].GetLighting(i - origin.x, j - origin.y, k - origin.z);
        }

        public Color GetColor(int i, int j, int k)
        {
            var origin = getOrigin(i, j, k);
            if (!map.ContainsKey(origin))
            {
                return default(Color);
            }
            return map[origin].GetColor(i - origin.x, j - origin.y, k - origin.z);
        }

        public Chunk GetOrCreateChunk(Vector3Int origin)
        {
            if (map.ContainsKey(origin))
            {
                return map[origin];
            }
            map[origin] = new Chunk(size, origin);
            map[origin].Chunks = this;
            return map[origin];
        }

        public Chunk GetChunk(Vector3Int origin)
        {
            Chunk chunk = null;
            map.TryGetValue(origin, out chunk);
            return chunk;
        }

        public bool HasChunk(Vector3Int origin)
        {
            return map.ContainsKey(origin);
        }

        private List<Vector3Int> GetKeys(int i, int j, int k) {
            var key = getKey(i, j, k);
            var list = new List<Vector3Int>();

            var origin = key * size;
            var ri = i - origin.x;
            var rj = j - origin.y;
            var rk = k - origin.z;

            var iList = new List<int>();
            iList.Add(0);
            var jList = new List<int>();
            jList.Add(0);
            var kList = new List<int>();
            kList.Add(0);

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

            if (ri >= size) {
                iList.Add(1);
            }

            if (rj >= size)
            {
                jList.Add(1);
            }

            if (rk >= size) {
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
            //var origin = getOrigin(i, j, k);
            //var chunk = GetOrCreateChunk(origin);
            //chunk.Set(i - origin.x, j - origin.y, k - origin.z, v);

            var keys = GetKeys(i, j, k);
            foreach(var key in keys) {
                var origin = key * size;
                var chunk = GetOrCreateChunk(origin);
                chunk.Set(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            //var origin = getOrigin(i, j, k);
            //var chunk = GetOrCreateChunk(origin);
            //chunk.SetColor(i - origin.x, j - origin.y, k - origin.z, v);

            var keys = GetKeys(i, j, k);
            foreach(var key in keys) {
                var origin = key * size;
                var chunk = GetOrCreateChunk(origin);
                chunk.SetColor(i - origin.x, j - origin.y, k - origin.z, v);
            }
        }

        public void SetColor(Vector3Int coord, Color v) {
            SetColor(coord.x, coord.y, coord.z, v);
        }

        public bool GetWaterfall(Vector3Int coord)
        {
            var origin = getOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetChunk(origin);
            if (terrianChunk == null)
            {
                return false;
            }
            return terrianChunk.GetWaterfall(coord - terrianChunk.Origin);
        }

        public void SetWaterfall(Vector3Int coord, float value)
        {
            var i = coord.x;
            var j = coord.y;
            var k = coord.z;

            var keys = GetKeys(i, j, k);
            foreach (var key in keys)
            {
                var origin = key * size;
                var chunk = GetOrCreateChunk(origin);
                chunk.SetWaterfall(i - origin.x, j - origin.y, k - origin.z, value);
            }
        }
    }
}
