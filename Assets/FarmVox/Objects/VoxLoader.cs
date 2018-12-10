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
                            var v = new VoxelModel.Voxel
                            {
                                X = voxel.X,
                                Y = voxel.Z,
                                Z = voxel.Y,
                                ColorIndex = voxel.ColorIndex
                            };
                            voxelModel.Voxels.Add(v);
                        }
                        break;
                    default:
                        break;
                }
            }

            return voxelModel;
        }
    }
}