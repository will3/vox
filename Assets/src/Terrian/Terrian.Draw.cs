using UnityEngine;
using UnityEngine.Profiling;

namespace FarmVox
{

    public partial class Terrian
    {
        public bool GenerateMesh(Chunks chunks, Vector3Int origin, Material material, TerrianChunk terrianChunk)
        {
            if (!chunks.HasChunk(origin))
            {
                return false;
            }

            var chunk = chunks.GetChunk(origin);

            if (!chunk.Dirty)
            {
                return false;
            }

            if (chunk.Mesh != null)
            {
                Object.Destroy(chunk.Mesh);
            }

            chunk.Mesh = MeshGPU(chunk, terrianChunk);
            chunk.GetMeshRenderer().material = material;
            chunk.GetMeshFilter().sharedMesh = chunk.Mesh;
            chunk.GetMeshCollider().sharedMesh = chunk.Mesh;
            chunk.GetGameObject().layer = (int)chunks.userLayer;

            chunk.Dirty = false;

            TerrianNavMeshBuilder.TriggerBuild();

            shadowMap.ChunkDrawn(origin);

            return true;
        }

        public void GenerateMeshes(TerrianColumn column) {
            foreach (var terrianChunk in column.TerrianChunks)
            {
                foreach (var chunks in chunksToDraw)
                {
                    GenerateMesh(chunks, terrianChunk.Origin, terrianChunk.Material, terrianChunk);
                }
            }
        }

        Mesh MeshGPU(Chunk chunk, TerrianChunk terrianChunk)
        {
            var chunks = chunk.Chunks;
            var mesherGPU = new MesherGPU(size);
            var voxelBuffer = mesherGPU.CreateVoxelBuffer();
            var colorsBuffer = mesherGPU.CreateColorBuffer();
            var trianglesBuffer = mesherGPU.CreateTrianglesBuffer();
            mesherGPU.useNormals = chunks.useNormals;
            mesherGPU.isWater = chunks.isWater;

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
