using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
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

        VoxelShadowMap shadowMap;

        TreeMap treeMap;

        public TreeMap TreeMap
        {
            get
            {
                return treeMap;
            }
        }

        public VoxelShadowMap ShadowMap
        {
            get
            {
                return shadowMap;
            }
        }

        VoxelMap voxelMap;

        Bounds bounds;

        HeightMap heightMap;

        public Terrian(int size = 32)
        {
            this.Size = size;
            sizeF = size;

            bounds = new Bounds();
            bounds.min = new Vector3(-config.maxChunksX, 0, -config.maxChunksX) * size;
            bounds.max = new Vector3(config.maxChunksX, config.maxChunksY, config.maxChunksX) * size;

            var boundsInt = config.BoundsInt;

            terrianObject = new GameObject("terrian");

            DefaultLayer = new Chunks(size);
            DefaultLayer.normalStrength = 0.4f;
            TreeLayer = new Chunks(size);
            TreeLayer.normalStrength = 0.2f;
            WaterLayer = new Chunks(size);
            WaterLayer.transparent = true;
            BuildingLayer = new Chunks(size);

            treeMap = new TreeMap(boundsInt);
            voxelMap = new VoxelMap(boundsInt);

            DefaultLayer.GetGameObject().layer = LayerMask.NameToLayer("terrian");
            TreeLayer.GetGameObject().layer = LayerMask.NameToLayer("trees");
            WaterLayer.GetGameObject().layer = LayerMask.NameToLayer("water");
             
            DefaultLayer.GetGameObject().name = "default";
            TreeLayer.GetGameObject().name = "trees";
            WaterLayer.GetGameObject().name = "water";

            DefaultLayer.GetGameObject().transform.parent = terrianObject.transform;
            TreeLayer.GetGameObject().transform.parent = terrianObject.transform;
            WaterLayer.GetGameObject().transform.parent = terrianObject.transform;

            WaterLayer.useNormals = false;
            WaterLayer.isWater = true;

            chunksToDraw = new Chunks[] { DefaultLayer, TreeLayer, WaterLayer, BuildingLayer };

            shadowMap = new VoxelShadowMap(size, config);

            heightMap = new HeightMap(size);
        }

        void GenerateColumn(Vector3Int columnOrigin) {
            var maxChunksY = config.maxChunksY;
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
            for (var i = -config.maxChunksX; i < config.maxChunksX; i++)
            {
                for (var k = -config.maxChunksX; k < config.maxChunksX; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * Size;
                    GenerateColumn(columnOrigin);
                }
            }
        }

        public IEnumerator UpdateTerrianLoop() {
            foreach (var column in columnsList)
            {
                if (column.generatedTerrian)
                {
                    continue;
                }

                GenerateGround(column);

                if (config.generateWater) {
                    GenerateWaters(column);    
                }

                if (config.generateTrees) {
                    GenerateTrees(column);    
                }

                column.generatedTerrian = true;

                heightMap.LoadColumn(column);

                yield return null; // new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator UpdateWaterfallsLoop() {
            while (true) {
                foreach (var chunk in map.Values) {
                    GenerateWaterfalls(chunk);
                    yield return null;
                }
            }
        }

        public IEnumerator UpdateMeshesLoop() {
            while (true) {
                foreach (var column in columnsList)
                {
                    UpdateMaterial();
                    shadowMap.Update();
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
            shadowMap.Dispose();
            foreach (var tc in map.Values) {
                tc.Dispose();
            }
        }
    }
}
