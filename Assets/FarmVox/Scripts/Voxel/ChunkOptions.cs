
using System;

namespace FarmVox.Scripts.Voxel
{
    [Serializable]
    public class ChunkOptions
    {
        public bool transparent;
        public int normalBanding;
        public float normalStrength;
        public float shadowStrength;
        public bool useNormals;
        public bool isWater;
        public float aoStrength = 0.15f;
    }
}