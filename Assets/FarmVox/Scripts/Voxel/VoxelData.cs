using UnityEngine;

namespace FarmVox.Scripts.Voxel
{
    public struct VoxelData
    {
        public Vector3Int Coord;
        public Vector3 Normal;

        public static int Size => sizeof(int) * 3 + sizeof(float) * 3;
    }
}