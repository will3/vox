using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private readonly float[] data;
    private readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();
    private readonly Dictionary<Vector3Int, float> lightingMap = new Dictionary<Vector3Int, float>();

    private Mesh mesh;
    private GameObject gameObject;
    private readonly int size;
    private Vector3Int origin;
    private bool dirty;
    public bool Hidden;
    public Chunks Chunks;

    public Vector3Int Origin
    {
        get
        {
            return origin;
        }
    }

    public float[] Data
    {
        get
        {
            return data;
        }
    }

    public int Size { get { return size; } }

    public Mesh Mesh
    {
        get
        {
            return mesh;
        }

        set
        {
            mesh = value;
        }
    }

    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }

        set
        {
            gameObject = value;
        }
    }

    public bool Dirty
    {
        get
        {
            return dirty;
        }

        set
        {
            dirty = value;
        }
    }

    public Chunk(int size, Vector3Int origin)
    {
        this.size = size;
        this.origin = origin;
        data = new float[size * size * size];
    }

    public float Get(int i, int j, int k)
    {
        var index = getIndex(i, j, k);
        return data[index];
    }

    public void Set(int i, int j, int k, float v)
    {
        var index = getIndex(i, j, k);
        data[index] = v;
        dirty = true;
    }

    public void SetIfHigher(int i, int j, int k, float v) {
        var index = getIndex(i, j, k);
        if (data[index] < v) {
            data[index] = v;
        }
        dirty = true;
    }

    public void SetColor(int i, int j, int k, Color v) {
        var index = getIndex(i, j, k);
        colors[index] = v;
    }

    public void SetIfHigherGlobal(int i, int j, int k, float v) {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            Chunks.SetIfHigher(i + origin.x, j + origin.y, k + origin.z, v);
        }
        else
        {
            SetIfHigher(i, j, k, v);
        }
    }

    public void SetGlobal(int i, int j, int k, float v) {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            Chunks.Set(i + origin.x, j + origin.y, k + origin.z, v);
        } else {
            Set(i, j, k, v);
        }
    }

    public void SetColorGlobal(int i, int j, int k, Color color)
    {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            Chunks.SetColor(i + origin.x, j + origin.y, k + origin.z, color);
        }
        else
        {
            SetColor(i, j, k, color);
        }
    }

    public float GetLightingGlobal(int i, int j, int k)
    {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            return Chunks.GetLighting(i + origin.x, j + origin.y, k + origin.z);
        }
        else
        {
            return GetLighting(i, j, k);
        }
    }

    public float GetGlobal(int i, int j, int k) {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            return Chunks.Get(i + origin.x, j + origin.y, k + origin.z);
        }
        else
        {
            return Get(i, j, k);
        }
    }

    public Color GetColor(int i, int j, int k) {
        var index = getIndex(i, j, k);
        Color color;
        colors.TryGetValue(index, out color);
        return color;
    }

    public Color GetColorGlobal(int i, int j, int k) {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            return Chunks.GetColor(i + origin.x, j + origin.y, k + origin.z);
        }
        else
        {
            return GetColor(i, j, k);
        }
    }

    private int getIndex(int i, int j, int k) {
        int index = i * size * size + j * size + k;
        return index;
    }

    public bool InBound(int i, int j, int k) {
        int max = size - 1;
        if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
        {
            return false;
        }
        return true;
    }

    public float GetLighting(int i, int j, int k) {
        Vector3Int coord = new Vector3Int(i, j, k);
        return GetLighting(coord);
    }

    public float GetLighting(Vector3Int coord) {
        if (lightingMap.ContainsKey(coord)) {
            return lightingMap[coord];    
        }

        return 1.0f;
    }

    public void SetLighting(Vector3Int coord, float v) {
        lightingMap[coord] = v;
    }
}
