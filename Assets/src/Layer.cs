using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FarmVox
{
    public enum MeshMethod
    {
        MarchingCubes,
        Voxel
    }

    public class Layer
    {
        private int size;
        public MeshMethod method = MeshMethod.Voxel;
        public Chunks Chunks;
        public bool AccurateOffset = false;
        private MarchingCubes marching = new MarchingCubes();

        public Layer(int size = 32)
        {
            this.size = size;
            Chunks = new Chunks(size);
        }


        public void Draw(Vector3Int origin, Transform transform, Material material, TerrianChunk terrianChunk)
        {
            if (!Chunks.HasChunk(origin)) {
                return;
            }

            var chunk = Chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return;
            }

            if (chunk.Mesh != null)
            {
                Object.Destroy(chunk.Mesh);
                Object.Destroy(chunk.GameObject);
            }

            Mesh mesh = new Mesh();

            if (method == MeshMethod.MarchingCubes)
            {
                var verts = new List<Vector3>();
                var indices = new List<int>();
                var colors = new List<Color>();

                marching.AccurateOffset = AccurateOffset;
                marching.Generate(chunk, verts, indices, colors);

                mesh.SetVertices(verts);
                mesh.SetTriangles(indices, 0);
                mesh.SetColors(colors);
            }
            else if (method == MeshMethod.Voxel)
            {
                VoxelMesher.Mesh(chunk, mesh, terrianChunk);
            }

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
    }
}