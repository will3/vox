using System.Threading;
using UnityEngine;

namespace FarmVox
{
    // WIP
    public class Voxel {
        Vector3Int coord;
        public Voxel(Vector3Int coord) {
            this.coord = coord;
        }
    }

    public class VoxelMap
    {
        Octree<Voxel> map;
        BoundsInt bounds;

        private Mutex mutex;

        public VoxelMap(BoundsInt bounds) {
            this.bounds = bounds;
            map = new Octree<Voxel>(bounds);
            mutex = new Mutex();
        }

        public void LoadChunk(Chunk chunk) {
            mutex.WaitOne();
            for (var i = 0; i < chunk.dataSize; i++)
            {
                for (var j = 0; j < chunk.dataSize; j++)
                {
                    for (var k = 0; k < chunk.dataSize; k++)
                    {
                        var index = chunk.GetIndex(i, j, k);
                        var v = chunk.Data[index];
                        if (v > 0) {
                            //var color = chunk.Colors[index];
                            var coord = new Vector3Int(i, j, k);
                            map.Add(coord, new Voxel(coord));
                        }
                    }
                }
            }
            mutex.ReleaseMutex();
        }

        public void LoadChunkAsync(Chunk chunk) {
            Thread thread = new Thread(() => LoadChunk(chunk));
            thread.Start();
        }
    }
}