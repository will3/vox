using System;
using System.Collections.Generic;
using Kaitai;
using UnityEngine;

namespace FarmVox
{
    static class VoxLoader
    {
        public static VoxelModel Load(string name) {
            
            var data = Vox.FromFile(Application.dataPath + "/Resources/vox/" + name + ".vox");
            
            var voxelModel = new VoxelModel();
            
            voxelModel.Voxels.Clear();
            
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
                        voxelModel.Size = new Vector3Int((int)size.SizeX, (int)size.SizeY, (int)size.SizeZ);
                        break;
                    case Vox.ChunkType.Xyzi:
                        var chunkContent = (Vox.Xyzi)chunk.ChunkContent;
                        foreach (var voxel in chunkContent.Voxels)
                        {
                            voxelModel.Voxels.Add(voxel);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return voxelModel;
        }
    }
}