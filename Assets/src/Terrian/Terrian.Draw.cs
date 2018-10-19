using UnityEngine;
using UnityEngine.Profiling;

namespace FarmVox
{

    public partial class Terrian
    {
        private void GenerateMesh(Chunks chunks, Vector3Int origin, TerrianChunk terrianChunk)
        {
            if (!chunks.HasChunk(origin))
            {
                return;
            }

            var chunk = chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return;
            }

            if (chunk.Mesh != null)
            {
                Object.Destroy(chunk.Mesh);
            }

            chunk.Mesh = MeshGpu(chunk, terrianChunk);
            chunk.GetMeshRenderer().material = chunk.Material;
            chunk.GetMeshFilter().sharedMesh = chunk.Mesh;
            chunk.GetMeshCollider().sharedMesh = chunk.Mesh;

            chunk.Dirty = false;

            ShadowMap.ChunkDrawn(origin);
        }

        private void GenerateMeshes(TerrianColumn column) {
            foreach (var terrianChunk in column.TerrianChunks)
            {
                foreach (var chunks in chunksToDraw)
                {
                    GenerateMesh(chunks, terrianChunk.Origin, terrianChunk);
                }
            }
        }

        Mesh MeshGpu(Chunk chunk, TerrianChunk terrianChunk)
        {
            var chunks = chunk.Chunks;
            var mesherGpu = new MesherGpu(Size);
            var voxelBuffer = mesherGpu.CreateVoxelBuffer();
            var colorBuffer = mesherGpu.CreateColorBuffer();

            var trianglesBuffer = mesherGpu.CreateTrianglesBuffer();
            mesherGpu.UseNormals = chunks.UseNormals;
            mesherGpu.IsWater = chunks.IsWater;

            voxelBuffer.SetData(chunk.Data);
            colorBuffer.SetData(chunk.Colors);

            mesherGpu.Dispatch(voxelBuffer, colorBuffer, trianglesBuffer, terrianChunk, chunk);
            var triangles = mesherGpu.ReadTriangles(trianglesBuffer);

            var builder = new MeshBuilder();
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
            colorBuffer.Dispose();
            trianglesBuffer.Dispose();

            return mesh;
        }
    }
}
