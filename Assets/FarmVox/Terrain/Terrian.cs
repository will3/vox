using System;
using System.Collections;
using System.Collections.Generic;
using FarmVox.Scripts;
using FarmVox.Terrain.Routing;
using FarmVox.Voxel;
using FarmVox.Workers;
using UnityEngine;
using UnityEngine.Serialization;

namespace FarmVox.Terrain
{
    public class Terrian : MonoBehaviour
    {
        public static Terrian Instance;
        
        public TerrianConfig Config;

        private int Size
        {
            get { return Config.Size; }
        }

        float _sizeF;

        public Chunks DefaultLayer { get; private set; }

        public Chunks TreeLayer { get; private set; }

        public Chunks WaterLayer { get; private set; }

        public Chunks WallLayer { get; private set; }

        public RouteChunks Routes { get; private set; }

        private readonly Dictionary<Vector3Int, TerrianChunk> _map = new Dictionary<Vector3Int, TerrianChunk>();
        
        private readonly Dictionary<Vector3Int, TerrianColumn> _columns = new Dictionary<Vector3Int, TerrianColumn>();
        
        private readonly List<TerrianColumn> _columnList = new List<TerrianColumn>();

        private Chunks[] _chunksToDraw;

        private TreeMap _treeMap;

        private VoxelShadowMap _shadowMap;

        public HeightMap HeightMap;

        private string _lastConfig;

        private void GenerateColumn(Vector3Int columnOrigin) {
            if (_columns.ContainsKey(columnOrigin))
            {
                return;
            }
            
            var maxChunksY = Config.MaxChunksY;
            var list = new List<TerrianChunk>();
            for (var j = 0; j < maxChunksY; j++)
            {
                var origin = new Vector3Int(columnOrigin.x, j * Size, columnOrigin.z);
                var terrianChunk = GetOrCreateTerrianChunk(origin);
                list.Add(terrianChunk);
            }
                
            var terrianColumn = new TerrianColumn(Size, columnOrigin, list);
            _columns[columnOrigin] = terrianColumn;

            var index = _columnList.BinarySearch(0, _columnList.Count, terrianColumn, new TerrianColumnDistanceComparer());

            if (index < 0)
            {
                _columnList.Insert(~index, terrianColumn);
            }
            else
            {
                _columnList.Insert(index, terrianColumn);
            }
        }

        private void InitColumns() {
            for (var i = -Config.MaxChunksX; i < Config.MaxChunksX; i++)
            {
                for (var k = -Config.MaxChunksX; k < Config.MaxChunksX; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * Size;
                    GenerateColumn(columnOrigin);
                }
            }
        }

        private void Awake() {
            var size = Config.Size;
            _sizeF = size;

            var boundsInt = Config.BoundsInt;

            DefaultLayer = new Chunks(size)
            {
                NormalStrength = Config.NormalStrength,
                ShadowStrength = Config.ShadowStrength,
                Layer = UserLayers.Terrian,
                Name = "default"
            };

            TreeLayer = new Chunks(size)
            {
                NormalStrength = Config.TreesNormalStrength,
                ShadowStrength = Config.ShadowStrength,
                Layer = UserLayers.Trees,
                Name = "trees"
            };

            WaterLayer = new Chunks(size)
            {
                Transparent = true,
                UseNormals = false,
                IsWater = true,
                ShadowStrength = Config.ShadowStrength,
                Layer = UserLayers.Water,
                Name = "water"
            };

            WallLayer = new Chunks(size)
            {
                NormalStrength = Config.WallNormalStrength, 
                ShadowStrength = Config.WallShadowStrength, 
                Layer = UserLayers.Wall,
                Name = "walls"
            };

            _treeMap = new TreeMap(boundsInt);

            DefaultLayer.GetGameObject().transform.parent = transform;
            TreeLayer.GetGameObject().transform.parent = transform;
            WaterLayer.GetGameObject().transform.parent = transform;
            WallLayer.GetGameObject().transform.parent = transform;

            _chunksToDraw = new[] { DefaultLayer, TreeLayer, WaterLayer, WallLayer };

            _shadowMap = new VoxelShadowMap(size);

            HeightMap = new HeightMap();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Only one instance of Terrian is allowed");
            }
            
            Routes = new RouteChunks(Config.Size);
        }

