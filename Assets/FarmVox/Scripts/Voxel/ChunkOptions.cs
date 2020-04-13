
using System;

namespace FarmVox.Scripts.Voxel
{
    [Serializable]
    public class ChunkOptions
    {
        public bool transparent;
        public bool isWater;
        public int normalBanding = 6;
        public float normalStrength = 0.4f;
        public float shadowStrength = 0.5f;
        public float aoStrength = 0.15f;
    }
}