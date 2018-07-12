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
    private int maxChunksY = 4;
    private int generateDis = 3;
    private int drawDis = 2;
    private int maxHeight = 64;
    private Material material = new Material(Shader.Find("Custom/voxel"));
    public Transform Transform;
    public Vector3 Target;

    private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();
    Perlin heightNoise = new Perlin();
    MarchingCubes marchingCubes = new MarchingCubes();
    Color grass;
    Color rock;
    Color water;

    private int waterLevel = 2;

    public Terrian(int size = 32) {
        this.size = size;
        sizeF = size;
        chunks = new Chunks(size);
        heightNoise.OctaveCount = 5;
        ColorUtility.TryParseHtmlString("#63912C", out grass);
        ColorUtility.TryParseHtmlString("#727D75", out rock);
        ColorUtility.TryParseHtmlString("#6F7993", out water);
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
                generateGrass(terrianChunk);
            }
        }

        foreach (var kv in map)
        {
            var terrianChunk = kv.Value;
            if (terrianChunk.Distance < drawDis)
            {
                Mesher.MeshChunk(terrianChunk.Chunk, chunks, Transform, material);
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
                            chunk.SetColor(i, j, k, water);
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
        for (var i = 0; i < chunk.Size; i++)
        {
            for (var j = 0; j < chunk.Size; j++)
            {
                for (var k = 0; k < chunk.Size; k++)
                {
                    var normal = marchingCubes.GetNormal(i, j, k, chunk);
                    if (normal.HasValue)
                    {
                        if (Vector3.Angle(Vector3.up, normal.Value) < 0) {
                            Debug.Log("Unexpected");
                        }

                        if (Vector3.Angle(Vector3.up, normal.Value) < 60)
                        {
                            chunk.SetColor(i, j, k, grass);
                        }
                    }
                }
            }
        }
        terrianChunk.GeneratedGrass = true;
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
                    chunk.SetColor(i, j, k, rock);
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
        map[origin] = new TerrianChunk(key);
       return map[origin];
    }

    class TerrianChunk {
        private Vector3Int key;
        public bool generated = false;
        public bool GeneratedWater = false;
        public bool GeneratedGrass = false;
        private int distance;
        public Chunk Chunk;

        public int Distance
        {
            get
            {
                return distance;
            }
        }

        public TerrianChunk(Vector3Int key) {
            this.key = key;
        }

        public void UpdateDistance(int x, int z) {
            var xDis = Mathf.Abs(x - this.key.x);
            var zDis = Mathf.Abs(z - this.key.z);
            distance = Mathf.Max(xDis, zDis);
        }
    }
}
