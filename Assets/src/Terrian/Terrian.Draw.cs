using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace FarmVox
{

    public partial class Terrian
    {
        public bool Draw(Chunks chunks, Vector3Int origin, Transform transform, Material material, TerrianChunk terrianChunk)
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
                Object.Destroy(chunk.GameObject);
            }

            var mesh = MeshGPU(chunk, terrianChunk);

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

            chunk.Dirty = false;
            return true;
        }

        public void Draw(HashSet<Vector3Int> updatedTerrianChunks) {
            foreach (var kv in map)
            {
                var terrianChunk = kv.Value;
                if (terrianChunk.Distance < config.drawDis)
                {
                    foreach (var chunks in chunksToDraw)
                    {
                        if (Draw(chunks, terrianChunk.Origin, Transform, material, terrianChunk))
                        {
                            updatedTerrianChunks.Add(terrianChunk.Origin);
                        }
                    }
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
