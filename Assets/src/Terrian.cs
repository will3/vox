using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Generator;
using System;

public class Terrian
{
    private int size;
    private float sizeF;
    private Chunks chunks;
    private Chunks treeLayer;

    private int maxChunksY = 4;
    private int generateDis = 3;
    private int drawDis = 2;
    private int maxHeight = 64;
    private int waterLevel = 2;
    private int minTreeJ = 4;

    private Material material = new Material(Shader.Find("Unlit/voxelunlit"));
    public Transform Transform;
    public Vector3 Target;

    private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();
    MarchingCubes marchingCubes = new MarchingCubes();
    Perlin heightNoise = new Perlin();
    Perlin grassNoise = new Perlin();
    Perlin treeNoise = new Perlin();

    public Terrian(int size = 32) {
        this.size = size;
        sizeF = size;
        chunks = new Chunks(size);
        treeLayer = new Chunks(size);
        heightNoise.OctaveCount = 5;
    }

    public void Update() {
        int x = Mathf.FloorToInt(Target.x / sizeF);
        int z = Mathf.FloorToInt(Target.z / sizeF);

        for (int j = 0; j < maxChunksY; j++)
        {
            for (int i = x - generateDis; i <= x + generateDis; i++)
            {
                for (int k = z - generateDis; k <= z + generateDis; k++)
                {
                    var origin = new Vector3Int(i, j, k) * size;
                    var terrianChunk = generateRock(origin);
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
                marchingCubes.AccurateOffset = false;
                //marchingCubes.UseLighting = true;
                Mesher.MeshChunk(terrianChunk.Chunk, chunks, Transform, material, marchingCubes);

                //marchingCubes.UseLighting = false;
				marchingCubes.AccurateOffset = false;
                var treeChunk = treeLayer.GetChunk(terrianChunk.Origin);
                Mesher.MeshChunk(treeChunk, chunks, Transform, material, marchingCubes);
            }
        }
        
    }

    private void generateWater(TerrianChunk terrianChunk) {
        if (terrianChunk.GeneratedWater) {
            return;
        }

        var chunk = terrianChunk.Chunk;
        if (chunk.Origin.y < waterLevel) {
            float maxJ = waterLevel - chunk.Origin.y;
            if (maxJ > chunk.Size) {
                maxJ = chunk.Size;
            }
            for (var i = 0; i < chunk.Size; i++) {
                for (var k = 0; k < chunk.Size; k++) {
                    for (var j = 0; j < maxJ; j++) {
                        if (chunk.Get(i, j, k) <= 0.5) {
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

    private void generateGrass(TerrianChunk terrianChunk) {
        if (terrianChunk.GeneratedGrass) {
            return;
        }

        var chunk = terrianChunk.Chunk;

        foreach(var kv in terrianChunk.Normals) {
            var coord = kv.Key;
            var normal = kv.Value;
            if (terrianChunk.GetWater(coord.x, coord.y, coord.z))
            {
                continue;
            }

            var absJ = coord.y + chunk.Origin.y;

            if (absJ < minTreeJ) {
                continue;
            }

            var angle = Vector3.Angle(Vector3.up, normal);
            var up = Mathf.Cos(angle * Mathf.Deg2Rad);
            if (up < 0.5)
            {
                continue;
            }

            chunk.SetColor(coord.x, coord.y, coord.z, Colors.grass);
        }

        terrianChunk.GeneratedGrass = true;
    }

    private void generateTrees(TerrianChunk terrianChunk) {
        if (terrianChunk.GeneratedTrees) {
            return;
        }

        var smallCone = new Cone(2.0f, 8);
        var bigCone = new Cone(2.5f, 10);

        var treeChunk = treeLayer.GetOrCreateChunk(terrianChunk.Origin);

        foreach(var kv in terrianChunk.Normals) {
            var coord = kv.Key;
            var normal = kv.Value;
            var i = coord.x;
            var j = coord.y;
            var k = coord.z;
            var vector = new Vector3(coord.x, coord.y, coord.z);

            var absJ = j + treeChunk.Origin.y;
            if (absJ < minTreeJ) {
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

            var clamp = 0.5f;
            if (value > clamp) {
                value = clamp;
            }
            value /= clamp;
            value = 1.0f - value;

            // dis 2 - 6
            var minTreeDis = 4.0f * value + 2.0f;

            if (!smallTree) {
                minTreeDis *= 1.5f;
            }

            if (terrianChunk.HasTreeCloseBy(coord, minTreeDis))
            {
                continue;
            }

            var cone = smallTree ? smallCone : bigCone;
            var shape = cone.Shape;

            var halfW = (shape.Width - 1) / 2;
            for (var it = 0; it < shape.Width; it++)
            {
                for (var jt = 0; jt < shape.Height; jt++)
                {
                    for (var kt = 0; kt < shape.Depth; kt++)
                    {
                        var x = i + it - halfW;
                        var y = j + jt;
                        var z = k + kt - halfW;

                        var density = shape.Get(it, jt, kt) - UnityEngine.Random.Range(0.0f, 0.1f);

                        treeChunk.SetIfHigherGlobal(x, y, z, density);
                        treeChunk.SetColorGlobal(x, y, z, Colors.leaf);


                    }
                }
            }

            terrianChunk.SetTree(coord, true);
        }

        terrianChunk.GeneratedTrees = true;
    }

    float getVoxel(int i, int j, int k) {
        var height = (1f - j / (float)maxHeight) - 0.5f;
        var value = height + (float)heightNoise.GetValue(new Vector3(i, j * 0.4f, k) * 0.015f);
        return value;
    }

    TerrianChunk generateRock(Vector3Int origin) {
        TerrianChunk terrianChunk = getTerrianChunk(origin);
        if (terrianChunk.generated) {
            return terrianChunk;
        }

        var chunk = chunks.GetOrCreateChunk(origin);

        var halfSize = size / 2 + 1;

        var field = new Field(halfSize);

        for (var i = 0; i < halfSize; i++) {
            for (var j = 0; j < halfSize; j++) {
                for (var k = 0; k < halfSize; k++) {
                    float value = getVoxel(i * 2 + chunk.Origin.x, j * 2 + chunk.Origin.y, k * 2 + chunk.Origin.z);
                    field.Set(i, j, k, value);
                }
            }
        }

        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j ++) {
                for (var k = 0; k < size; k++) {
                    float value = field.Sample(i / 2.0f, j / 2.0f, k / 2.0f);
                    chunk.Set(i, j, k, value);
                    chunk.SetColor(i, j, k, Colors.rock);
                }
            }
        }

        terrianChunk.generated = true;
        terrianChunk.Chunk = chunk;

        return terrianChunk;
    }

    TerrianChunk getTerrianChunk(Vector3Int origin) {
        if (map.ContainsKey(origin)) {
            return map[origin];
        }

        Vector3Int key = new Vector3Int(origin.x / size, origin.y / size, origin.z / size);
        map[origin] = new TerrianChunk(key, size);
       return map[origin];
    }

    private void generateNormals(TerrianChunk terrianChunk) {
        if (terrianChunk.GeneratedNormals) {
            return;
        }
        var chunk = terrianChunk.Chunk;
        for (var i = 0; i < chunk.Size; i++)
        {
            for (var j = 0; j < chunk.Size; j++)
            {
                for (var k = 0; k < chunk.Size; k++)
                {
                    var normal = marchingCubes.GetNormal(i, j, k, chunk);
                    if (normal.HasValue)
                    {
                        var coord = new Vector3Int(i, j, k);
                        terrianChunk.SetNormal(coord, normal.Value);
                    }
                }
            }
        }
        terrianChunk.GeneratedNormals = true;
    }

    private void generateShadows(TerrianChunk terrianChunk) {
        if (terrianChunk.GeneratedShadows) {
            return;
        }

        var chunk = terrianChunk.Chunk;
        var treeChunk = treeLayer.GetChunk(chunk.Origin);
        var origin = chunk.Origin;
        foreach(var kv in terrianChunk.Normals) {
            var coord = kv.Key;
            var globalCoord = coord + origin;
            var shadow = calculateShadow(globalCoord);
            chunk.SetLighting(coord, shadow);
            //treeChunk.SetLighting(coord, shadow);
        }

        terrianChunk.GeneratedShadows = true;
    }

    private float calculateShadow(Vector3Int coord) {
        var shadowStrength = 0.4f;
        var result = Raycast4545.Trace(coord, chunks) || Raycast4545.Trace(coord, treeLayer);
        if (result) {
            return 1 - shadowStrength;
        } else {
            return 1.0f;
        }
    }

    class TerrianChunk {
        private readonly Dictionary<Vector3Int, Vector3> normals = new Dictionary<Vector3Int, Vector3>();

        public IDictionary<Vector3Int, Vector3> Normals
        {
            get
            {
                return normals;
            }
        }

        private readonly HashSet<int> waters = new HashSet<int>();

        private Vector3Int key;
        public bool generated = false;
        public bool GeneratedWater = false;
        public bool GeneratedGrass = false;
        public bool GeneratedTrees = false;
        public bool GeneratedNormals = false;
        public bool GeneratedShadows = false;

        private int distance;
        private Vector3Int origin;

        public Vector3Int Origin
        {
            get
            {
                return origin;
            }
        }

        public Chunk Chunk;
        private readonly HashSet<Vector3Int> trees = new HashSet<Vector3Int>();

        private int size;

        public int Distance
        {
            get
            {
                return distance;
            }
        }

        public TerrianChunk(Vector3Int key, int size) {
            this.key = key;
            this.origin = key * size;
            this.size = size;
        }

        public void UpdateDistance(int x, int z) {
            var xDis = Mathf.Abs(x - this.key.x);
            var zDis = Mathf.Abs(z - this.key.z);
            distance = Mathf.Max(xDis, zDis);
        }

        public void SetTree(Vector3Int coord, bool flag)
        {
            if (flag)
            {
                trees.Add(coord);
            }
            else
            {
                trees.Remove(coord);
            }
        }

        public bool GetTree(Vector3Int coord)
        {
            return trees.Contains(coord);
        }

        public void SetNormal(Vector3Int coord, Vector3 normal)
        {
            normals[coord] = normal;
        }

        public Vector3 GetNormal(Vector3Int coord) {
            return normals[coord];
        }

        public void SetWater(int i, int j, int k, bool flag)
        {
            var index = getIndex(i, j, k);
            if (flag)
            {
                waters.Add(index);
            }
            else
            {
                waters.Remove(index);
            }
        }

        public bool GetWater(int i, int j, int k)
        {
            var index = getIndex(i, j, k);
            return waters.Contains(index);
        }

        private int getIndex(int i, int j, int k)
        {
            int index = i * size * size + j * size + k;
            return index;
        }

        public bool HasTreeCloseBy(Vector3Int from, float minTreeDis) {
            foreach (var coord in trees) {
                if ((coord - from).magnitude < minTreeDis) {
                    return true;
                }
            }
            return false;
        }
    }
}
