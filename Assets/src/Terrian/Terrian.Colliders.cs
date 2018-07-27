using UnityEngine;

namespace FarmVox
{

    public partial class Terrian
    {
        void GenerateColliders() {
            foreach (var kv in map)
            {
                foreach (var chunks in chunksReceivingShadows)
                {
                    var terrianChunk = kv.Value;
                    var origin = terrianChunk.Origin;
                    var chunk = chunks.GetChunk(origin);
                    if (chunk == null)
                    {
                        continue;
                    }
                    GenerateCollider(chunk, terrianChunk);
                }
            }
        }

        void GenerateCollider(Chunk chunk, TerrianChunk terrianChunk) {
            if (!chunk.colliderDirty) {
                return;
            }

            var colliderMesh = MeshColliderGPU(chunk, terrianChunk);
            chunk.ColliderMesh = colliderMesh;
            chunk.GetMeshCollider().sharedMesh = colliderMesh;

            TerrianNavMeshBuilder.TriggerBuild();

            chunk.colliderDirty = false;
        }

        Mesh MeshColliderGPU(Chunk chunk, TerrianChunk terrianChunk)
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
