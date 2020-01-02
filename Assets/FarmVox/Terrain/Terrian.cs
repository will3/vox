using System;
using System.Collections.Generic;
using FarmVox.Scripts;
using FarmVox.Terrain.Routing;
using FarmVox.Voxel;
using FarmVox.Workers;
using UnityEngine;

namespace FarmVox.Terrain
{
    public class Terrian : MonoBehaviour
    {
        public static Terrian Instance;
        
        public TerrianConfig Config;

        private int Size => Config.Size;

        float _sizeF;

        public Chunks defaultLayer;
        public Chunks treeLayer;
        public Chunks waterLayer;
        public Chunks wallLayer;
        public Trees trees;
        public Waterfalls waterfalls;
        
        public RouteChunks Routes { get; private set; }

        private readonly Dictionary<Vector3Int, TerrianChunk> _map = new Dictionary<Vector3Int, TerrianChunk>();
        
        private readonly Dictionary<Vector3Int, TerrianColumn> _columns = new Dictionary<Vector3Int, TerrianColumn>();
        
        private readonly List<TerrianColumn> _columnList = new List<TerrianColumn>();

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

            // TODO move gen to individual components
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenGroundWorker(chunk, defaultLayer, Config));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenWaterWorker(chunk, defaultLayer, waterLayer, Config));
            });

            trees.GenerateTrees(this);

            VisitChunks(chunk =>
            {
                queue.Enqueue(new ActionWorker(() =>
                {
                    waterfalls.GenerateWaterfalls(chunk);
                }));
            });
            
            VisitChunks(chunk =>
            {
                queue.Enqueue(new GenRoutesWorker(chunk.Origin, Routes, defaultLayer));
            });
        }

        public TerrianChunk GetTerrianChunk(Vector3Int origin)
        {
            return _map.TryGetValue(origin, out var terrianChunk) ? terrianChunk : null;
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

        private Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / _sizeF) * Size,
                Mathf.FloorToInt(j / _sizeF) * Size,
                Mathf.FloorToInt(k / _sizeF) * Size
            );
        }

        public void VisitChunks(Action<TerrianChunk> visit)
        {
            foreach (var column in _columnList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    visit(chunk);
                }
            }
        }

        public bool IsGround(Vector3Int coord)
        {
            return defaultLayer.Get(coord) > 0;
        }
    }
}
