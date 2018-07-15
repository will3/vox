using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Generator;
using System;

namespace FarmVox
{
    public class Terrian
    {
        private int size;
        private float sizeF;

        private Layer defaultLayer;
        private Layer treeLayer;

        private int maxChunksY = 4;
        private int generateDis = 2;
        private int drawDis = 2;
        private int waterLevel = 2;
        private int minTreeJ = 1;
        //private int maxHeight = 64;

        private float hillHeight = 64;
        private float plainHeight = 12;

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

        MarchingCubes marchingCubes = new MarchingCubes();
        Perlin heightNoise = new Perlin();
        Perlin grassNoise = new Perlin();
        Perlin treeNoise = new Perlin();
        Perlin biomeNoise = new Perlin();

        public Terrian(int size = 32)
        {
            this.size = size;
            sizeF = size;

            defaultLayer = new Layer(size);
            treeLayer = new Layer(size);

            heightNoise.OctaveCount = 5;
            grassNoise.OctaveCount = 5;

            treeNoise.Frequency = 0.05f;
            treeNoise.OctaveCount = 5;
            biomeNoise.Frequency = 0.01f;
        }

        public void Update()
        {
            int x = Mathf.FloorToInt(Target.x / sizeF);
            int z = Mathf.FloorToInt(Target.z / sizeF);

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

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);

                if (terrianChunk.Distance < drawDis)
                {
                    generateWater(terrianChunk);
                    generateNormals(terrianChunk);
                    generateGrass(terrianChunk);
                    //generateTrees(terrianChunk);
                    //generateShadows(terrianChunk);
                    //terrianChunk.UpdateRoutes();
                }
            }

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < drawDis)
                {
                    generateHouses(terrianChunk);
                }
            }

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < drawDis)
                {
                    defaultLayer.Draw(terrianChunk.Origin, Transform, material);
                    treeLayer.Draw(terrianChunk.Origin, Transform, material);
                }
            }
        }

        private void generateWater(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.waterNeedsUpdate)
            {
                return;
            }

            var chunk = terrianChunk.Chunk;
            if (chunk.Origin.y < waterLevel)
            {
                float maxJ = waterLevel - chunk.Origin.y;
                if (maxJ > chunk.Size)
                {
                    maxJ = chunk.Size;
                }
                for (var i = 0; i < chunk.Size; i++)
                {
                    for (var k = 0; k < chunk.Size; k++)
                    {
                        for (var j = 0; j < maxJ; j++)
                        {
                            if (chunk.Get(i, j, k) <= 0.5)
                            {
                                chunk.Set(i, j, k, 1);
                                chunk.SetColor(i, j, k, Colors.water);
                                terrianChunk.SetWater(i, j, k, true);
                            }
                        }
                    }
                }
            }

            terrianChunk.waterNeedsUpdate = false;
        }

        private void generateGrass(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.grassNeedsUpdate)
            {
                return;
            }

            var chunk = terrianChunk.Chunk;

            foreach (var kv in terrianChunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                if (terrianChunk.GetWater(coord.x, coord.y, coord.z))
                {
                    continue;
                }

                var absJ = coord.y + chunk.Origin.y;

                if (absJ < minTreeJ)
                {
                    continue;
                }

                var upDot = Vector3.Dot(Vector3.up, normal);

                Vector3 globalCoord = coord + chunk.Origin;

                var value = grassNoise.GetValue(globalCoord * 0.05f) * 0.8f;

                if (upDot + value < 0.5)
                {
                    continue;
                }

                chunk.SetColor(coord.x, coord.y, coord.z, Colors.grass);
            }

            terrianChunk.grassNeedsUpdate = false;
        }

        private void generateTrees(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.treesNeedsUpdate)
            {
                return;
            }

            var pine = new Pine(2.0f, 7);

            var chunk = terrianChunk.Chunk;

            foreach (var kv in terrianChunk.Normals)
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

                print(pine.Shape, coord + chunk.Origin, treeLayer.Chunks, pine.Offset);

                terrianChunk.SetTree(coord, true);
            }

            var treeChunk = treeLayer.Chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null) {
                treeChunk.UpdateSurfaceCoords();    
            }

            terrianChunk.treesNeedsUpdate = false;
        }

        float getVoxel(int i, int j, int k)
        {
            var biome = biomeNoise.GetValue(new Vector3(i, j * 0.4f, k));
            float terrainHeight;
            if (biome > 0) {
                terrainHeight = hillHeight;    
            } else {
                terrainHeight = plainHeight;
            }

            var height = (1f - j / (float)terrainHeight) - 0.5f;
            var value = height + (float)heightNoise.GetValue(new Vector3(i, j * 0.4f, k) * 0.015f);
            return value;
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

            var halfSize = size / 2 + 1;

            var field = new Field(halfSize);

            for (var i = 0; i < halfSize; i++)
            {
                for (var j = 0; j < halfSize; j++)
                {
                    for (var k = 0; k < halfSize; k++)
                    {
                        float value = getVoxel(i * 2 + chunk.Origin.x, j * 2 + chunk.Origin.y, k * 2 + chunk.Origin.z);
                        field.Set(i, j, k, value);
                    }
                }
            }

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    for (var k = 0; k < size; k++)
                    {
                        float value = field.Sample(i / 2.0f, j / 2.0f, k / 2.0f);
                        chunk.Set(i, j, k, value);
                        chunk.SetColor(i, j, k, Colors.rock);
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
            map[origin].Terrian = this;
            return map[origin];
        }

        private void generateNormals(TerrianChunk terrianChunk)
        {
            if (!terrianChunk.normalsNeedsUpdate)
            {
                return;
            }
            var chunk = terrianChunk.Chunk;

            var origin = terrianChunk.Origin;
            var treeChunk = treeLayer.Chunks.GetChunk(origin);

            chunk.UpdateSurfaceCoords();

            foreach (var coord in chunk.SurfaceCoords)
            {
                var normal = marchingCubes.GetNormal(coord.x, coord.y, coord.z, chunk);
                terrianChunk.SetNormal(coord, normal.Value);
            }

            terrianChunk.normalsNeedsUpdate = false;
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
                GameObject go = new GameObject("guy");
                var actor = go.AddComponent<Actor>();
                actor.terrian = this;
                actor.SetNode(node.Value);
            }
        }

        public HashSet<Vector3Int> GetNextNodes(Vector3Int node) {
            var origin = getOrigin(node.x, node.y, node.z);
            var terrianChunk = GetTerrianChunk(origin);
            if (terrianChunk == null) {
                return new HashSet<Vector3Int>();
            }
            var routesMap = terrianChunk.Routes.Map;
            if (routesMap.ContainsKey(node)) {
                return routesMap[node];
            }

            return new HashSet<Vector3Int>();
        }

        private Vector3Int getOrigin(int i, int j, int k)
        {
            return new Vector3Int(
                Mathf.FloorToInt(i / this.sizeF) * this.size,
                Mathf.FloorToInt(j / this.sizeF) * this.size,
                Mathf.FloorToInt(k / this.sizeF) * this.size
            );
        }

        public RaycastResult Trace(Vector3 pos, Vector3 dir, int maxD) {
            var list = new Chunks[] { defaultLayer.Chunks, treeLayer.Chunks };
            return Raycast.Trace(list, pos, dir, maxD);
        }
    }
}
