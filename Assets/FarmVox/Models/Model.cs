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
        public int Size;

        public float[] Data { get; private set; }
        public Color[] Colors { get; private set; }

        public void InitData()
        {
            Data = new float[Size * Size * Size];
            Colors = new Color[Size * Size * Size];
            
            foreach (var voxel in Voxels)
            {
                var index = voxel.X * Size * Size + voxel.Y * Size + voxel.Z;

                Data[index] = 1;
                Colors[index] = Palette[voxel.ColorIndex - 1];
            }
        }
    }
}