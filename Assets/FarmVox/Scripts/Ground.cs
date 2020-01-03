using FarmVox.GPU.Shaders;
using FarmVox.Terrain;
using FarmVox.Voxel;
using UnityEngine;

namespace FarmVox.Scripts
{
    public class Ground : MonoBehaviour
    {
        public Chunks chunks;
        
        public void GenerateChunk(TerrianChunk terrianChunk, Terrian terrian)
        {
            var origin = terrianChunk.Origin;
            var chunk = chunks.GetOrCreateChunk(origin);

            var genTerrianGpu = new GenTerrianGpu(terrian.Config.Size, origin, terrian.Config);

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