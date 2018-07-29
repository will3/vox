using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

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

        public Vector3 Target;

        private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();

        public Dictionary<Vector3Int, TerrianChunk> Map
        {
            get
            {
                return map;
            }
        }

        TerrianConfig config = new TerrianConfig();

        Chunks[] chunksToDraw;
        Chunks[] chunksCastingShadows;
        Chunks[] chunksReceivingShadows;

        Dictionary<Vector3Int, TerrianColumn> columns = new Dictionary<Vector3Int, TerrianColumn>();

        GameObject terrianObject;

        VoxelShadowMap shadowMap;

        public VoxelShadowMap ShadowMap
        {
            get
            {
                return shadowMap;
            }
        }

        TerrianColumn GetColumn(Vector3Int origin) {
            if (columns.ContainsKey(origin)) {
                return columns[origin];
            }
            return null;
        }

        public Terrian(int size = 32)
        {
            this.size = size;
            sizeF = size;

            terrianObject = new GameObject("terrian");
            defaultLayer = new Chunks(size);
            treeLayer = new Chunks(size);
            waterLayer = new Chunks(size);

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

            chunksToDraw = new Chunks[] { defaultLayer, treeLayer, waterLayer };
            chunksCastingShadows = new Chunks[] { defaultLayer, treeLayer };
            chunksReceivingShadows = new Chunks[] { waterLayer, defaultLayer, treeLayer };

            shadowMap = new VoxelShadowMap(size, config);
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
                var terrianColumn = new TerrianColumn(columnOrigin, list);
                columns[columnOrigin] = terrianColumn;
            }
        }

        public void Update()
        {
            PerformanceLogger.Start("Terrian update");

            var start = DateTime.Now;

            int x = Mathf.FloorToInt(Target.x / sizeF);
            int z = Mathf.FloorToInt(Target.z / sizeF);

            var generateDis = config.generateDis;

            for (int i = x - generateDis; i <= x + generateDis; i++)
            {
                for (int k = z - generateDis; k <= z + generateDis; k++)
                {
                    var columnOrigin = new Vector3Int(i, 0, k) * size;
                    GenerateColumn(columnOrigin);
                }
            }

            // Update distance
            foreach (var kv in map) {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);
            }

            PerformanceLogger.Start("Generate terrian");

            foreach (var column in columns.Values)
            {
                if (column.generatedTerrian)
                {
                    continue;
                }

                GenerateGround(column);
                GenerateWaters(column);
                //GenerateTrees(column);

                column.generatedTerrian = true;
            }

            PerformanceLogger.End();

            foreach (var column in columns.Values)
            {
                GenerateWaterfalls(column);
            }

            UpdateMaterial();

            foreach (var column in columns.Values)
            {
                GenerateMeshes(column);
            }

            shadowMap.Update();

            PerformanceLogger.End();
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

        public bool GetTree(Vector3Int coord) {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null)
            {
                return false;
            }
            return terrianChunk.GetTree(coord);
        }

        public void SetTree(Vector3Int coord) {
            var origin = GetOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetOrCreateTerrianChunk(origin);
            terrianChunk.SetTree(coord, true);
        }

        public void Dispose() {
            shadowMap.Dispose();
        }
    }
}
