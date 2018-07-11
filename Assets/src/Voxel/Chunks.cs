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

    public Chunk GetOrCreateChunk(Vector3Int origin) {
        if (map.ContainsKey(origin)) {
            return map[origin];
        }
        map[origin] = new Chunk(size, origin);
        return map[origin];
    }

    public void Set(int i, int j, int k, float v)
    {
        var origin = getOrigin(i, j, k);
        if (!map.ContainsKey(origin))
        {
            map[origin] = new Chunk(size, origin);
        }

        map[origin].Set(i - origin.x, j - origin.y, k - origin.z, v);
    }
}
