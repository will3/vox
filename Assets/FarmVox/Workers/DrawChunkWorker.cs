using System.Security.Permissions;
using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Threading;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Workers
{
    public class DrawChunkWorker : IWorker
    {
        private readonly TerrianConfig _config;
        private readonly VoxelShadowMap _shadowMap;
        private readonly Chunks _chunks;
        private readonly TerrianChunk _terrianChunk;
        
        public DrawChunkWorker(TerrianConfig config, VoxelShadowMap shadowMap, Chunks chunks, TerrianChunk terrianChunk)
        {
            _config = config;
            _shadowMap = shadowMap;
            _chunks = chunks;
            _terrianChunk = terrianChunk;
        }
        
        public void Start()
        {
            GenerateMesh(_chunks, _terrianChunk);
        }
        
        private void GenerateMesh(Chunks chunks, TerrianChunk terrianChunk)
        {
            var origin = terrianChunk.Origin;
            
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

            _shadowMap.ChunkDrawn(origin);
        }

        private Mesh MeshGpu(Chunk chunk)
        {
            var chunks = chunk.Chunks;
            
            using (var mesher = new MesherGpu(_config.Size, chunk.DataSize, _config))
            {
                mesher.UseNormals = chunks.UseNormals;
                mesher.IsWater = chunks.IsWater;
                mesher.NormalStrength = chunk.Chunks.NormalStrength;
                
                mesher.SetData(chunk.Data);
                mesher.SetColors(chunk.Colors);
                
                mesher.Dispatch();
            
                var triangles = mesher.ReadTriangles();

                var builder = new MeshBuilder();
                var mesh = builder.AddTriangles(triangles).Build();
                return mesh;
            }
        }

        public float Priority
        {
            get { return Priorities.GenTrees; }
        }
    }
}