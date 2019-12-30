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
        private readonly Chunk _chunk;
        private readonly TerrianChunk _terrianChunk;
        
        public DrawChunkWorker(TerrianConfig config, VoxelShadowMap shadowMap, Chunk chunk, TerrianChunk terrianChunk)
        {
            _config = config;
            _shadowMap = shadowMap;
            _chunk = chunk;
            _terrianChunk = terrianChunk;
        }
        
        public void Start()
        {
            if (!_chunk.Dirty)
            {
                return;
            }

            if (_chunk.Mesh != null)
            {
                Object.Destroy(_chunk.Mesh);
            }

            _chunk.Mesh = MeshGpu(_chunk);
            _chunk.GetMeshRenderer().material = _chunk.Material;
            _chunk.GetMeshFilter().sharedMesh = _chunk.Mesh;
            _chunk.GetMeshCollider().sharedMesh = _chunk.Mesh;

            _chunk.Dirty = false;

            _shadowMap.ChunkDrawn(_chunk.Origin);
        }

        private Mesh MeshGpu(Chunk chunk)
        {
            var chunks = chunk.Chunks;

            var mesherSettings = new MesherSettings
            {
                AoStrength = _config.AoStrength
            };
            
            using (var mesher = new MesherGpu(chunk.DataSize, mesherSettings)) {
                mesher.UseNormals = chunks.useNormals;
                mesher.IsWater = chunks.isWater;
                mesher.NormalStrength = chunk.Chunks.normalStrength;
                
                mesher.SetData(chunk.Data);
                mesher.SetColors(chunk.Colors);
                
                mesher.Dispatch();
            
                var triangles = mesher.ReadTriangles();

                var builder = new MeshBuilder();
                var meshResult = builder.AddTriangles(triangles, _terrianChunk).Build();
                chunk.SetVoxelData(meshResult.VoxelData);
                
                // Update voxel data buffer
                chunk.Material.SetBuffer("_VoxelData", chunk.GetVoxelDataBuffer());
                
                return meshResult.Mesh;
            }
        }
    }
}