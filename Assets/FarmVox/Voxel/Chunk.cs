using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmVox.Voxel
{
    public class Chunk : IDisposable, IWaterfallChunk
    {
        public readonly int Size;
        public readonly Vector3Int Origin;
        public readonly int DataSize;
        
        public float[] Data { get; private set; }
        
        public Chunks Chunks { get; set; }
        public bool Dirty { get; set; }
        
        private Material _material;
        private bool _waterfallDirty;

        public Material Material {
            get {
                if (_material != null) return _material;
                _material = Chunks.Transparent ? Materials.GetVoxelMaterialTrans() : Materials.GetVoxelMaterial();
                return _material;
            }
        }
        
        private GameObject _gameObject;
        
        public Mesh Mesh { get; set; }

        public readonly HashSet<Vector3Int> SurfaceCoords = new HashSet<Vector3Int>();
        public readonly HashSet<Vector3Int> SurfaceCoordsUp = new HashSet<Vector3Int>();
        public readonly Dictionary<Vector3Int, Vector3> Normals = new Dictionary<Vector3Int, Vector3>();

        public Color[] Colors { get; private set; }
        
        private bool _surfaceCoordsDirty = true;
        private bool _normalsDirty = true;

        private ComputeBuffer _voxelDataBuffer;

        private List<CoordData> _coordData = new List<CoordData>();
        
        public void SetVoxelData(List<CoordData> coordData)
        {
            _coordData = coordData;
            UpdateVoxelDataBuffer();
        }

        private void UpdateVoxelDataBuffer()
        {
            if (_voxelDataBuffer != null)
            {
                _voxelDataBuffer.Dispose();
            }

            if (_coordData.Count == 0)
            {
                return;
            }
            
            _voxelDataBuffer = new ComputeBuffer(_coordData.Count, CoordData.Size);
            _voxelDataBuffer.SetData(_coordData);
        }

        private ComputeBuffer _defaultVoxelDataBuffer;
        
        public ComputeBuffer GetVoxelDataBuffer()
        {
            if (_coordData.Count == 0)
            {
                return _defaultVoxelDataBuffer ?? (_defaultVoxelDataBuffer = new ComputeBuffer(1, CoordData.Size));
            }

            return _voxelDataBuffer;
        }
        
        public void SetColors(Color[] colors)
        {
            if (colors.Length != Colors.Length)
            {
                throw new ArgumentException("invalid length");
            }
            Colors = colors;
            Dirty = true;
        }

        public void SetData(float[] data)
        {
            if (data.Length != Data.Length)
            {
                throw new ArgumentException("invalid length");
            }
            Data = data;
            Dirty = true;
            _surfaceCoordsDirty = true;
            _normalsDirty = true;
        }

        private readonly Dictionary<Vector3Int, float> _waterfalls = new Dictionary<Vector3Int, float>();

        private GameObject GetGameObject() {
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
                throw new Exception("out of bounds:" + new Vector3Int(i, j, k));
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
                throw new Exception("out of bounds:" + new Vector3Int(i, j, k));
            }
            var index = GetIndex(i, j, k);
            Data[index] = v;
            Dirty = true;
            _surfaceCoordsDirty = true;
            _normalsDirty = true;
        }

        public void SetColor(int i, int j, int k, Color v)
        {
            var index = GetIndex(i, j, k);
            Colors[index] = v;
            Dirty = true;
        }

        public Color GetColor(int i, int j, int k)
        {
            var index = GetIndex(i, j, k);
            return Colors[index];
        }

        private int GetIndex(int i, int j, int k)
        {
            var index = i * DataSize * DataSize + j * DataSize + k;
            return index;
        }

        public void UpdateNormals()
        {
            if (!_normalsDirty)
            {
                return;
            }

            UpdateSurfaceCoords();

            Normals.Clear();
            
            foreach (var coord in SurfaceCoords)
            {
                if (coord.x >= DataSize - 1 || coord.y >= DataSize - 1 || coord.z >= DataSize - 1)
                {
                    continue;
                }
                
                var normal = CalcNormal(coord);
                Normals[coord] = normal;
            }

            _normalsDirty = false;
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

        private float Get(Vector3Int coord)
        {
            return Get(coord.x, coord.y, coord.z);
        }

        /// <summary>
        /// Set water fall, with local coord
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="v"></param>
        public void SetWaterfall(Vector3Int coord, float v)
        {
            _waterfallDirty = true;
            _waterfalls[coord] = v;
        }

        /// <summary>
        /// Get water fall, with local coord
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public float GetWaterfall(Vector3Int coord)
        {
            float value;
            _waterfalls.TryGetValue(coord, out value);
            return value;
        }

        public void Clear()
        {
            SetData(new float[DataSize * DataSize * DataSize]);
            SetColors(new Color[DataSize * DataSize * DataSize]);
        }

        public void Dispose()
        {
            if (_voxelDataBuffer != null)
            {
                _voxelDataBuffer.Dispose();    
            }

            if (_defaultVoxelDataBuffer != null)
            {
                _defaultVoxelDataBuffer.Dispose();
            }
        }
    }
}