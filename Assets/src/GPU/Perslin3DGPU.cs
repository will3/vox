using UnityEngine;
using System.Collections;

public class Perlin3DGPU
{
    private int size;
    public float frequency = 0.01f;
    public float amplitude = 1.0f;
    public int seed = 1337;
    private int workGroups = 8;
    private float lacunarity = 2.0f;
    private float persistence = 0.5f;
    private int octaves = 2;

    private ComputeShader computeShader;
    private ComputeBuffer results;

    public Perlin3DGPU(int size) {
        this.size = size;
        this.computeShader = Resources.Load<ComputeShader>("Shaders/Perlin3D");
        this.results = new ComputeBuffer(size * size * size, sizeof(float));
    }

    public void Dispatch() {
        computeShader.SetBuffer(0, "_Results", results);
        computeShader.SetFloat("_Persistence", persistence);
        computeShader.SetInt("_Size", size);
        computeShader.SetInt("_Seed", seed);
        computeShader.SetFloat("_Frequency", frequency);
        computeShader.SetFloat("_Lacunarity", lacunarity);
        computeShader.SetInt("_Octaves", octaves);

        computeShader.Dispatch(0, size / workGroups, size / workGroups, size / workGroups);
    }

    public void ReleaseBuffer() {
        results.Release();
    }

    public float[] Read() {
        var data = new float[size * size * size];
        results.GetData(data);

        var min = Mathf.Infinity;
        var max = -Mathf.Infinity;

        for (var i = 0; i < data.Length; i++) {
            var v = data[i];
            if (v > max) {
                max = v;
            } 
            if (v < min) {
                min = v;
            }
        }
        Debug.Log(min);
        Debug.Log(max);
        return data;
    }
}
