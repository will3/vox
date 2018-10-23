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
            
            using (var mesher = new MesherGpu(Size))
            {
                mesher.UseNormals = chunks.UseNormals;
                mesher.IsWater = chunks.IsWater;

                mesher.SetData(chunk.Data);
                mesher.SetColors(chunk.Colors);

                mesher.Dispatch();
            
                var triangles = mesher.ReadTriangles();

                var builder = new MeshBuilder();
                var mesh = builder.AddTriangles(triangles).Build();
                return mesh;
            }
        }
    }
}
