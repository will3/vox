using System;
using UnityEngine;

namespace FarmVox.Models
{
    public class Model
    {
        [Serializable]
        public class Voxel
        {
            public int X;
            public int Y;
            public int Z;
            public int ColorIndex;
        }
        
        public Color[] Palette;
        public Voxel[] Voxels;
        public int[] Size;
    }
}