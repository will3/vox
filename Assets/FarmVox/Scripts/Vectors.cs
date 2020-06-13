using UnityEngine;

namespace FarmVox.Scripts
{
    public static class Vectors
    {
        public static Vector3Int GetVector3Int(int i, int j, int k, int d)
        {
            switch (d)
            {
                case 0:
                    return new Vector3Int(i, j, k);
                case 1:
                    return new Vector3Int(k, i, j);
                default:
                    return new Vector3Int(j, k, i);
            }
        }
    }
}