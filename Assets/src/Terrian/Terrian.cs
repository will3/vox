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

        private Layer defaultLayer;
        private Layer treeLayer;

        private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
        public Transform Transform;
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

        MarchingCubes marchingCubes = new MarchingCubes();


        public Terrian(int size = 32)
        {
            this.size = size;
            sizeF = size;

            defaultLayer = new Layer(size);
            treeLayer = new Layer(size);
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
                        generateRock(terrianChunk);
                    }
                }
            }

            var drawDis = config.generateDis;

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);

                if (terrianChunk.Distance < drawDis)
                {
                    var origin = terrianChunk.Origin;
                    defaultLayer.Chunks.GetChunk(origin).UpdateNormals();

                    //terrianChunk.GenerateWaters();

                    //Grass.Generate(terrianChunk, config);

                    //generateTrees(terrianChunk);

                    //terrianChunk.UpdateRoutes();

                    //generateTowns(terrianChunk);

                    //generateShadows(terrianChunk);
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

                    defaultLayer.Draw(terrianChunk.Origin, Transform, material, terrianChunk);
                    //treeLayer.Draw(terrianChunk.Origin, Transform, material, terrianChunk);

                    Profiler.EndSample();
                }
            }
        }

        private void generateTowns(TerrianChunk terrianChunk) {
            if (!terrianChunk.townPointsNeedsUpdate) {
                return;
            }

            var chunk = terrianChunk.Chunk;
            chunk.UpdateSurfaceCoords();
            chunk.UpdateNormals();

            var origin = chunk.Origin;

            var townField = new Field(chunk.Size);
            var generator = new TownGenerator(config);
            var townRandom = config.townRandom;

            townField.Generate(generator, origin);

            foreach(var coord in chunk.surfaceCoords) {
                var r = townRandom.NextDouble();
                if (r > 0.001)
                {
                    continue;
                }

                var normal = chunk.GetNormal(coord);
                if (normal == null) {
                    continue;
                }

                var dot = Vector3.Dot(normal.Value, Vector3.up);
                if (dot < 0.5f) {
                    continue;
                }

                var water = terrianChunk.GetWater(coord.x, coord.y, coord.z);
                if (water) {
                    continue;
                }

                var absY = coord.y + origin.y;
                var height = - absY / config.maxHeight;

                var n = townField.Sample(coord.x, coord.y, coord.z);
                var value = n + height;

                if (value > 0) {
                    terrianChunk.AddTownPoint(coord);
                    chunk.SetColor(coord.x, coord.y, coord.z, Colors.special);
                }
            }

            terrianChunk.townPointsNeedsUpdate = false;
        }

        private void generateGrowth(TerrianChunk terrianChunk) {
            if (!terrianChunk.growthNeedsUpdate) {
                return;
            }

            terrianChunk.growthNeedsUpdate = false;
        }

        private void generateTrees(TerrianChunk terrianChunk)
        {
            var minTreeJ = config.minTreeJ;
            var treeNoise = config.treeNoise;

            if (!terrianChunk.treesNeedsUpdate)
            {
                return;
            }

            var pine = new Pine(3.0f, 10, 2);

            var chunk = terrianChunk.Chunk;

            foreach (var kv in chunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                var i = coord.x;
                var j = coord.y;
                var k = coord.z;

                var absJ = j + chunk.Origin.y;
                if (absJ < minTreeJ)
                {
                    continue;
                }

                if (terrianChunk.GetWater(i, j, k))
                {
                    continue;
                }

                var angle = Vector3.Angle(Vector3.up, normal);
                var up = Mathf.Cos(angle * Mathf.Deg2Rad);
                if (up < 0.7f)
                {
                    continue;
                }

                Vector3 globalCoord = coord + chunk.Origin;
                var value = (float)treeNoise.GetValue(globalCoord);

                var otherTrees = terrianChunk.GetOtherTrees(coord);

                value -= otherTrees * 4.0f;

                if (value < 0.4f) { continue; }

                print(pine.GetShape(), coord + chunk.Origin, treeLayer.Chunks, pine.Offset);

                terrianChunk.SetTree(coord, true);
            }

            var treeChunk = treeLayer.Chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null) {
                treeChunk.UpdateSurfaceCoords();    
            }

            terrianChunk.treesNeedsUpdate = false;
        }

        void generateRock(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.rockNeedsUpdate)
            {
                return;
            }

            if (terrianChunk.Chunk == null)
            {
                terrianChunk.Chunk = defaultLayer.Chunks.GetOrCreateChunk(terrianChunk.Origin);
            }
            var chunk = terrianChunk.Chunk;

            var field = new Field(size);
            var generator = new HeightGenerator(config);
            field.Generate(generator, chunk.Origin);

            for (var i = 0; i < chunk.dataSize; i++)
            {
                for (var j = 0; j < chunk.dataSize; j++)
                {
                    for (var k = 0; k < chunk.dataSize; k++)
                    {
                        float value = field.Sample(i, j, k);
                        chunk.Set(i, j, k, value);
                        if (value > 0) {
                            chunk.SetColor(i, j, k, Colors.rock);    
                        }
                    }
                }
            }

            terrianChunk.rockNeedsUpdate = false;
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

        private void generateShadows(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.shadowsNeedsUpdate)
            {
                return;
            }

            var chunk = terrianChunk.Chunk;
            var origin = chunk.Origin;
            var treeChunk = treeLayer.Chunks.GetChunk(origin);

            var chunksList = new Chunks[] { defaultLayer.Chunks, treeLayer.Chunks };
            chunk.UpdateShadows(chunksList);

            if (treeChunk != null) {
                treeChunk.UpdateShadows(chunksList);
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
            var list = new Chunks[] { defaultLayer.Chunks, defaultLayer.Chunks };
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
    }
}
