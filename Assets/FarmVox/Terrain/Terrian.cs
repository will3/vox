using System;
using System.Collections;
using System.Collections.Generic;
using FarmVox.Scripts;
using FarmVox.Voxel;
using FarmVox.Workers;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class Terrian : MonoBehaviour
    {
        public static Terrian Instance;

        private int Size
        {
            get { return config.Size; }
        }

        float _sizeF;

        public Chunks DefaultLayer { get; private set; }

        public Chunks TreeLayer { get; private set; }

        public Chunks WaterLayer { get; private set; }

        public Chunks BuildingLayer { get; private set; }

        private readonly Dictionary<Vector3Int, TerrianChunk> _map = new Dictionary<Vector3Int, TerrianChunk>();
        
        private readonly Dictionary<Vector3Int, TerrianColumn> _columns = new Dictionary<Vector3Int, TerrianColumn>();
        
        private readonly List<TerrianColumn> _columnList = new List<TerrianColumn>();
        
        public TerrianConfig config
        {
            get { return _config; }
        }

        private TerrianConfig _config;

        Chunks[] _chunksToDraw;

        private TreeMap TreeMap { get; set; }

        private VoxelShadowMap ShadowMap { get; set; }

        private Bounds _bounds;

        public HeightMap heightMap;

        private void GenerateColumn(Vector3Int columnOrigin) {
            var maxChunksY = config.MaxChunksY;
            var list = new List<TerrianChunk>();
            for (var j = 0; j < maxChunksY; j++)
            {
                var origin = new Vector3Int(columnOrigin.x, j * Size, columnOrigin.z);
                var terrianChunk = GetOrCreateTerrianChunk(origin);
                list.Add(terrianChunk);
            }

            if (!_columns.ContainsKey(columnOrigin))
            {
                var terrianColumn = new TerrianColumn(Size, columnOrigin, list);
                _columns[columnOrigin] = terrianColumn;

                _columnList.Add(terrianColumn);
            }

            _columnList.Sort(new TerrianColumnDistanceComparer());
        }

        private void InitColumns() {
            for (var i = -config.MaxChunksX; i < config.MaxChunksX; i++)
            {
                for (var k = -config.MaxChunksX; k < config.MaxChunksX; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * Size;
                    GenerateColumn(columnOrigin);
                }
            }
        }

        private void Awake()
        {
            _config = new TerrianConfig();
            
            var size = config.Size;
            _sizeF = size;

            _bounds = new Bounds();
            _bounds.min = new Vector3(-config.MaxChunksX, 0, -config.MaxChunksX) * size;
            _bounds.max = new Vector3(config.MaxChunksX, config.MaxChunksY, config.MaxChunksX) * size;

            var boundsInt = config.BoundsInt;

            DefaultLayer = new Chunks(size);
            DefaultLayer.NormalStrength = 0.4f;
            TreeLayer = new Chunks(size);
            TreeLayer.NormalStrength = 0.2f;
            WaterLayer = new Chunks(size);
            WaterLayer.Transparent = true;
            WaterLayer.UseNormals = false;
            BuildingLayer = new Chunks(size);

            TreeMap = new TreeMap(boundsInt);

            DefaultLayer.GetGameObject().layer = LayerMask.NameToLayer("terrian");
            TreeLayer.GetGameObject().layer = LayerMask.NameToLayer("trees");
            WaterLayer.GetGameObject().layer = LayerMask.NameToLayer("water");
             
            DefaultLayer.GetGameObject().name = "default";
            TreeLayer.GetGameObject().name = "trees";
            WaterLayer.GetGameObject().name = "water";

            DefaultLayer.GetGameObject().transform.parent = transform;
            TreeLayer.GetGameObject().transform.parent = transform;
            WaterLayer.GetGameObject().transform.parent = transform;

            WaterLayer.UseNormals = false;
            WaterLayer.IsWater = true;

            _chunksToDraw = new Chunks[] { DefaultLayer, TreeLayer, WaterLayer, BuildingLayer };

            ShadowMap = new VoxelShadowMap(size, config);

            heightMap = new HeightMap();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Only one instance of Terrian is allowed");
            }
        }

        public void Start()
        {
            InitColumns();

            var queue = GameController.Instance.Queue;

            foreach (var column in _columnList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    queue.Enqueue(new GenGroundWorker(chunk, DefaultLayer, config));
                    queue.Enqueue(new GenWaterWorker(chunk, DefaultLayer, WaterLayer, config));
                    queue.Enqueue(new GenTreesWorker(config, chunk, DefaultLayer, TreeLayer, this, TreeMap));
                }
            }
            
            foreach (var column in _columnList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    queue.Enqueue(new GenWaterfallWorker(chunk, DefaultLayer, config));
                }
            }
            
            StartCoroutine(UpdateMeshesLoop());
        }

        private IEnumerator UpdateMeshesLoop() {
            while (true) {
                foreach (var column in _columnList)
                {
                    UpdateMaterial();
                    ShadowMap.Update();

                    foreach (var chunks in _chunksToDraw)
                    {
                        foreach (var terrianChunk in column.TerrianChunks)
                        {
                            var worker = new DrawChunkWorker(config, ShadowMap, chunks, terrianChunk);    
                            worker.Start();
                        }
                    }
                    
                    yield return null;
                }
            }
        }

        private void UpdateMaterial() {
            foreach (var chunks in _chunksToDraw) {
                foreach (var chunk in chunks.Map.Values) {
                    var material = chunk.Material;

                    var origin = chunk.Origin;
                    material.SetVector("_Origin", (Vector3)origin);
                    material.SetInt("_Size", Size);

                    ShadowMap.UpdateMaterial(material, origin);
                }
            }
        }

        private TerrianChunk GetTerrianChunk(Vector3Int origin) {
            TerrianChunk terrianChunk = null;
            _map.TryGetValue(origin, out terrianChunk);
            return terrianChunk;
        }

        public TerrianColumn GetTerrianColumn(Vector3Int origin) {
            TerrianColumn terrianColumn = null;
            _columns.TryGetValue(origin, out terrianColumn);
            return terrianColumn;
        }

        private TerrianChunk GetOrCreateTerrianChunk(Vector3Int origin)
        {
            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            Vector3Int key = new Vector3Int(origin.x / Size, origin.y / Size, origin.z / Size);
            _map[origin] = new TerrianChunk(key, Size, this);
            _map[origin].Config = config;
            return _map[origin];
        }

        public Vector3Int GetOrigin(float i, float j, float k)
        {
            return new Vector3Int(
               Mathf.FloorToInt(i / _sizeF) * Size,
               Mathf.FloorToInt(j / _sizeF) * Size,
               Mathf.FloorToInt(k / _sizeF) * Size
            );
        }

        private Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / _sizeF) * Size,
                Mathf.FloorToInt(j / _sizeF) * Size,
                Mathf.FloorToInt(k / _sizeF) * Size
            );
        }

        public bool GetWater(Vector3Int coord) {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null) {
                return false;
            }
            return terrianChunk.GetWater(coord);
        }

        public void SetWater(Vector3Int coord) {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null)
            {
                return;
            }
            terrianChunk.SetWater(coord, true);
        }

        private void OnDestroy()
        {
            ShadowMap.Dispose();
            foreach (var tc in _map.Values) {
                tc.Dispose();
            }
        }
    }
}
