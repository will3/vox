using System.Collections.Generic;
using Kaitai;
using UnityEngine;
using Application = UnityEngine.Application;

namespace FarmVox
{
    public class VoxelModel
    {
        public class Voxel
        {
            public int X;
            public int Y;
            public int Z;
            public int ColorIndex;
        }
        
        public readonly List<Voxel> Voxels = new List<Voxel>();
        
        public Vector3Int Size { get; set; }
        
        public readonly List<Color> Palette = new List<Color> {
            Colors.GetColor("#654f30"),
            Colors.GetColor("#705836"),
            Colors.GetColor("#9a7a4d"),
            Colors.GetColor("#4d4232"),
            Colors.GetColor("#818181")
        };
    }
}