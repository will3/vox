using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    public class Chunk
    {
        public readonly int Size;
        public readonly Vector3Int Origin;
        public float[] Data { get; private set; }
        public readonly int DataSize;
        
        public Chunks Chunks { get; set; }
        public bool Dirty { get; set; }
        
        private Material _material;
        private GameObject _gameObject;
        
        public Mesh Mesh { get; set; }

        public readonly HashSet<Vector3Int> SurfaceCoords = new HashSet<Vector3Int>();
        public readonly HashSet<Vector3Int> SurfaceCoordsUp = new HashSet<Vector3Int>();
        private readonly Dictionary<Vector3Int, float> _lightNormals = new Dictionary<Vector3Int, float>();

        public Color[] Colors { get; private set; }
        
        private int[] Types { get; set; }
        private bool _surfaceCoordsDirty = true;
        private bool _normalsNeedsUpdate = true;
        
        public void SetTypes(int[] types) {
            Types = types;
        }

        public VoxelType GetType(Vector3Int coord) {
            var index = GetIndex(coord);
            var value = Types[index];
            return (VoxelType)value;
        }

        public void SetColors(Color[] colors)
        {
            if (colors.Length != Colors.Length)
            {
                throw new System.ArgumentException("invalid length");
            }
            Colors = colors;
            Dirty = true;
        }

        public void SetData(float[] data)
        {
            if (data.Length != Data.Length)
            {
                throw new System.ArgumentException("invalid length");
            }
            Data = data;
            Dirty = true;
            _surfaceCoordsDirty = true;
            _normalsNeedsUpdate = true;
        }

        private readonly Dictionary<Vector3Int, float> _waterfalls = new Dictionary<Vector3Int, float>();

        public Dictionary<Vector3Int, float> Waterfalls
        {
            get
            {
                return _waterfalls;
            }
        }

        public GameObject GetGameObject() {
            if (_gameObject == null) {
                var name = "Chunk" + Origin.ToString();
                _gameObject = new GameObject(name);
                _gameObject.transform.localPosition = Origin;
                _gameObject.transform.parent = Chunks.GetGameObject().transform;
            }
            return _gameObject;
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
            if (!_surfaceCoordsDirty)
            {
                return;
            }

            SurfaceCoords.Clear();
            SurfaceCoordsUp.Clear();

            for (var d = 0; d < 3; d++)
            {
                for (var i = 0; i < DataSize - 1; i++)
                {

                    for (var j = 0; j < DataSize - 1; j++)
                    {

                        for (var k = 0; k < DataSize - 1; k++)
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
                                SurfaceCoords.Add(coordA);
                                if (d == 1)
                                {
                                    SurfaceCoordsUp.Add(coordA);
                                }
                            }
                            else
                            {
                                SurfaceCoords.Add(coordB);
                            }
                        }
                    }
                }
            }

            _surfaceCoordsDirty = false;
        }

        public Material Material {
            get {
                if (_material != null) return _material;
                _material = Chunks.transparent ? Materials.GetVoxelMaterialTrans() : Materials.GetVoxelMaterial();
                return _material;
            }
        }

        public Chunk(int size, Vector3Int origin)
        {
            Size = size;
            Origin = origin;
            DataSize = size + 3;
            Data = new float[DataSize * DataSize * DataSize];
            Colors = new Color[DataSize * DataSize * DataSize];
        }

        public float Get(int i, int j, int k)
        {
            if (i < 0 || i >= DataSize ||
                j < 0 || j >= DataSize ||
                k < 0 || k >= DataSize)
            {
                throw new System.Exception("out of bounds:" + new Vector3Int(i, j, k).ToString());
            }
            var index = GetIndex(i, j, k);
            return Data[index];
        }

        public void Set(int i, int j, int k, float v)
        {
            if (i < 0 || i >= DataSize ||
                j < 0 || j >= DataSize ||
                k < 0 || k >= DataSize)
            {
                throw new System.Exception("out of bounds:" + new Vector3Int(i, j, k).ToString());
            }
            var index = GetIndex(i, j, k);
            Data[index] = v;
            Dirty = true;
            _surfaceCoordsDirty = true;
            _normalsNeedsUpdate = true;
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var index = GetIndex(i, j, k);
            Colors[index] = v;
            Dirty = true;
        }

        public void SetGlobal(int i, int j, int k, float v)
        {
            int max = Size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                Chunks.Set(i + Origin.x, j + Origin.y, k + Origin.z, v);
            }
            else
            {
                Set(i, j, k, v);
            }
        }

        public void SetColorGlobal(int i, int j, int k, Color color)
        {
            int max = Size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                Chunks.SetColor(i + Origin.x, j + Origin.y, k + Origin.z, color);
            }
            else
            {
                SetColor(i, j, k, color);
            }
        }



        public Color GetColor(int i, int j, int k)
        {
            var index = GetIndex(i, j, k);
            return Colors[index];
        }

        public Color GetColorGlobal(int i, int j, int k)
        {
            int max = Size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return Chunks.GetColor(i + Origin.x, j + Origin.y, k + Origin.z);
            }
            else
            {
                return GetColor(i, j, k);
            }
        }

        public float GetGlobal(int i, int j, int k)
        {
            int max = Size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                return Chunks.Get(i + Origin.x, j + Origin.y, k + Origin.z);
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
            int index = i * DataSize * DataSize + j * DataSize + k;
            return index;
        }

        public void UpdateNormals()
        {
            if (!_normalsNeedsUpdate)
            {
                return;
            }

            UpdateSurfaceCoords();

            var lightDir = Raycast4545.LightDir;

            foreach (var coord in SurfaceCoords)
            {
                if (coord.x >= DataSize - 1 || coord.y >= DataSize - 1 || coord.z >= DataSize - 1)
                {
                    continue;
                }
                else
                {
                    var normal = CalcNormal(coord);
                    normals[coord] = normal;
                    _lightNormals[coord] = Vector3.Dot(lightDir, normal);
                }
            }

            _normalsNeedsUpdate = false;
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
            if (_lightNormals.ContainsKey(coord))
            {
                return _lightNormals[coord];
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
            _waterfalls[coord] = v;
        }

        public bool GetWaterfall(Vector3Int coord)
        {
            return GetWaterfall(coord.x, coord.y, coord.z);
        }

        public bool GetWaterfall(int i, int j, int k)
        {
            var coord = new Vector3Int(i, j, k);
            return _waterfalls.ContainsKey(coord);
        }

        public bool IsSurfaceCoord(Vector3Int coord) {
            return SurfaceCoords.Contains(coord);
        }

        public bool IsSurfaceCoordUp(Vector3Int coord) {
            return SurfaceCoordsUp.Contains(coord);
        }
    }
}