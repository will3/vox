using UnityEngine;

namespace FarmVox.Voxel
{
    public struct CoordData
    {
        public Vector3Int Coord;

        public static int Size
        {
            get { return sizeof(int) * 3; }
        }
    }
}