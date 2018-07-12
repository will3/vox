using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mesher
{
    private static MarchingCubes marching = new MarchingCubes();

    public static void MeshChunk(Chunk chunk, Chunks chunks, Transform transform, Material material) {
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
        var data = new MarchingCubeData(chunk, chunks);

        marching.Generate(data, chunk.Size, chunk.Size, chunk.Size, verts, indices, colors);

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

    public static void MeshChunks(Chunks chunks, Transform transform, Material material) {
        foreach(var kv in chunks.Map) {
            var chunk = kv.Value;
            MeshChunk(chunk, chunks, transform, material);
        }
    }

    class MarchingCubeData : MarchingCubes.IMarchingCubesData {
        private Chunk chunk;
        private Chunks chunks;

        public MarchingCubeData(Chunk chunk, Chunks chunks) {
            this.chunk = chunk;
            this.chunks = chunks;
        }

        public float GetValue(int i, int j, int k) {
            int max = chunks.Size - 1;
            if (i < 0 || i > max || j < 0 || j > max || k < 0 || k > max)
            {
                var origin = chunk.Origin;
                return chunks.Get(i + origin.x, j + origin.y, k + origin.z);
            }
            return chunk.Get(i, j, k);               
        }

        public Color GetColor(int i, int j, int k) {
            return new Color(1.0f, 1.0f, 1.0f);
        }
    }
}
