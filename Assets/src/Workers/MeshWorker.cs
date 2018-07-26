using UnityEngine;
using UnityEngine.AI;

namespace FarmVox
{
    public class MeshWorker : Worker
    {
        Chunk chunk;
        TerrianChunk terrianChunk;
        Transform transform;
        Material material;

        public MeshWorker(Chunk chunk, TerrianChunk terrianChunk, Transform transform, Material material)
        {
            this.chunk = chunk;
            this.terrianChunk = terrianChunk;
            this.transform = transform;
            this.material = material;
        }

        public override void Start()
        {
            if (chunk.Mesh != null)
            {
                Object.Destroy(chunk.Mesh);
                Object.Destroy(chunk.GameObject);
            }

            var mesh = MeshGPU();
            var origin = chunk.Origin;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            var group = chunk.Chunks.GameObject;
            group.name = chunk.Chunks.groupName;
            group.transform.parent = transform;

            GameObject go = new GameObject("Mesh" + origin.ToString());
            go.transform.parent = group.transform;
            go.AddComponent<MeshRenderer>().material = material;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
            go.transform.localPosition = chunk.Origin;

            chunk.Mesh = mesh;
            chunk.GameObject = go;

            TerrianNavMeshBuilder.TriggerBuild();
        }

        Mesh MeshGPU()
        {
            var mesherGPU = new MesherGPU(chunk.Size);
            var voxelBuffer = mesherGPU.CreateVoxelBuffer();
            var colorsBuffer = mesherGPU.CreateColorBuffer();
            var trianglesBuffer = mesherGPU.CreateTrianglesBuffer();
            mesherGPU.useNormals = chunk.Chunks.useNormals;
            mesherGPU.isWater = chunk.Chunks.isWater;

            voxelBuffer.SetData(chunk.Data);
            colorsBuffer.SetData(chunk.Colors);

            mesherGPU.Dispatch(voxelBuffer, colorsBuffer, trianglesBuffer, terrianChunk, chunk);
            var triangles = mesherGPU.ReadTriangles(trianglesBuffer);

            var builder = new VoxelMeshBuilder();

            foreach (var triangle in triangles)
            {
                builder.AddTriangle(triangle);
            }

            var mesh = new Mesh();
            mesh.SetVertices(builder.Vertices);
            mesh.SetTriangles(builder.Indices, 0);

            mesh.SetColors(builder.Colors);
            mesh.uv = builder.Uvs.ToArray();

            voxelBuffer.Dispose();
            colorsBuffer.Dispose();
            trianglesBuffer.Dispose();

            return mesh;
        }
    }
}
