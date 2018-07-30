using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public interface IChunkColorProvider
    {
        Color GetColor(int i, int j, int k);
    }

    public class Chunk
    {
        readonly int size;
        readonly Vector3Int origin;
        public Chunks Chunks;

        float[] data;
        Color[] colors;

        bool dirty;
        public bool surfaceCoordsDirty = true;
        bool normalsNeedsUpdate = true;

        GameObject gameObject;
        Mesh mesh;

        public HashSet<Vector3Int> surfaceCoords = new HashSet<Vector3Int>();
        public HashSet<Vector3Int> surfaceCoordsUp = new HashSet<Vector3Int>();
        readonly Dictionary<Vector3Int, float> lightNormals = new Dictionary<Vector3Int, float>();

        public readonly int dataSize;

        public Color[] Colors
        {
            get
            {
                return colors;
            }
        }

        public void SetColors(Color[] colors)
        {
            if (colors.Length != this.colors.Length)
            {
                throw new System.ArgumentException("invalid length");
            }
            this.colors = colors;
            dirty = true;
        }

        public void SetData(float[] data)
        {
            if (data.Length != this.data.Length)
            {
                throw new System.ArgumentException("invalid length");
            }
            this.data = data;
            dirty = true;
            surfaceCoordsDirty = true;
            normalsNeedsUpdate = true;
        }

        private readonly Dictionary<Vector3Int, float> waterfalls = new Dictionary<Vector3Int, float>();

        public Dictionary<Vector3Int, float> Waterfalls
        {
            get
            {
                return waterfalls;
            }
        }

        public GameObject GetGameObject() {
            if (gameObject == null) {
                var name = "Chunk" + origin.ToString();
                gameObject = new GameObject(name);
                gameObject.transform.localPosition = origin;
                gameObject.transform.parent = Chunks.GetGameObject().transform;
            }
            return gameObject;
        }

        public MeshRenderer GetMeshRenderer() {
            var meshRenderer = GetGameObject().GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                return meshRenderer;
            }
            return GetGameObject().AddComponent<MeshRenderer>();
        }

        public MeshFilter GetMeshFilter() {
            var meshFilter = GetGameObject().GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                return meshFilter;
            }
            return GetGameObject().AddComponent<MeshFilter>();
        }

        public MeshCollider GetMeshCollider() {
            var meshCollider = GetGameObject().GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                return meshCollider;
            }
            return GetGameObject().AddComponent<MeshCollider>();
        }

        public void UpdateSurfaceCoords()
        {
            if (!surfaceCoordsDirty)
            {
                return;
            }

            surfaceCoords.Clear();
            surfaceCoordsUp.Clear();

            for (var d = 0; d < 3; d++)
            {
                for (var i = 0; i < dataSize - 1; i++)
                {

                    for (var j = 0; j < dataSize - 1; j++)
                    {

                        for (var k = 0; k < dataSize - 1; k++)
                        {

                            var coordA = Vectors.GetVector3Int(i, j, k, d);
                            var coordB = Vectors.GetVector3Int(i + 1, j, k, d);
                            var a = Get(coordA.x, coordA.y, coordA.z);
                            var b = Get(coordB.x, coordB.y, coordB.z);

                            if (a > 0 == b > 0)
                            {
                                continue;
                            }

                            if (a > 0)
                            {
                                surfaceCoords.Add(coordA);
                                if (d == 1)
                                {
                                    surfaceCoordsUp.Add(coordA);
                                }
                            }
                            else
                            {
                                surfaceCoords.Add(coordB);
                            }
                        }
                    }
                }
            }

            surfaceCoordsDirty = false;
        }

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        public float[] Data
        {
            get
            {
                return data;
            }
        }

        public int Size { get { return size; } }

        public Mesh Mesh
        {
            get
            {
                return mesh;
            }

            set
            {
                mesh = value;
            }
        }

        public bool Dirty
        {
            get
            {
                return dirty;
            }

            set
            {
                dirty = value;
            }
        }

        public Chunk(int size, Vector3Int origin)
        {
            this.size = size;
            this.origin = origin;
            dataSize = size + 3;
            data = new float[dataSize * dataSize * dataSize];
            colors = new Color[dataSize * dataSize * dataSize];
        }

        public float Get(int i, int j, int k)
        {
            if (i < 0 || i >= dataSize ||
                j < 0 || j >= dataSize ||
                k < 0 || k >= dataSize)
            {
                throw new System.Exception("out of bounds:" + new Vector3Int(i, j, k).ToString());
            }
            var index = GetIndex(i, j, k);
            return data[index];
        }

        public void Set(int i, int j, int k, float v)
        {
            if (i < 0 || i >= dataSize ||
                j < 0 || j >= dataSize ||
                k < 0 || k >= dataSize)
            {
                throw new System.Exception("out of bounds:" + new Vector3Int(i, j, k).ToString());
            }
            var index = GetIndex(i, j, k);
            data[index] = v;
            dirty = true;
            surfaceCoordsDirty = true;
            normalsNeedsUpdate = true;
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var index = GetIndex(i, j, k);
            colors[index] = v;
            dirty = true;
        }

        public void SetGlobal(int i, int j, int k, float v)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                Chunks.Set(i + origin.x, j + origin.y, k + origin.z, v);
            }
            else
            {
                Set(i, j, k, v);
            }
        }

        public void SetColorGlobal(int i, int j, int k, Color color)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                Chunks.SetColor(i + origin.x, j + origin.y, k + origin.z, color);
            }
            else
            {
                SetColor(i, j, k, color);
            }
        }



        public Color GetColor(int i, int j, int k)
        {
            var index = GetIndex(i, j, k);
            return colors[index];
        }

        public Color GetColorGlobal(int i, int j, int k)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return Chunks.GetColor(i + origin.x, j + origin.y, k + origin.z);
            }
            else
            {
                return GetColor(i, j, k);
            }
        }

        public float GetGlobal(int i, int j, int k)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return Chunks.Get(i + origin.x, j + origin.y, k + origin.z);
            }
            else
            {
                return Get(i, j, k);
            }
        }

        public int GetIndex(Vector3Int coord)
        {
            return GetIndex(coord.x, coord.y, coord.z);
        }

        public int GetIndex(int i, int j, int k)
        {
            int index = i * dataSize * dataSize + j * dataSize + k;
            return index;
        }

        public void UpdateNormals()
        {
            if (!normalsNeedsUpdate)
            {
                return;
            }

            UpdateSurfaceCoords();

            var lightDir = Raycast4545.LightDir;

            foreach (var coord in surfaceCoords)
            {
                if (coord.x >= dataSize - 1 || coord.y >= dataSize - 1 || coord.z >= dataSize - 1)
                {
                    continue;
                }
                else
                {
                    var normal = CalcNormal(coord);
                    normals[coord] = normal;
                    lightNormals[coord] = Vector3.Dot(lightDir, normal);
                }
            }

            normalsNeedsUpdate = false;
        }

        private Vector3 CalcNormal(Vector3Int coord)
        {
            var n = new Vector3(-1, -1, -1) * Get(new Vector3Int(coord.x, coord.y, coord.z)) +
                new Vector3(1, -1, -1) * Get(new Vector3Int(coord.x + 1, coord.y, coord.z)) +
                new Vector3(-1, 1, -1) * Get(new Vector3Int(coord.x, coord.y + 1, coord.z)) +
                new Vector3(1, 1, -1) * Get(new Vector3Int(coord.x + 1, coord.y + 1, coord.z)) +

                new Vector3(-1, -1, 1) * Get(new Vector3Int(coord.x, coord.y, coord.z + 1)) +
                new Vector3(1, -1, 1) * Get(new Vector3Int(coord.x + 1, coord.y, coord.z + 1)) +
                new Vector3(1, 1, 1) * Get(new Vector3Int(coord.x, coord.y + 1, coord.z + 1)) +
                new Vector3(1, 1, 1) * Get(new Vector3Int(coord.x + 1, coord.y + 1, coord.z + 1));

            return n.normalized * -1;
        }

        public float? GetLightNormal(Vector3Int coord)
        {
            if (lightNormals.ContainsKey(coord))
            {
                return lightNormals[coord];
            }
            return null;
        }

        public Vector3? GetNormal(Vector3Int coord)
        {
            if (normals.ContainsKey(coord))
            {
                return normals[coord];
            }
            return null;
        }

        private readonly Dictionary<Vector3Int, Vector3> normals = new Dictionary<Vector3Int, Vector3>();

        public Dictionary<Vector3Int, Vector3> Normals
        {
            get
            {
                return normals;
            }
        }

        public float Get(Vector3Int coord)
        {
            return Get(coord.x, coord.y, coord.z);
        }

        public void SetWaterfall(Vector3Int coord, float v)
        {
            SetWaterfall(coord.x, coord.y, coord.z, v);
        }

        public void SetWaterfall(int i, int j, int k, float v)
        {
            var coord = new Vector3Int(i, j, k);
            waterfalls[coord] = v;
        }

        public bool GetWaterfall(Vector3Int coord)
        {
            return GetWaterfall(coord.x, coord.y, coord.z);
        }

        public bool GetWaterfall(int i, int j, int k)
        {
            var coord = new Vector3Int(i, j, k);
            return waterfalls.ContainsKey(coord);
        }

        public bool IsSurfaceCoord(Vector3Int coord) {
            return surfaceCoords.Contains(coord);
        }
    }
}