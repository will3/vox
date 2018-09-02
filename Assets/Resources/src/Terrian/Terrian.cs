using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using System.Collections;

namespace FarmVox
{
    public partial class Terrian
    {
        private int size;

        public int Size
        {
            get
            {
                return size;
            }
        }

        float sizeF;

        Chunks defaultLayer;

        public Chunks DefaultLayer
        {
            get
            {
                return defaultLayer;
            }
        }

        Chunks treeLayer;

        public Chunks TreeLayer
        {
            get
            {
                return treeLayer;
            }
        }

        Chunks waterLayer;

        public Chunks WaterLayer
        {
            get
            {
                return waterLayer;
            }
        }

        Chunks buildingLayer;

        public Chunks BuildingLayer
        {
            get
            {
                return buildingLayer;
            }
        }

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

        BuildGrid buildGrid;

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

        public Terrian(int size = 32)
        {
            this.size = size;
            sizeF = size;

            bounds = new Bounds();
            bounds.min = new Vector3(-config.maxChunksX, 0, -config.maxChunksX) * size;
            bounds.max = new Vector3(config.maxChunksX, config.maxChunksY, config.maxChunksX) * size;

            var boundsInt = config.BoundsInt;

            terrianObject = new GameObject("terrian");

            defaultLayer = new Chunks(size);
            treeLayer = new Chunks(size);
            waterLayer = new Chunks(size);
            waterLayer.transparent = true;
            buildingLayer = new Chunks(size);

            treeMap = new TreeMap(boundsInt);
            voxelMap = new VoxelMap(boundsInt);

            defaultLayer.GetGameObject().layer = LayerMask.NameToLayer("terrian");
            treeLayer.GetGameObject().layer = LayerMask.NameToLayer("trees");
            waterLayer.GetGameObject().layer = LayerMask.NameToLayer("water");

            var modifier = treeLayer.GetGameObject().AddComponent<NavMeshModifier>();
            modifier.overrideArea = true;
            modifier.area = NavMeshAreas.NotWalkable;
             
            defaultLayer.GetGameObject().name = "default";
            treeLayer.GetGameObject().name = "trees";
            waterLayer.GetGameObject().name = "water";

            defaultLayer.GetGameObject().transform.parent = terrianObject.transform;
            treeLayer.GetGameObject().transform.parent = terrianObject.transform;
            waterLayer.GetGameObject().transform.parent = terrianObject.transform;

            waterLayer.useNormals = false;
            waterLayer.isWater = true;

            chunksToDraw = new Chunks[] { defaultLayer, treeLayer, waterLayer, buildingLayer };

            shadowMap = new VoxelShadowMap(size, config);

            buildGrid = new BuildGrid(size);
        }

        void GenerateColumn(Vector3Int columnOrigin) {
            var maxChunksY = config.maxChunksY;
            var list = new List<TerrianChunk>();
            for (int j = 0; j < maxChunksY; j++)
            {
                var origin = new Vector3Int(columnOrigin.x, j * size, columnOrigin.z);
                var terrianChunk = GetOrCreateTerrianChunk(origin);
                list.Add(terrianChunk);
            }

            if (!columns.ContainsKey(columnOrigin))
            {
                var terrianColumn = new TerrianColumn(size, columnOrigin, list);
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
                    var columnOrigin = new Vector3Int(i, 0, k) * size;
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

            Vector3Int key = new Vector3Int(origin.x / size, origin.y / size, origin.z / size);
            map[origin] = new TerrianChunk(key, size, this);
            map[origin].Config = config;
            return map[origin];
        }

        public Vector3Int GetOrigin(float i, float j, float k)
        {
            return new Vector3Int(
               Mathf.FloorToInt(i / this.sizeF) * this.size,
               Mathf.FloorToInt(j / this.sizeF) * this.size,
               Mathf.FloorToInt(k / this.sizeF) * this.size
            );
        }

        public Vector3Int GetOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / this.sizeF) * this.size,
                Mathf.FloorToInt(j / this.sizeF) * this.size,
                Mathf.FloorToInt(k / this.sizeF) * this.size
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
