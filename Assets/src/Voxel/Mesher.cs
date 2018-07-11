using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mesher
{
    private static void MeshChunk(Chunk chunk, Chunks chunks, Transform transform, Material material) {
        if (chunk.Mesh != null) {
            Object.Destroy(chunk.Mesh);
            Object.Destroy(chunk.GameObject);
        }

        var marching = new MarchingCubes();

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Generate(chunk, chunks, chunk.Size, chunk.Size, chunk.Size, verts, indices);

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
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
    }

    public static void MeshChunks(Chunks chunks, Transform transform, Material material) {
        foreach(var kv in chunks.Map) {
            var chunk = kv.Value;
            if (chunk.Dirty) {
                MeshChunk(chunk, chunks, transform, material);
                chunk.Dirty = false;
            }
        }
    }
}
