using System;
using UnityEngine;

namespace FarmVox.Models
{
    [Serializable]
    public class Model
    {
        [Serializable]
        public class Voxel
        {
            public int x;
            public int y;
            public int z;
            public int colorIndex;
        }
        
        public Color[] palette;
        public Voxel[] voxels;
        public int[] size;
    }
}