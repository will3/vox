using UnityEngine;
using System.Collections;

public class Array3
{
    private int width;

    public int Width
    {
        get
        {
            return width;
        }
    }

    private int height;

    public int Height
    {
        get
        {
            return height;
        }
    }

    private int depth;

    public int Depth
    {
        get
        {
            return depth;
        }
    }

    private float[] data;

    public Array3(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        data = new float[width * height * depth];
    }

    public float Get(int i, int j, int k)
    {
        var index = i * depth * height + j * depth + k;
        return data[index];
    }

    public void Set(int i, int j, int k, float v)
    {
        var index = i * depth * height + j * depth + k;
        data[index] = v;
    }
}
