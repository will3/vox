using FarmVox.Scripts.GPU.Shaders;
using FarmVox.Scripts.Voxel;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Ground : MonoBehaviour
    {
        public GroundConfig config;
        public Chunks chunks;
        public Water water;

        public void GenerateChunk(Vector3Int origin)
        {
            var chunk = chunks.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(config.size, origin, config, water.config);

            var voxelBuffer = genTerrianGpu.CreateVoxelBuffer();
            var colorBuffer = genTerrianGpu.CreateColorBuffer();

            genTerrianGpu.Dispatch(voxelBuffer, colorBuffer);

            var voxelBufferData = new float[voxelBuffer.count];
            voxelBuffer.GetData(voxelBufferData);

            var colorBufferData = new Color[colorBuffer.count];
            colorBuffer.GetData(colorBufferData);

            chunk.SetColors(colorBufferData);
            chunk.SetData(voxelBufferData);

            voxelBuffer.Dispose();
            colorBuffer.Dispose();
        }

        public void UnloadChunk(Vector3Int origin)
        {
            chunks.UnloadChunk(origin);
        }

        public bool IsGround(Vector3Int coord)
        {
            return chunks.Get(coord) > 0;
        }

        public Chunk GetChunk(Vector3Int coord)
        {
            return chunks.GetChunk(coord);
        }
    }
}