        private void Start()
        {
            InitColumns();

            var queue = GameController.Instance.Queue;

            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenGroundWorker(chunk, DefaultLayer, Config));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenWaterWorker(chunk, DefaultLayer, WaterLayer, Config));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenTreesWorker(Config, chunk, DefaultLayer, TreeLayer, _treeMap));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenWaterfallWorker(chunk, DefaultLayer, Config, this));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenRoutesWorker(chunk.Origin, Routes, DefaultLayer));
            });
            
            StartCoroutine(UpdateMeshesLoop());
        }

        private IEnumerator UpdateMeshesLoop() {
            while (true) {
                _shadowMap.Update();
                foreach (var column in _columnList)
                {
                    foreach (var chunks in _chunksToDraw)
                    {
                        foreach (var terrianChunk in column.TerrianChunks)
                        {
                            var chunk = chunks.GetChunk(terrianChunk.Origin);
                            if (chunk == null)
                            {
                                continue;
                            }
                            
                            UpdateMaterial(chunk);

                            if (!chunk.Dirty)
                            {
                                continue;
                            }
                            
                            var worker = new DrawChunkWorker(Config, _shadowMap, chunk, terrianChunk);    
                            worker.Start();
                        }
                    }
                    
                    yield return null;
                }
            }            
        }

        private void UpdateMaterial(Chunk chunk) {
            var material = chunk.Material;

            var origin = chunk.Origin;
            material.SetVector("_Origin", (Vector3)origin);
            material.SetInt("_Size", Size);

            material.SetFloat("_WaterfallShadowStrength", Config.WaterfallShadowStrength);
            material.SetFloat("_WaterfallSpeed", Config.WaterfallSpeed);
            material.SetFloat("_WaterfallWidth", Config.WaterfallWidth);
            material.SetFloat("_WaterfallMin", Config.WaterfallMin);
            material.SetFloat("_WaterfallVariance", Config.WaterfallVariance);

            var shadowStrength = chunk.Chunks.ShadowStrength;
            
            _shadowMap.UpdateMaterial(material, origin, shadowStrength);
        }

        private TerrianChunk GetTerrianChunk(Vector3Int origin) {
            TerrianChunk terrianChunk;
            _map.TryGetValue(origin, out terrianChunk);
            return terrianChunk;
        }

        public TerrianColumn GetTerrianColumn(Vector3Int origin) {
            TerrianColumn terrianColumn;
            _columns.TryGetValue(origin, out terrianColumn);
            return terrianColumn;
        }

        private TerrianChunk GetOrCreateTerrianChunk(Vector3Int origin)
        {
            if (_map.ContainsKey(origin))
            {
                return _map[origin];
            }

            var key = new Vector3Int(origin.x / Size, origin.y / Size, origin.z / Size);
            _map[origin] = new TerrianChunk(key, Size);
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
            return terrianChunk != null && terrianChunk.GetWater(coord);
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
            if (_shadowMap != null)
            {
                _shadowMap.Dispose();    
            }
            
            foreach (var tc in _map.Values) {
                tc.Dispose();
            }
            
            DefaultLayer.Dispose();
            TreeLayer.Dispose();
            WaterLayer.Dispose();
        }

        private delegate void VisitChunk(TerrianChunk chunk);
        
        private void VisitChunks(VisitChunk visit)
        {
            foreach (var column in _columnList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    visit(chunk);
                }
            }
        }
        
        public float GetWaterfall(Vector3Int coord)
        {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetTerrianChunk(origin);

            if (terrianChunk == null)
            {
                return 0;
            }

            return terrianChunk.GetWaterfall(coord - terrianChunk.Origin);
        }

        public void SetWaterfall(Vector3Int coord, float value)
        {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var chunk = GetOrCreateTerrianChunk(origin);
            chunk.SetWaterfall(coord - origin, value);
        }

        public void AddWall(Vector3Int coord)
        {
            const int height = 3;
            const int yOffset = 1;
            
            for (var i = 0; i < height; i++)
            {
                WallLayer.Set(coord + new Vector3Int(0, i + yOffset, 0), 1.0f);
                WallLayer.SetColor(coord + new Vector3Int(0, i + yOffset, 0), Color.red);    
            }
        }
    }
}
