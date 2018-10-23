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

            chunk.Mesh = MeshGpu(chunk);
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

        private Mesh MeshGpu(Chunk chunk)
        {
            var chunks = chunk.Chunks;
            var mesher = new MesherGpu(Size);
            
            using (var voxelBuffer = mesher.CreateVoxelBuffer())
            using (var colorBuffer = mesher.CreateColorBuffer())
            using (var trianglesBuffer = mesher.CreateTrianglesBuffer())
            {
                mesher.UseNormals = chunks.UseNormals;
                mesher.IsWater = chunks.IsWater;

                voxelBuffer.SetData(chunk.Data);
                colorBuffer.SetData(chunk.Colors);

                mesher.Dispatch(voxelBuffer, colorBuffer, trianglesBuffer, chunk);
            
                var triangles = mesher.ReadTriangles(trianglesBuffer);

                var builder = new MeshBuilder();
                var mesh = builder.AddTriangles(triangles).Build();
                return mesh;
            }
        }
    }
}
