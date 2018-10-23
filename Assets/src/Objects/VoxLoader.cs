using System;
using System.Collections.Generic;
using Kaitai;

namespace FarmVox
{
    static class VoxLoader
    {
        public static void LoadData(Vox data, List<Vox.Voxel> voxels) {
            foreach (var chunk in data.Main.ChildrenChunks)
            {
                switch (chunk.ChunkId)
                {
                    case Vox.ChunkType.Main:
                        break;
                    case Vox.ChunkType.Matt:
                        break;
                    case Vox.ChunkType.Pack:
                        break;
                    case Vox.ChunkType.Rgba:
                        break;
                    case Vox.ChunkType.Size:
                        var size = (Vox.Size)chunk.ChunkContent;
                        break;
                    case Vox.ChunkType.Xyzi:
                        var xyzi = (Vox.Xyzi)chunk.ChunkContent;
                        foreach (var voxel in xyzi.Voxels)
                        {
                            voxels.Add(voxel);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}