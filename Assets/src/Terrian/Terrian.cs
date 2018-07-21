using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using System.Linq;

namespace FarmVox
{

    public partial class Terrian
    {
        private int size;
        private float sizeF;

        private Chunks defaultLayer;
        private Chunks treeLayer;
        private Chunks waterLayer;

        private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
        public Transform Transform;
        public Vector3 Target;

        private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();
        private HashSet<Actor> actors = new HashSet<Actor>();

        public Dictionary<Vector3Int, TerrianChunk> Map
        {
            get
            {
                return map;
            }
        }

        private TerrianConfig config = new TerrianConfig();

        MarchingCubes marchingCubes = new MarchingCubes();

        private Chunks[] chunksToDraw;
        private Chunks[] chunksCastingShadows;

        public Terrian(int size = 32)
        {
            this.size = size;
            sizeF = size;

            defaultLayer = new Chunks(size);
            treeLayer = new Chunks(size);
            waterLayer = new Chunks(size);
            waterLayer.useNormals = false;
            waterLayer.isWater = true;

            chunksToDraw = new Chunks[] { defaultLayer, treeLayer, waterLayer };
            chunksCastingShadows = new Chunks[] { defaultLayer, treeLayer };
        }

        public void Update()
        {
            int x = Mathf.FloorToInt(Target.x / sizeF);
            int z = Mathf.FloorToInt(Target.z / sizeF);
            var maxChunksY = config.maxChunksY;
            var generateDis = config.generateDis;

            for (int j = 0; j < maxChunksY; j++)
            {
                for (int i = x - generateDis; i <= x + generateDis; i++)
                {
                    for (int k = z - generateDis; k <= z + generateDis; k++)
                    {
                        var origin = new Vector3Int(i, j, k) * size;
                        var terrianChunk = getOrCreateTerrianChunk(origin);
                    }
                }
            }

            foreach(var kv in map) {
                var terrianChunk = kv.Value;
                GenerateGround(terrianChunk);
            }

            var drawDis = config.generateDis;

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);

                if (terrianChunk.Distance < drawDis)
                {
                    var origin = terrianChunk.Origin;
                    defaultLayer.GetChunk(origin).UpdateNormals();

                    GenerateWaters(terrianChunk);

                    GenerateGrass(terrianChunk);

                    GenerateTrees(terrianChunk);

                    terrianChunk.UpdateRoutes();

                    generateShadows(terrianChunk);

                    //GenerateEnemies(terrianChunk);
                }
            }

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < drawDis)
                {
                    //generateHouses(terrianChunk);
                }
            }

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < drawDis)
                {
                    Profiler.BeginSample("Meshing");

                    foreach(var chunks in chunksToDraw) {
                        Draw(chunks, terrianChunk.Origin, Transform, material, terrianChunk);    
                    }

                    Profiler.EndSample();
                }
            }
        }

        private void generateGrowth(TerrianChunk terrianChunk) {
            if (!terrianChunk.growthNeedsUpdate) {
                return;
            }

            terrianChunk.growthNeedsUpdate = false;
        }

        public TerrianChunk GetTerrianChunk(Vector3Int origin) {
            TerrianChunk terrianChunk = null;
            map.TryGetValue(origin, out terrianChunk);
            return terrianChunk;
        }

        TerrianChunk getOrCreateTerrianChunk(Vector3Int origin)
        {
            if (map.ContainsKey(origin))
            {
                return map[origin];
            }

            Vector3Int key = new Vector3Int(origin.x / size, origin.y / size, origin.z / size);
            map[origin] = new TerrianChunk(key, size);
            map[origin].Config = config;
            map[origin].Terrian = this;
            return map[origin];
        }

        private bool CheckShadow(Vector3Int key, int i, int j, int k) {
            key.x += i;
            key.y += j;
            key.z += k;
            var origin = key * size;

            var ready = !getOrCreateTerrianChunk(origin).rockNeedsUpdate;
            return ready;
        }

        private bool ShouldGenerateShadows(TerrianChunk terrianChunk) {
            var key = terrianChunk.key;

            var ready = true;
            for (var j = 0; j < config.maxChunksY - key.y; j ++)  {
                ready &= CheckShadow(key, 0, j, 0);
                ready &= CheckShadow(key, -1, j, 0);
                ready &= CheckShadow(key, 0, j, -1);
                ready &= CheckShadow(key, -1, j, -1);
            }
            return ready;
        }

        private void generateShadows(TerrianChunk terrianChunk)
        {
            
            if (!terrianChunk.shadowsNeedsUpdate)
            {
                return;
            }

            if (!ShouldGenerateShadows(terrianChunk)) {
                return;
            }

            var chunk = terrianChunk.Chunk;
            var origin = chunk.Origin;
            var treeChunk = treeLayer.GetChunk(origin);

            foreach (var chunks in chunksToDraw) {
                var c = chunks.GetChunk(origin);
                if (c != null) {
                    c.UpdateShadows(chunksCastingShadows);
                }
            }

            terrianChunk.shadowsNeedsUpdate = false;
        }

        private void generateHouses(TerrianChunk terrianChunk)
        {
            //if (terrianChunk.GeneratedHouses)
            //{
            //    return;
            //}
            //var house = new House(3, 2, 5);
            //print(house.Shape, new Vector3Int(0, 20, 0), defaultLayer.Chunks, new Vector3Int());
            //terrianChunk.GeneratedHouses = true;
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

        public void SpawnDwarfs() {
            var origin = new Vector3Int(0, 0, 0);
            var terrianChunk = map[origin];
            var node = terrianChunk.Routes.GetNodeCloseTo(new Vector3Int(size / 2, 0, size / 2));

            if (node != null) {
                //var house = new House(3, 2, 5);
                //print(house.Shape, node.Value, defaultLayer.Chunks, new Vector3Int());
                //GameObject go = new GameObject("guy");
                //var actor = go.AddComponent<Actor>();
                //actor.terrian = this;
                //actor.SetNode(node.Value);
            }
        }

        public HashSet<Routes.Connection> GetNextNodes(Vector3Int node) {
            var origin = getOrigin(node.x, node.y, node.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null) {
                return new HashSet<Routes.Connection>();
            }
            var routesMap = terrianChunk.Routes.Map;
            if (routesMap.ContainsKey(node)) {
                return routesMap[node];
            }

            return new HashSet<Routes.Connection>();
        }

        public Vector3Int getOrigin(int i, int j, int k)
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
            var origin = getOrigin(coord.x, coord.y, coord.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null) {
                return false;
            }
            return terrianChunk.GetWater(coord - terrianChunk.Origin);
        }

        public void Draw(Chunks chunks, Vector3Int origin, Transform transform, Material material, TerrianChunk terrianChunk)
        {
            if (!chunks.HasChunk(origin))
            {
                return;
            }

            var chunk = chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return;
            }

            if (chunk.Mesh != null)
            {
                UnityEngine.Object.Destroy(chunk.Mesh);
                UnityEngine.Object.Destroy(chunk.GameObject);
            }

            //Mesh mesh = new Mesh();
            //VoxelMesher.Mesh(chunk, mesh, terrianChunk);

            var mesh = VoxelMesher.MeshGPU(chunk, terrianChunk);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = chunk.Origin;

            chunk.Mesh = mesh;
            chunk.GameObject = go;
            chunk.Dirty = false;
        }
    }
}
