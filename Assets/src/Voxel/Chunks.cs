using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunks
{
    private float sizeF;
    private int size;

    public int Size
    {
        get
        {
            return size;
        }
    }

    private Dictionary<Vector3Int, Chunk> map = new Dictionary<Vector3Int, Chunk>();

    public Dictionary<Vector3Int, Chunk> Map
    {
        get
        {
            return map;
        }
    }

    public Chunks(int size) {
        this.size = size;
        this.sizeF = size;
    }

    private Vector3Int getOrigin(int i, int j, int k)
    {
        return new Vector3Int(
            Mathf.FloorToInt(i / this.sizeF) * this.size,
            Mathf.FloorToInt(j / this.sizeF) * this.size,
            Mathf.FloorToInt(k / this.sizeF) * this.size
        );
    }

    public float Get(int i, int j, int k)
    {
        var origin = getOrigin(i, j, k);
        if (!map.ContainsKey(origin))
        {
            return 0;
        }

        return map[origin].Get(i - origin.x, j - origin.y, k - origin.z);
    }

    public float GetLighting(int i, int j, int k) {
        var origin = getOrigin(i, j, k);
        if (!map.ContainsKey(origin))
        {
            return 1.0f;
        }

        return map[origin].GetLighting(i - origin.x, j - origin.y, k - origin.z);
    }

    public Color GetColor(int i, int j, int k) {
        var origin = getOrigin(i, j, k);
        if (!map.ContainsKey(origin))
        {
            return default(Color);
        }
        return map[origin].GetColor(i - origin.x, j - origin.y, k - origin.z);
    }

    public Chunk GetOrCreateChunk(Vector3Int origin) {
        if (map.ContainsKey(origin)) {
            return map[origin];
        }
        map[origin] = new Chunk(size, origin);
        map[origin].Chunks = this;
        return map[origin];
    }

    public Chunk GetChunk(Vector3Int origin) {
        Chunk chunk = null;
        map.TryGetValue(origin, out chunk);
        return chunk;
    }

    public bool HasChunk(Vector3Int origin) {
        return map.ContainsKey(origin);
    }

    public void Set(int i, int j, int k, float v)
    {
        var origin = getOrigin(i, j, k);
        var chunk = GetOrCreateChunk(origin);
        chunk.Set(i - origin.x, j - origin.y, k - origin.z, v);
    }

    public void SetIfHigher(int i, int j, int k, float v)
    {
        var origin = getOrigin(i, j, k);
        var chunk = GetOrCreateChunk(origin);
        chunk.SetIfHigher(i - origin.x, j - origin.y, k - origin.z, v);
    }

    public void SetColor(int i, int j, int k, Color v)
    {
        var origin = getOrigin(i, j, k);
        var chunk = GetOrCreateChunk(origin);
        chunk.SetColor(i - origin.x, j - origin.y, k - origin.z, v);
    }
}
