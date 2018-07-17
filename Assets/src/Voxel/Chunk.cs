using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public interface IChunkColorProvider {
        Color GetColor(int i, int j, int k);
    }

    public class Chunk
    {
        private readonly float[] data;
        private readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();

        private Mesh mesh;
        private GameObject gameObject;
        private readonly int size;
        private Vector3Int origin;
        private bool dirty;
        public bool Hidden;
        public Chunks Chunks;
        public bool surfaceCoordsDirty = true;
        private bool shadowsDirty = true;
        private ChunkShadowMap shadowMap = new ChunkShadowMap();
        private List<Vector3Int> surfaceCoords = new List<Vector3Int>();
        private List<Vector3Int> surfaceCoordsUp = new List<Vector3Int>();
        private readonly Dictionary<Vector3Int, float> lightNormals = new Dictionary<Vector3Int, float>();
        private bool normalsNeedsUpdate = true;

        public List<Vector3Int> SurfaceCoordsUp
        {
            get
            {
                return surfaceCoordsUp;
            }
        }

        public List<Vector3Int> SurfaceCoords
        {
            get
            {
                return surfaceCoords;
            }
        }

        public void UpdateSurfaceCoords()
        {
            if (!surfaceCoordsDirty)
            {
                return;
            }

            surfaceCoords.Clear();
            surfaceCoordsUp.Clear();

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    for (var k = 0; k < size; k++)
                    {
                        var v = GetGlobal(i, j, k);
                        var left = GetGlobal(i - 1, j, k);
                        var bot = GetGlobal(i, j - 1, k);
                        var back = GetGlobal(i, j, k - 1);
                        var order = v > 0;

                        if (v > 0 != left > 0)
                        {
                            var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i - 1, j, k);
                            if (InBound(coord)) {
                                surfaceCoords.Add(coord);    
                            }
                        }

                        if (v > 0 != bot > 0)
                        {
                            var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j - 1, k);
                            if (InBound(coord)) {
                                surfaceCoords.Add(coord);    
                            }
                            if (bot > 0) {
                                surfaceCoordsUp.Add(new Vector3Int(i, j - 1, k));
                            }
                        }

                        if (v > 0 != back > 0)
                        {
                            var coord = v > 0 ? new Vector3Int(i, j, k) : new Vector3Int(i, j, k - 1);
                            if (InBound(coord)) {
                                surfaceCoords.Add(coord);    
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

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }

            set
            {
                gameObject = value;
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
            int dataSize = size + 2;
            data = new float[dataSize * dataSize * dataSize];
        }

        private int getDataIndex(int i, int j, int k) {
            return (i + 1) * size * size + (j + 1) * size + k + 1;
        }

        public float Get(int i, int j, int k)
        {
            var index = getDataIndex(i, j, k);
            return data[index];
        }

        public void Set(int i, int j, int k, float v)
        {
            var index = getDataIndex(i, j, k);
            data[index] = v;
            dirty = true;
            surfaceCoordsDirty = true;
            shadowsDirty = true;
            normalsNeedsUpdate = true;
        }

        public void SetIfHigher(int i, int j, int k, float v)
        {
            var index = getDataIndex(i, j, k);
            if (data[index] < v)
            {
                data[index] = v;
            }
            dirty = true;
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var index = getIndex(i, j, k);
            colors[index] = v;
            dirty = true;
        }

        public void SetIfHigherGlobal(int i, int j, int k, float v)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                Chunks.SetIfHigher(i + origin.x, j + origin.y, k + origin.z, v);
            }
            else
            {
                SetIfHigher(i, j, k, v);
            }
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

        public float GetLightingGlobal(int i, int j, int k)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return Chunks.GetLighting(i + origin.x, j + origin.y, k + origin.z);
            }
            else
            {
                return GetLighting(i, j, k);
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

        public float GetGlobal(Vector3Int coord)
        {
            return GetGlobal(coord.x, coord.y, coord.z);
        }

        public Color GetColor(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            Color color;
            colors.TryGetValue(index, out color);
            return color;
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

        private int getIndex(int i, int j, int k)
        {
            int index = i * size * size + j * size + k;
            return index;
        }

        private bool InBound(Vector3Int coord) {
            return InBound(coord.x, coord.y, coord.z);
        }

        public bool InBound(int i, int j, int k)
        {
            int max = size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return false;
            }
            return true;
        }

        public float GetLighting(int i, int j, int k)
        {
            Vector3Int coord = new Vector3Int(i, j, k);
            return GetLighting(coord);
        }

        public void UpdateShadows(IList<Chunks> chunksList)
        {
            if (!shadowsDirty)
            {
                return;
            }

            UpdateSurfaceCoords();

            shadowMap.Clear();

            foreach (var coord in surfaceCoords)
            {
                shadowMap.CalcShadow(this, coord, chunksList);
                shadowMap.CalcShadow(this, new Vector3Int(coord.x + 1, coord.y, coord.z), chunksList);
                shadowMap.CalcShadow(this, new Vector3Int(coord.x - 1, coord.y, coord.z), chunksList);
                shadowMap.CalcShadow(this, new Vector3Int(coord.x, coord.y, coord.z + 1), chunksList);
                shadowMap.CalcShadow(this, new Vector3Int(coord.x, coord.y, coord.z - 1), chunksList);
            }

            shadowsDirty = false;
        }

        private float shadowStrength = 0.5f;

        private float getShadow(Vector3Int coord) {
            return shadowMap.GetShadow(this, coord);
        }

        public float GetLighting(Vector3Int coord)
        {
            var shadow = getShadow(coord);
            return shadow * shadowStrength;
        }

        public void UpdateNormals()
        {
            if (!normalsNeedsUpdate)
            {
                return;
            }

            UpdateSurfaceCoords();

            var lightDir = Raycast4545.LightDir;

            foreach (var coord in SurfaceCoords)
            {
                var normal = CalcNormal(coord);
                if (normal != null) {
                    normals[coord] = normal.Value;
                    lightNormals[coord] = Vector3.Dot(lightDir, normal.Value);    
                }
            }

            normalsNeedsUpdate = false;
        }

        private Vector3? CalcNormal(Vector3Int coord) {
            if (coord.x >= size - 1 || coord.y >= size - 1 || coord.z >= size - 1) {
                return null;
            }

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

        public float? GetLightNormal(Vector3Int coord) {
            if (lightNormals.ContainsKey(coord)) {
                return lightNormals[coord];
            }
            return null;
        }

        public Vector3? GetNormal(Vector3Int coord) {
            if (normals.ContainsKey(coord)) {
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

        public float Get(Vector3Int coord) {
            return Get(coord.x, coord.y, coord.z);
        }
    }
}