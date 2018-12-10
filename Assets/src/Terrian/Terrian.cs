using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace FarmVox
{
    public partial class Terrian
    {
        public int Size { get; private set; }

        float sizeF;

        public Chunks DefaultLayer { get; private set; }

        public Chunks TreeLayer { get; private set; }

        public Chunks WaterLayer { get; private set; }

        public Chunks BuildingLayer { get; private set; }

        Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();

        public Dictionary<Vector3Int, TerrianChunk> Map
        {
            get
            {
                return map;
            }
        }

        TerrianConfig config = TerrianConfig.Instance;

        Chunks[] chunksToDraw;

        Dictionary<Vector3Int, TerrianColumn> columns = new Dictionary<Vector3Int, TerrianColumn>();
        List<TerrianColumn> columnsList = new List<TerrianColumn>();

        GameObject terrianObject;

        public TreeMap TreeMap { get; private set; }

        public VoxelShadowMap ShadowMap { get; private set; }

        Bounds bounds;

        public HeightMap heightMap;

        public Terrian(int size = 32)
        {
            Size = size;
            sizeF = size;

            bounds = new Bounds();
            bounds.min = new Vector3(-config.MaxChunksX, 0, -config.MaxChunksX) * size;
            bounds.max = new Vector3(config.MaxChunksX, config.MaxChunksY, config.MaxChunksX) * size;

            var boundsInt = config.BoundsInt;

            terrianObject = new GameObject("terrian");

            DefaultLayer = new Chunks(size);
            DefaultLayer.NormalStrength = 0.4f;
            TreeLayer = new Chunks(size);
            TreeLayer.NormalStrength = 0.2f;
            WaterLayer = new Chunks(size);
            WaterLayer.Transparent = true;
            BuildingLayer = new Chunks(size);

            TreeMap = new TreeMap(boundsInt);

            DefaultLayer.GetGameObject().layer = LayerMask.NameToLayer("terrian");
            TreeLayer.GetGameObject().layer = LayerMask.NameToLayer("trees");
            WaterLayer.GetGameObject().layer = LayerMask.NameToLayer("water");
             
            DefaultLayer.GetGameObject().name = "default";
            TreeLayer.GetGameObject().name = "trees";
            WaterLayer.GetGameObject().name = "water";

            DefaultLayer.GetGameObject().transform.parent = terrianObject.transform;
            TreeLayer.GetGameObject().transform.parent = terrianObject.transform;
            WaterLayer.GetGameObject().transform.parent = terrianObject.transform;

            WaterLayer.UseNormals = false;
            WaterLayer.IsWater = true;

            chunksToDraw = new Chunks[] { DefaultLayer, TreeLayer, WaterLayer, BuildingLayer };

            ShadowMap = new VoxelShadowMap(size, config);

            heightMap = new HeightMap();
        }

        void GenerateColumn(Vector3Int columnOrigin) {
            var maxChunksY = config.MaxChunksY;
            var list = new List<TerrianChunk>();
            for (int j = 0; j < maxChunksY; j++)
            {
                var origin = new Vector3Int(columnOrigin.x, j * Size, columnOrigin.z);
                var terrianChunk = GetOrCreateTerrianChunk(origin);
                list.Add(terrianChunk);
            }

            if (!columns.ContainsKey(columnOrigin))
            {
                var terrianColumn = new TerrianColumn(Size, columnOrigin, list);
                columns[columnOrigin] = terrianColumn;

                columnsList.Add(terrianColumn);
            }

            columnsList.Sort(new TerrianColumnDistanceComparer());
        }

        class TerrianColumnDistanceComparer : IComparer<TerrianColumn>
        {
            public int Compare(TerrianColumn x, TerrianColumn y)
            {
                return GetDistance(x).CompareTo(GetDistance(y));
            }

            float GetDistance(TerrianColumn column) {
                var xDis = column.Origin.x + column.Size / 2.0f;
                var zDis = column.Origin.z + column.Size / 2.0f;

                var distance = (Mathf.Abs(xDis) + Mathf.Abs(zDis)) * 1024;

                return distance;
            }
        }

        public void InitColumns() {
            for (var i = -config.MaxChunksX; i < config.MaxChunksX; i++)
            {
                for (var k = -config.MaxChunksX; k < config.MaxChunksX; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * Size;
                    GenerateColumn(columnOrigin);
                }
            }
        }

        public void Start()
        {
            var queue = Finder.FindGameController().Queue;

            foreach (var column in columnsList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    queue.Enqueue(new GenGroundWorker(chunk, DefaultLayer, config));
                    queue.Enqueue(new GenWaterWorker(chunk, DefaultLayer, WaterLayer, config));
                    queue.Enqueue(new GenTreesWorker(config, chunk, DefaultLayer, TreeLayer, this, TreeMap));
                }
            }
            
            foreach (var column in columnsList)
            {
                foreach (var chunk in column.TerrianChunks)
                {
                    queue.Enqueue(new GenWaterfallWorker(chunk, DefaultLayer, config));
                }
            }
        }

        public IEnumerator UpdateMeshesLoop() {
            while (true) {
                foreach (var column in columnsList)
                {
                    UpdateMaterial();
                    ShadowMap.Update();
                    GenerateMeshes(column);
                    yield return null;
                }
            }
        }

        public TerrianChunk GetTerrianChunk(Vector3Int origin) {
            TerrianChunk terrianChunk = null;
            map.TryGetValue(origin, out terrianChunk);
            return terrianChunk;
        }

        public TerrianColumn GetTerrianColumn(Vector3Int origin) {
            TerrianColumn terrianColumn = null;
            columns.TryGetValue(origin, out terrianColumn);
            return terrianColumn;
        }

        TerrianChunk GetOrCreateTerrianChunk(Vector3Int origin)
        {
            if (map.ContainsKey(origin))
            {
                return map[origin];
            }

            Vector3Int key = new Vector3Int(origin.x / Size, origin.y / Size, origin.z / Size);
            map[origin] = new TerrianChunk(key, Size, this);
            map[origin].Config = config;
            return map[origin];
        }

        public Vector3Int GetOrigin(float i, float j, float k)
        {
            return new Vector3Int(
               Mathf.FloorToInt(i / this.sizeF) * this.Size,
               Mathf.FloorToInt(j / this.sizeF) * this.Size,
               Mathf.FloorToInt(k / this.sizeF) * this.Size
            );
        }

        public Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / this.sizeF) * this.Size,
                Mathf.FloorToInt(j / this.sizeF) * this.Size,
                Mathf.FloorToInt(k / this.sizeF) * this.Size
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

        public void Dispose() {
            ShadowMap.Dispose();
            foreach (var tc in map.Values) {
                tc.Dispose();
            }
        }
    }
}
