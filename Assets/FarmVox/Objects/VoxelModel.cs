using System.Collections.Generic;
using FarmVox.Terrain;
using UnityEngine;

namespace FarmVox.Objects
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
            ColorUtils.GetColor("#654f30"),
            ColorUtils.GetColor("#705836"),
            ColorUtils.GetColor("#9a7a4d"),
            ColorUtils.GetColor("#4d4232"),
            ColorUtils.GetColor("#818181")
        };
    }
}