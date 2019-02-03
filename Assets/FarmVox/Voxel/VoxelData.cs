using UnityEngine;

namespace FarmVox.Voxel
{
    public struct VoxelData
    {
        public Vector3Int Coord;

        public static int Size
        {
            get
            {
                return sizeof(int) * 3;    
            }
        }
    }
}