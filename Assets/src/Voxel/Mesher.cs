using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mesher
{
    public static void MeshChunk(Chunk chunk, Chunks chunks, Transform transform, Material material, MarchingCubes marching) {
        if (!chunk.Dirty) {
            return;
        }

        if (chunk.Mesh != null) {
            Object.Destroy(chunk.Mesh);
            Object.Destroy(chunk.GameObject);
        }

        var verts = new List<Vector3>();
        var indices = new List<int>();
        var colors = new List<Color>();

        marching.Generate(chunk, verts, indices, colors);

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.SetColors(colors);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GameObject go = new GameObject("Mesh");
        go.transform.parent = transform;
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.transform.localPosition = chunk.Origin;

        chunk.Mesh = mesh;
        chunk.GameObject = go;
        chunk.Dirty = false;
    }

    public static void MeshChunks(Chunks chunks, Transform transform, Material material, MarchingCubes marching) {
        foreach(var kv in chunks.Map) {
            var chunk = kv.Value;
            MeshChunk(chunk, chunks, transform, material, marching);
        }
    }
}
