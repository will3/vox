using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise.Generator;

public class Terrian
{
    private int size;
    private float sizeF;
    private Chunks chunks;
    private int maxChunksY = 2;
    private int generateDis = 2;
    private Dictionary<Vector3Int, TerrianChunk> map = new Dictionary<Vector3Int, TerrianChunk>();
    Perlin heightNoise = new Perlin();

    public Terrian(Chunks chunks) {
        this.size = chunks.Size;
        this.chunks = chunks;
        sizeF = chunks.Size;
        heightNoise.OctaveCount = 5;
    }

    public void GenerateForCamera(Vector3 cameraTarget) {
        int x = Mathf.FloorToInt(cameraTarget.x / sizeF);
        int z = Mathf.FloorToInt(cameraTarget.z / sizeF);
        for (int j = 0; j < maxChunksY; j ++) {
            for (int i = x - generateDis; i <= x + generateDis; i++) {
                for (int k = z - generateDis; k <= z + generateDis; k ++) {
                    var origin = new Vector3Int(i, j, k) * size;
                    generate(origin);
                }
            }
        }
    }

    float getVoxel(int i, int j, int k) {
        // TODO sample this
        var value = (float)heightNoise.GetValue(new Vector3(i, j, k) * 0.01f);
        return value;
    }

    void generate(Vector3Int origin) {
        TerrianChunk terrianChunk = getTerrianChunk(origin);
        if (terrianChunk.generated) {
            return;
        }

        var chunk = chunks.GetOrCreateChunk(origin);

        for (var i = 0; i < chunk.Size; i++) {
            for (var j = 0; j < chunk.Size; j ++) {
                for (var k = 0; k < chunk.Size; k++) {
                    float value = getVoxel(i + chunk.Origin.x, j + chunk.Origin.y, k + chunk.Origin.z);
                    chunk.Set(i, j, k, value);          
                }
            }
        }

        terrianChunk.generated = true;
    }

    TerrianChunk getTerrianChunk(Vector3Int origin) {
        if (map.ContainsKey(origin)) {
            return map[origin];
        }

        map[origin] = new TerrianChunk();
       return map[origin];
    }

    class TerrianChunk {
        public bool generated = false;
    }
}
