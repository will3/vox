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
        private int generateDis = 3;
        private int drawDis = 2;
        private int waterLevel = 2;
        private int minTreeJ = 1;
        //private int maxHeight = 64;

        private float terrainHeight = 64;
        private float maxTreeDis = 16.0f;
        private float minTreeDis = 8.0f;

        private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
        public Transform Transform;
        public Vector3 Target;

        private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();
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
                        var terrianChunk = getTerrianChunk(origin);
                        generateRock(terrianChunk);
                        generateWater(terrianChunk);
                    }
                }
            }

            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                terrianChunk.UpdateDistance(x, z);

                if (terrianChunk.Distance < drawDis)
                {
                    generateNormals(terrianChunk);
                    generateGrass(terrianChunk);
                    generateTrees(terrianChunk);
                    generateShadows(terrianChunk);
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
            if (terrianChunk.GeneratedWater)
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

            terrianChunk.GeneratedWater = true;
        }

        private void generateGrass(TerrianChunk terrianChunk)
        {
            if (terrianChunk.GeneratedGrass)
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
                if (upDot < 0.5)
                {
                    continue;
                }

                chunk.SetColor(coord.x, coord.y, coord.z, Colors.grass);
            }

            terrianChunk.GeneratedGrass = true;
        }

        private void generateTrees(TerrianChunk terrianChunk)
        {
            if (terrianChunk.GeneratedTrees)
            {
                return;
            }

            var smallPine = new Pine(2.0f, 7);

            var chunk = terrianChunk.Chunk;

            foreach (var kv in terrianChunk.Normals)
            {
                var coord = kv.Key;
                var normal = kv.Value;
                var i = coord.x;
                var j = coord.y;
                var k = coord.z;
                var vector = new Vector3(coord.x, coord.y, coord.z);

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

                var value = (float)treeNoise.GetValue(vector * 0.04f);

                if (value < 0)
                {
                    continue;
                }

                var smallTree = value < 0.4;

                var clamp = 1.0f;
                if (value > clamp)
                {
                    value = clamp;
                }
                value /= clamp;
                value = 1.0f - value;

                var treeDis = (maxTreeDis - minTreeDis) * value + minTreeDis;

                if (!smallTree)
                {
                    treeDis *= 1.5f;
                }

                if (terrianChunk.HasTreeCloseBy(coord, treeDis))
                {
                    continue;
                }

                var pine = smallPine;

                print(pine.Shape, coord + chunk.Origin, treeLayer.Chunks, pine.Offset);

                terrianChunk.SetTree(coord, true);
            }

            var treeChunk = treeLayer.Chunks.GetChunk(terrianChunk.Origin);
            if (treeChunk != null) {
                treeChunk.UpdateSurfaceCoords();    
            }

            terrianChunk.GeneratedTrees = true;
        }

        float getVoxel(int i, int j, int k)
        {
            var height = (1f - j / (float)terrainHeight) - 0.5f;
            var value = height + (float)heightNoise.GetValue(new Vector3(i, j * 0.4f, k) * 0.015f);
            return value;
        }

        TerrianChunk generateRock(TerrianChunk terrianChunk)
        {
            if (terrianChunk.generated)
            {
                return terrianChunk;
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

            terrianChunk.generated = true;

            return terrianChunk;
        }

        TerrianChunk getTerrianChunk(Vector3Int origin)
        {
            if (map.ContainsKey(origin))
            {
                return map[origin];
            }

            Vector3Int key = new Vector3Int(origin.x / size, origin.y / size, origin.z / size);
            map[origin] = new TerrianChunk(key, size);
            return map[origin];
        }

        private void generateNormals(TerrianChunk terrianChunk)
        {
            if (terrianChunk.GeneratedNormals)
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

            terrianChunk.GeneratedNormals = true;
        }

        private void generateShadows(TerrianChunk terrianChunk)
        {
            if (terrianChunk.GeneratedShadows)
            {
                return;
            }

            var chunk = terrianChunk.Chunk;
            var origin = chunk.Origin;

            foreach (var coord in chunk.SurfaceCoords)
            {
                var globalCoord = coord + origin;
                var shadow = calculateShadow(globalCoord);
                chunk.SetLighting(coord, shadow);
            }

            var treeChunk = treeLayer.Chunks.GetChunk(origin);
            if (treeChunk != null) {
                foreach (var coord in treeChunk.SurfaceCoords)
                {
                    var globalCoord = coord + origin;
                    var shadow = calculateShadow(globalCoord);
                    treeChunk.SetLighting(coord, shadow);
                }    
            }

            terrianChunk.GeneratedShadows = true;
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

        private float calculateShadow(Vector3Int coord)
        {
            var shadowStrength = 0.4f;
            var result = Raycast4545.Trace(coord, defaultLayer.Chunks) || Raycast4545.Trace(coord, treeLayer.Chunks);
            if (result)
            {
                return 1 - shadowStrength;
            }
            else
            {
                return 1.0f;
            }
        }

        private void SpawnDwarfs() {
            
        }
    }
}
