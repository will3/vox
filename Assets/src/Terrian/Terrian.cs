using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using System.Linq;
using UnityEngine.AI;

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

        private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
        public Material Material
        {
            get
            {
                return material;
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

        private TerrianConfig config = new TerrianConfig();

        private Chunks[] chunksToDraw;
        private Chunks[] chunksCastingShadows;
        private Chunks[] chunksReceivingShadows;

        private Dictionary<Vector3Int, TerrianColumn> columns = new Dictionary<Vector3Int, TerrianColumn>();

        GameObject terrianObject;

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
        }

        public void Update()
        {
            var start = DateTime.Now;

            int x = Mathf.FloorToInt(Target.x / sizeF);
            int z = Mathf.FloorToInt(Target.z / sizeF);
            var maxChunksY = config.maxChunksY;
            var generateDis = config.generateDis;

            for (int i = x - generateDis; i <= x + generateDis; i++)
            {
                for (int k = z - generateDis; k <= z + generateDis; k++)
                {
                    var list = new List<TerrianChunk>();
                    for (int j = 0; j < maxChunksY; j++)
                    {
                        var origin = new Vector3Int(i, j, k) * size;
                        var terrianChunk = GetOrCreateTerrianChunk(origin);
                        list.Add(terrianChunk);
                    }

                    var columnOrigin = new Vector3Int(i, 0, k) * size;
                    if (!columns.ContainsKey(columnOrigin))
                    {
                        var terrianColumn = new TerrianColumn(columnOrigin, list);
                        columns[columnOrigin] = terrianColumn;
                    }
                }
            }

            // Update distance
            foreach (var kv in map) {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);
            }

            foreach (var column in columns.Values)
            {
                if (column.generatedTerrian)
                {
                    continue;
                }

                GenerateGround(column);
                //GenerateWaters(column);
                //GenerateTrees(column);
                //GenerateGrass(column);
                GenerateColliders(column);

                column.generatedTerrian = true;
            }

            foreach (var column in columns.Values)
            {
                //GenerateWaterfalls(column);
            }

            bool skipShadows = false;
            foreach (var column in columns.Values)
            {
                GenerateShadows(column);
                GenerateMeshes(column, skipShadows);
            }

            // TODO
            //UpdateVision();

            var end = System.DateTime.Now;
            if ((end - start).Milliseconds > 16) {
                Debug.Log("Terrian Update took: " + (end - start).Milliseconds);    
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

        private void print(Array3<Voxel> shape, Vector3Int pos, Chunks layer, Vector3Int offset)
        {
            for (var i = 0; i < shape.Width; i++)
            {
                for (var j = 0; j < shape.Height; j++)
                {
                    for (var k = 0; k < shape.Depth; k++)
                    {
                        var voxel = shape.Get(i, j, k);
                        if (voxel == null)
                        {
                            continue;
                        }
                        if (voxel.value <= 0)
                        {
                            continue;
                        }
                        var x = pos.x + i + offset.x;
                        var y = pos.y + j + offset.y;
                        var z = pos.z + k + offset.z;
                        layer.Set(x, y, z, voxel.value);
                        layer.SetColor(x, y, z, voxel.color);
                    }
                }
            }
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

        public RaycastResult Trace(Vector3 pos, Vector3 dir, int maxD) {
            var list = new Chunks[] { defaultLayer, defaultLayer };
            return Raycast.Trace(list, pos, dir, maxD);
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
    }
}
