using System.Collections.Generic;
using Kaitai;
using UnityEngine;
using Application = UnityEngine.Application;

namespace FarmVox
{
    public class VoxelModel
    {
        public Vector3Int Offset { get; set; }
        
        public readonly List<Vox.Voxel> Voxels = new List<Vox.Voxel>();
        
        public Vector3Int Size { get; set; }
        
        public readonly List<Color> Palette = new List<Color> {
            Colors.GetColor("#654f30"),
            Colors.GetColor("#705836"),
            Colors.GetColor("#ac8956"),
            Colors.GetColor("#4d4232"),
            Colors.GetColor("#676767")
        };
    }
}