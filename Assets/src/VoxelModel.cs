﻿using System.Collections.Generic;
using Kaitai;
using UnityEngine;

namespace FarmVox
{
    class VoxelModel
    {
        public VoxelModel(string name) {
            var data = Vox.FromFile(Application.dataPath + "/Resources/vox/" + name + ".vox");
            LoadData(data);
        }

        List<Color> palette = new List<Color> {
            Colors.GetColor("#654f30"),
            Colors.GetColor("#705836"),
            Colors.GetColor("#ac8956"),
            Colors.GetColor("#4d4232")
        };

        List<Vox.Voxel> voxels = new List<Vox.Voxel>();

        void LoadData(Vox data) {
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
                }
            }
        }

        public void Add(Chunks chunks, Vector3Int position) {
            foreach (var voxel in voxels) {
                var color = palette[voxel.ColorIndex - 1];
                var coord = new Vector3Int(voxel.X, voxel.Z, voxel.Y);

                chunks.Set(coord + position, 1.0f);
                chunks.SetColor(coord + position, color);
            }
        }
    }
}